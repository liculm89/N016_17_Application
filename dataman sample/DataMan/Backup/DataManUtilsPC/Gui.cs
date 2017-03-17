using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cognex.DataMan.Utils
{
    public class Gui
    {
        public static double m_MaxImageEnlargeFactor = 16.0;

        /// <summary>
        /// Calculates Zoom factor for placing the image into a control, while keeping the aspect ratio of the image.
        /// </summary>
        /// <param name="aImageSize">Size of the image in pixels</param>
        /// <param name="aControlSize">Size of the control in pixels</param>
        /// <returns>Zoom factor to be applied to the image, in order to fit it into the control</returns>
        public static double GetZoomForImageInControl(Size aImageSize, Size aControlSize)
        {
            if (aImageSize.Height <= 0 || aImageSize.Width <= 0 || aControlSize.Height <= 0 || aControlSize.Width <= 0)
                return 1.0;

            double AspectRatioImg = (double)aImageSize.Width / aImageSize.Height;
            double AspectRatioCtl = (double)aControlSize.Width / aControlSize.Height;

            if (AspectRatioCtl < AspectRatioImg)
            {
                return Math.Min(m_MaxImageEnlargeFactor, (double)aControlSize.Width / (double)aImageSize.Width);
            }
            else
            {
                return Math.Min(m_MaxImageEnlargeFactor, (double)aControlSize.Height / (double)aImageSize.Height);
            }
        }

        /// <summary>
        /// Fitting an image into a control, while keeping the aspect ratio.
        /// </summary>
        /// <param name="aImageSize">Size of the image</param>
        /// <param name="aControlSize">Size of the control</param>
        /// <returns>Calculated size of the zoomed image</returns>
        public static Size FitImageInControl(Size aImageSize, Size aControlSize, out double aZoom)
        {
            aZoom = GetZoomForImageInControl(aImageSize, aControlSize);

            Size NewImageSize = new Size((int)Math.Round(aImageSize.Width * aZoom), (int)Math.Round(aImageSize.Height * aZoom));
            return NewImageSize;
        }

        /// <summary>
        /// Fitting an image into a control, while keeping the aspect ratio.
        /// </summary>
        /// <param name="aImageSize">Size of the image</param>
        /// <param name="aControlSize">Size of the control</param>
        /// <returns>Calculated size of the zoomed image</returns>
        public static Size FitImageInControl(Size aImageSize, Size aControlSize)
        {
            double Zoom;
            return FitImageInControl(aImageSize, aControlSize, out Zoom);
        }

        /// <summary>
        /// Calculates boundary rectangle of an image, which placed into a control, while keeping its aspect ratio. 
        /// </summary>
        /// <param name="aImageSize">Size of the image</param>
        /// <param name="aControlRect">Bounds of the control</param>
        /// <param name="aZoom">Calculated Image zoom factor (output)</param>
        /// <returns>Boundary rect for the image, zoomed and centered as necessary</returns>
        public static Rectangle FitImageInControl(Size aImageSize, Rectangle aControlRect, out double aZoom)
        {
            Size NewImgSize = FitImageInControl(aImageSize, aControlRect.Size, out aZoom);
            int RoomLeftX = Math.Max(0, NewImgSize.Width - aControlRect.Width);
            int RoomLeftY = Math.Max(0, NewImgSize.Height - aControlRect.Height);

            Rectangle NewImageRect;
            if (RoomLeftY < RoomLeftX)
            {
                //image is filling up the control in Y direction: center image horizontally in control
                NewImageRect = new Rectangle(aControlRect.Left + RoomLeftX / 2, 0, NewImgSize.Width, NewImgSize.Height);
            }
            else
            {
                //image is filling up the control in X direction: center image vertically in control
                NewImageRect = new Rectangle(0, aControlRect.Top + RoomLeftY / 2, NewImgSize.Width, NewImgSize.Height);
            }

            return NewImageRect;
        }

        /// <summary>
        /// Calculates boundary rectangle of an image, which placed into a control, while keeping its aspect ratio. 
        /// </summary>
        /// <param name="aImageSize">Size of the image</param>
        /// <param name="aControlRect">Bounds of the control</param>
        /// <returns>Boundary rect for the image, zoomed and centered as necessary</returns>
        public static Rectangle FitImageInControl(Size aImageSize, Rectangle aControlRect)
        {
            double Zoom;
            return FitImageInControl(aImageSize, aControlRect, out Zoom);
        }

        /// <summary>
        /// Resizes a bitmap
        /// </summary>
        /// <param name="aSrcImg">Source image</param>
        /// <param name="aDestImgSize">Destination image size</param>
        /// <returns>Resized image</returns>
        public static Bitmap ResizeImageToBitmap(Image aSrcImg, Size aDestImgSize)
        {
            Bitmap NewImg = new Bitmap(aDestImgSize.Width, aDestImgSize.Height);
            Graphics NewGraphics = Graphics.FromImage((Image)NewImg);
#if !WindowsCE
            NewGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
#endif
            NewGraphics.DrawImage(aSrcImg, new Rectangle(0, 0, aDestImgSize.Width, aDestImgSize.Height), new Rectangle(0, 0, aSrcImg.Width, aSrcImg.Height), GraphicsUnit.Pixel);
            NewGraphics.Dispose();

            return NewImg;
        }

        public static byte[] BitmapToBytes(Bitmap aImg, System.Drawing.Imaging.ImageFormat aImageFormat)
        {
            try
            {
                if (aImg == null)
                    return null;

                System.IO.MemoryStream MS = new System.IO.MemoryStream();
                aImg.Save(MS, aImageFormat);
                byte[] ImgBytes = MS.GetBuffer();
                MS.Close();
                MS = null;
                return ImgBytes;
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap BytesToBitmap(byte[] aImgData)
        {
            if (aImgData == null)
                return null;

            return BytesToBitmap(aImgData, 0, aImgData.Length);
        }

        public static Bitmap BytesToBitmap(byte[] aImgData, int aOffset, int aByteCount)
        {
            try
            {
                if (aImgData == null)
                    return null;

                if (aImgData.Length < 1 || aByteCount < 1)
                    return null;

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                ms.Write(aImgData, 0, aImgData.Length);
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                //converting bytes to bitmap
                Bitmap TmpImage = new Bitmap(ms);
                // we need to construct 2 bitmaps so we can close the memory stream before returning.
                // From MS Documentation, "You must keep the stream open for the lifetime of the Bitmap."
                Bitmap TheImage = new Bitmap(TmpImage);

                TmpImage.Dispose();
                TmpImage = null;
                ms.Close();
                ms = null;

                return TheImage;
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap StreamToBitmap( System.IO.Stream aImgStream, int aByteCount)
        {
            try
            {
                if (aImgStream == null || !aImgStream.CanRead)
                    return null;

                if (aByteCount < 1)
                    return null;

                byte[] ImgData = new byte[aByteCount];
                if( aImgStream.Read( ImgData, 0, aByteCount) != aByteCount ) 
                    return null;
                
                return Gui.BytesToBitmap(ImgData, 0, aByteCount);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Describes the DMCC graphics result
    /// </summary>
    public class ResultGraphics
    {
        public ResultGraphics()
        {
            m_Polygons = new List<ResultPolygon>();
            m_ViewBoxSize = new Size(1280, 1024);   //default graphics size , according to modern readers
            m_OrigSvgData = "";
        }

        public List<ResultPolygon> m_Polygons;  //Polygons found in SVG (frame rectangle as well as boundary polygon around the code)
        public Size m_ViewBoxSize;              //ViewBox boundaries found in SVG (these are the original image dimensions)
        public string m_OrigSvgData;            //Original SVG data
    }

    /// <summary>
    /// Describes one DMCC polygon in the result graphics
    /// </summary>
    public class ResultPolygon
    {
        public static Color m_DefaultPolygonColor = Color.LawnGreen;

        public ResultPolygon()
        {
            Clear();
        }

        public void Set(Point[] aPoints)
        {
            m_Points = aPoints;
        }

        public void Clear()
        {
            m_Points = new Point[0];
            m_Color = m_DefaultPolygonColor;
        }

        public Point[] m_Points;
        public Color m_Color;
    }

    /// <summary>
    /// Barebone parser for DMCC's SVG result graphics
    /// </summary>
    public static class GraphicsResultParser
    {
        private static Regex m_RegexpViewBox = new Regex(@"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)");

        public static Color UIntToColor(uint aRGBValue)
        {
#if WindowsCE
            return Color.FromArgb((int)((aRGBValue >> 16) & 0xff), (int)((aRGBValue >> 8) & 0xff), (int)((aRGBValue >> 0) & 0xff));
#else
            return Color.FromArgb(255, (int)((aRGBValue >> 16) & 0xff), (int)((aRGBValue >> 8) & 0xff), (int)((aRGBValue >> 0) & 0xff));
#endif
        }

        public static ResultGraphics Parse(string aSvgData, Rectangle aDisplayControlRect)
        {
            ResultGraphics G = new ResultGraphics();
            G.m_OrigSvgData = aSvgData;

            // Parsing ViewBox size from the following node:   viewBox="0 0 1280 1024"
            int viewBoxIndex = aSvgData.IndexOf("viewBox=\"");
            if (viewBoxIndex > 0)
            {
                Match VBM = m_RegexpViewBox.Match(aSvgData, viewBoxIndex);

                if (VBM.Groups.Count > 4)
                    G.m_ViewBoxSize = new Size(Int32.Parse(VBM.Groups[3].Value), Int32.Parse(VBM.Groups[4].Value));
            }

            double Zoom;
            Rectangle GraphicRect = Gui.FitImageInControl(G.m_ViewBoxSize, aDisplayControlRect, out Zoom);
            Point GraphicShift = new Point((aDisplayControlRect.Width - GraphicRect.Width) / 2, (aDisplayControlRect.Height - GraphicRect.Height) / 2);

            //If there is only image but no decoded result we don't want to parse out the coordinates
            int dataLength = aSvgData.Length;
            int pointsIndex = aSvgData.IndexOf("points", 0, dataLength);
            int colorIndex = aSvgData.IndexOf("stroke=\"#", 0, dataLength);

            int startIndex;
            int endIndex;
            string coordsString;
            string[] coordsArray;

            //parsing one or more polygons
            while (pointsIndex != -1)
            {
                ResultPolygon Polygon = new ResultPolygon();

                //parsing polygon color
                colorIndex = aSvgData.IndexOf("stroke=\"#", colorIndex, dataLength - colorIndex);
                if (colorIndex >= 0)
                {
                    try
                    {
                        uint ColorValue = UInt32.Parse(aSvgData.Substring(colorIndex + 9, 6), System.Globalization.NumberStyles.HexNumber);
                        Polygon.m_Color = UIntToColor(ColorValue);
                        colorIndex += 9;
                    }
                    catch { }
                }

                //parsing polygon points
                List<Point> Points = new List<Point>();
                startIndex = aSvgData.IndexOf("points", pointsIndex, dataLength - pointsIndex) + 8;
                endIndex = aSvgData.IndexOf('"', startIndex, dataLength - startIndex) - 1;
                coordsString = aSvgData.Substring(startIndex, endIndex - startIndex);
                coordsArray = coordsString.Split(' ', ',');

                Point LastPoint = new Point();
                for (int i = 0; i < coordsArray.Length; i += 2)
                {
                    int PointX = (int)Math.Round(Convert.ToInt32(coordsArray[i + 0]) * Zoom) + GraphicShift.X;
                    int PointY = (int)Math.Round(Convert.ToInt32(coordsArray[i + 1]) * Zoom) + GraphicShift.Y;
                    LastPoint = new Point(PointX, PointY);
                    Points.Add(LastPoint);
                }

                //Adding the first point twice makes drawing easier (see: Graphics.DrawLines)
                if (Points.Count > 0)
                    Points.Add(Points[0]);

                Polygon.Set(Points.ToArray());
                G.m_Polygons.Add(Polygon);
                pointsIndex = aSvgData.IndexOf("points", pointsIndex + 1, dataLength - pointsIndex - 1);
            }

            return G;
        }
    }

    /// <summary>
    /// Utility class that can be used to render graphics result to a graphics surface.
    /// </summary>
    public static class ResultGraphicsRenderer
    {
        public static void PaintResults(Graphics aAreaForPainting, ResultGraphics aResultGraphics)
        {
            if (aResultGraphics != null && aResultGraphics.m_Polygons != null && aResultGraphics.m_Polygons.Count > 0)
            {
                Pen LinePen = new Pen(aResultGraphics.m_Polygons[0].m_Color);

                foreach (ResultPolygon Poly in aResultGraphics.m_Polygons)
                {
                    if (!LinePen.Color.Equals(Poly.m_Color))
                        LinePen.Color = Poly.m_Color;

                    aAreaForPainting.DrawLines(LinePen, Poly.m_Points);
                }
            }
        }
    }
}
