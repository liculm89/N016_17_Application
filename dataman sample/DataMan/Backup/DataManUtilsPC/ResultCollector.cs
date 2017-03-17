using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Cognex.DataMan.SDK;

namespace Cognex.DataMan.Utils
{
    /// <summary>
    /// Class representing a complex result
    /// </summary>
    public class ResultInfo
    {
        private int m_ResultId;
        private Image m_Image;
        private string m_ImageGraphics;
        private string m_ReadString;
        private string m_ReadXml;

        /// <summary>
        /// Gets or sets the result's ID
        /// </summary>
        public int ResultId
        {
            get
            {
                return m_ResultId;
            }
            set
            {
                m_ResultId = value;
            }
        }

        /// <summary>
        /// Gets or sets the result's image
        /// </summary>
        public Image Image
        {
            get
            {
                return m_Image;
            }
            set
            {
                m_Image = value;
            }
        }

        /// <summary>
        /// Gets or sets the result's image graphics (SVG)
        /// </summary>
        public string ImageGraphics
        {
            get
            {
                return m_ImageGraphics;
            }
            set
            {
                m_ImageGraphics = value;
            }
        }

        /// <summary>
        /// Gets or sets the read string
        /// </summary>
        public string ReadString
        {
            get
            {
                return m_ReadString;
            }
            set
            {
                m_ReadString = value;
            }
        }

        /// <summary>
        /// Gets or sets the XML result
        /// </summary>
        public string XmlResult
        {
            get
            {
                return m_ReadXml;
            }
            set
            {
                m_ReadXml = value;
            }
        }

        /// <summary>
        /// Gets the time stamp when the resuklt was created (when its first component arrived)
        /// </summary>
        public DateTime ResultArrivedAt
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a complex result
        /// </summary>
        /// <param name="resultId">result ID</param>
        /// <param name="image">result image</param>
        /// <param name="imageGraphics">result graphics</param>
        /// <param name="readString">read string</param>
        /// <param name="readXml">result XML</param>
        public ResultInfo(int resultId, Image image, string imageGraphics, string readString, string readXml)
        {
            ResultId = resultId;
            Image = image;
            ImageGraphics = imageGraphics;
            ReadString = readString;
            XmlResult = readXml;
            ResultArrivedAt = DateTime.Now;
        }

        /// <summary>
        /// Checks if a complex result is complete
        /// </summary>
        /// <param name="requiredResultTypes">set of result types that make a result complete</param>
        /// <returns>true, if the result is comlpete (all specifiied components are collected)</returns>
        public bool IsResultComplete(ResultTypes requiredResultTypes)
        {
            if ((requiredResultTypes & ResultTypes.Image) != 0 && Image == null)
                return false;

            if ((requiredResultTypes & ResultTypes.ImageGraphics) != 0 && ImageGraphics == null)
                return false;

            if ((requiredResultTypes & ResultTypes.ReadString) != 0 && ReadString == null)
                return false;

            if ((requiredResultTypes & ResultTypes.ReadXml) != 0 && XmlResult == null)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Event handler for complex result arrival
    /// </summary>
    /// <param name="sender">sender obejct</param>
    /// <param name="e">completed complex result</param>
    public delegate void ComplexResultArrivedEventHandler(object sender, ResultInfo e);

    /// <summary>
    /// Event handler for partial result arrival
    /// </summary>
    /// <param name="sender">sender obejct</param>
    /// <param name="e">partial complex result</param>
    public delegate void PartialResultDroppedEventHandler(object sender, ResultInfo e);

    /// <summary>
    /// Result collector class. Used for collecting result components by result IDs so that a result is only fired by an event if it is complete (or
    /// timed out or collector result is full). 
    /// </summary>
    public class ResultCollector
    {
        /// <summary>
        /// Event for (complete) complex result arrival
        /// </summary>
        public event ComplexResultArrivedEventHandler ComplexResultArrived;

        /// <summary>
        /// Event for partial xomplex result arrival
        /// </summary>
        public event PartialResultDroppedEventHandler PartialResultDroppend;

        private DataManSystem m_DmSystem;
        private object _currentResultInfoSyncLock = new object();
        private Dictionary<int, ResultInfo> _resultCache = new Dictionary<int, ResultInfo>();
        private int _resultCacheLength = 24;
        private TimeSpan _resultTimeOut = TimeSpan.FromSeconds(10);
        private ResultTypes m_ResultTypes;

        /// <summary>
        /// Ctor. Creats a result collector instance
        /// </summary>
        /// <param name="aDmSystem">DataMan system to use for getting the result components</param>
        /// <param name="aResultTypes">result types that are required for a complete result</param>
        public ResultCollector(DataManSystem aDmSystem, ResultTypes aResultTypes)
        {
            m_DmSystem = aDmSystem;
            m_ResultTypes = aResultTypes;
            m_DmSystem.ReadStringArrived += new ReadStringArrivedHandler(OnReadStringArrived);
            m_DmSystem.ImageGraphicsArrived += new ImageGraphicsArrivedHandler(OnImageGraphicsArrived);
            m_DmSystem.ImageArrived += new ImageArrivedHandler(OnImageArrived);
            m_DmSystem.XmlResultArrived += new XmlResultArrivedHandler(OnXmlResultArrived);
        }

        /// <summary>
        /// Gets or sets the size of the result collector cache (nr. of results)
        /// </summary>
        public int ResultCacheLength
        {
            get
            {
                return _resultCacheLength;
            }
            set
            {
                _resultCacheLength = value;
                ProcessResultQueue();
            }
        }

        /// <summary>
        /// Timout value for a result. After timeout passed if the result is not complete it is dropped and a <see cref="PartialResultDroppedEventHandler"/> is fired
        /// </summary>
        public TimeSpan ResultTimeOut
        {
            get { return _resultTimeOut; }
            set { _resultTimeOut = value; }
        }

        /// <summary>
        /// Empties the result cahce
        /// </summary>
        public void ClearCachedResults()
        {
            lock (_currentResultInfoSyncLock)
            {
                foreach (KeyValuePair<int, ResultInfo> result in _resultCache)
                {
                    if (PartialResultDroppend != null)
                        PartialResultDroppend(this, result.Value);
                }
                _resultCache.Clear();
            }
        }

        private void OnReadStringArrived(object sender, ReadStringArrivedEventArgs args)
        {
            // If the current result info has the same result ID as the one we've just
            // received, store the read string in it, otherwise create a new result info object.
            lock (_currentResultInfoSyncLock)
            {
                if (_resultCache.ContainsKey(args.ResultId))
                    _resultCache[args.ResultId].ReadString = args.ReadString;
                else
                    _resultCache.Add(args.ResultId, new ResultInfo(args.ResultId, null, null, args.ReadString, null));

                ProcessResultQueue();
            }
        }

        private void OnImageGraphicsArrived(object sender, ImageGraphicsArrivedEventArgs args)
        {
            // If the current result info has the same result ID as the one we've just
            // received, store the image graphics in it, otherwise create a new result info object.
            lock (_currentResultInfoSyncLock)
            {
                if (_resultCache.ContainsKey(args.ResultId))
                    _resultCache[args.ResultId].ImageGraphics = args.ImageGraphics;
                else
                    _resultCache.Add(args.ResultId, new ResultInfo(args.ResultId, null, args.ImageGraphics, null, null));

                ProcessResultQueue();
            }
        }

        private void OnImageArrived(object sender, ImageArrivedEventArgs args)
        {
            // If the current result info has the same result ID as the one we've just
            // received, store the image in it, otherwise create a new result info object.
            lock (_currentResultInfoSyncLock)
            {
                if (_resultCache.ContainsKey(args.ResultId))
                    _resultCache[args.ResultId].Image = args.Image;
                else
                    _resultCache.Add(args.ResultId, new ResultInfo(args.ResultId, args.Image, null, null, null));

                ProcessResultQueue();
            }
        }

        private void OnXmlResultArrived(object sender, XmlResultArrivedEventArgs args)
        {
            // If the current result info has the same result ID as the one we've just
            // received, store the xml result in it, otherwise create a new result info object.
            lock (_currentResultInfoSyncLock)
            {
                if (_resultCache.ContainsKey(args.ResultId))
                    _resultCache[args.ResultId].XmlResult = args.XmlResult;
                else
                    _resultCache.Add(args.ResultId, new ResultInfo(args.ResultId, null, null, null, args.XmlResult));

                ProcessResultQueue();
            }
        }

        /// <summary>
        /// Precesses the currently collected results. Checks for complete and timed-out results and throws out the oldest results if the result cache
        /// size is smaller than the amount of partial results.
        /// </summary>
        public void ProcessResultQueue()
        {
            lock (_currentResultInfoSyncLock)
            {
                DateTime now = DateTime.Now;

                // Collect completed results and fire events
                int finishedKey = -1;
                foreach (KeyValuePair<int, ResultInfo> result in _resultCache)
                {
                    if (result.Value.IsResultComplete(m_ResultTypes))
                    {
                        finishedKey = result.Key;
                        if (ComplexResultArrived != null)
                            ComplexResultArrived(this, result.Value);
                        break;
                    }
                }
                // Remove collected results
                if (finishedKey >= 0)
                {
                    _resultCache.Remove(finishedKey);
                }

                // Look for old results and throw them out as partial results
                if (_resultCache.Count > 0)
                {
                    List<int> keysToRemove = new List<int>();
                    foreach (KeyValuePair<int, ResultInfo> result in _resultCache)
                    {
                        if (now - result.Value.ResultArrivedAt > _resultTimeOut)
                        {
                            keysToRemove.Add(result.Key);
                        }
                    }

                    foreach (int key in keysToRemove)
                    {
                        if (PartialResultDroppend != null)
                            PartialResultDroppend(this, _resultCache[key]);
                        _resultCache.Remove(key);
                    }
                }

                // Check for the remaining results if ResultCacheLength is smaller then the cached results' count
                while (_resultCache.Count > _resultCacheLength)
                {
                    DateTime oldestResultTime = now;
                    int oldestResultId = -1;
                    foreach (KeyValuePair<int, ResultInfo> result in _resultCache)
                    {
                        if (result.Value.ResultArrivedAt.CompareTo(oldestResultTime) < 0)
                        {
                            oldestResultTime = result.Value.ResultArrivedAt;
                            oldestResultId = result.Key;
                        }
                    }
                    if (oldestResultId != -1)
                    {
                        ResultInfo removedResult = _resultCache[oldestResultId];
                        _resultCache.Remove(oldestResultId);
                        if (PartialResultDroppend != null)
                            PartialResultDroppend(this, removedResult);
                    }
                }
            }
        }
    }
}
