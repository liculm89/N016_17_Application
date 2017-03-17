using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Cognex.DataMan.Utils
{
    public delegate void LogDelegate(string aMsg);

    /// <summary>
    /// File logger class
    /// </summary>
    public class FileLogger
    {
        /// <summary>
        /// Creates a file logger
        /// </summary>
        /// <param name="aLogFileName">file to log to</param>
        public FileLogger(string aLogFileName)
        {
            m_LogFileName = aLogFileName;
        }
        
        /// <summary>
        /// Closes log file
        /// </summary>
        public void Close()
        {
            CloseLogFile();
        }

        /// <summary>
        /// Logs a messsage
        /// </summary>
        /// <param name="aMsg">message to log</param>
        public void Log(string aMsg)
        {
            try
            {
                lock (m_LockLog)
                {
                    ++m_LogId;

                    if (OpenLogFile())
                    {
                        StringBuilder line = new StringBuilder();

                        if (m_UseTimeStamp)
                            line.Append(DateTime.Now.ToLongTimeString());
                        else
                            line.Append(m_LogId);

                        line.Append(": ");
                        line.Append(aMsg);

                        m_LogFile.WriteLine(line.ToString());
                        m_LogFile.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Error: FileLogger.Log('{0}') failed: {1}", aMsg, ex.Message));
            }
        }

        void CloseLogFile()
        {
            try
            {
                lock (m_LockLog)
                {
                    if (m_LogFile != null)
                    {
                        m_LogFile.Close();
                        m_LogFile = null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Error: FileLogger.CloseLogFile failed: {0}", ex.Message));
            }
        }

        bool OpenLogFile()
        {
            if (m_LogFile != null)
                return true;

            try
            {
                int CurrentLogFileLength = 0;
                
                try
                {
                    System.IO.FileInfo FI = new System.IO.FileInfo(m_LogFileName);
                    CurrentLogFileLength = (int)FI.Length;
                }
                catch
                {
                }

                lock (m_LockLog)
                {
                    //open log file
                    System.IO.FileMode Mode = CurrentLogFileLength > 0 && CurrentLogFileLength < 10000 ? System.IO.FileMode.Append : System.IO.FileMode.Create;
                    System.IO.FileStream F = new System.IO.FileStream(m_LogFileName, Mode, System.IO.FileAccess.Write, System.IO.FileShare.Read);

                    m_LogFile = new System.IO.StreamWriter(F);
                    m_LogFile.WriteLine(String.Format("-----{0} {1}-----", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString()));
                }
                
                return true;
            }
            catch (Exception ex)
            {
                //Another instance of this app is already running: quit immediately.
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Console.WriteLine("Warn: Could not open log file: {0}", ex.Message);
#endif
                return false;
            }
        }

        #region Disposing
        bool IsDisposed = false;
        
        ~FileLogger()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool aDisposeCalledByUser)
        {
            try
            {
                if (!IsDisposed)
                {
                    IsDisposed = true;
                    if (aDisposeCalledByUser)
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Error: FileLogger.Dispose failed: {0}", ex.Message));
            }
        }
        #endregion  //Disposing

        /// <summary>
        /// Use time stamps in log messages
        /// </summary>
        public bool m_UseTimeStamp = false;

        private int m_LogId = 0;
        private System.IO.StreamWriter m_LogFile;
        private object m_LockLog = new object();
        private string m_LogFileName;
    }
}
