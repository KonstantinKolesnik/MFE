using MFE.Graphics.Geometry;
using Microsoft.SPOT;
using System;
using System.Collections;
using MSMedia = Microsoft.SPOT.Presentation.Media;

namespace MFE.Graphics.Media
{
    public class DrawingContext : IDisposable
    {
        #region Fields
        private Bitmap bitmap;
        private int translationX = 0;
        private int translationY = 0;
        //private int dx = 0;
        //private int dy = 0;
        private Rect clipRect = Rect.Empty;

        private Stack clippingRectangles = new Stack();
        //private ArrayList clippingRectangles = new ArrayList();
        #endregion

        #region Properties
        public Bitmap Bitmap
        {
            get { return bitmap; }
        }
        public int Width
        {
            get { return bitmap.Width; }
        }
        public int Height
        {
            get { return bitmap.Height; }
        }
        public Rect ClippingRectangle
        {
            get { return clipRect; }
        }
        #endregion

        #region Constructors
        public DrawingContext(Bitmap bmp)
        {
            bitmap = bmp;
        }
        public DrawingContext(int width, int height)
        {
            bitmap = new Bitmap(width, height);
        }
        public void Dispose()
        {
            bitmap.Dispose();
            bitmap = null;
            GC.SuppressFinalize(this);
        }
        #endregion

        internal void Close()
        {
            bitmap = null;
        }

        #region Drawing
        public void Clear()
        {
            bitmap.Clear();
        }

        public void SetPixel(Color color, int x, int y)
        {
            bitmap.SetPixel(translationX + x, translationY + y, (MSMedia.Color)color);
        }
        public Color GetPixel(int x, int y)
        {
            return (Color)bitmap.GetPixel(translationX + x, translationY + y);
        }

        public void DrawRectangle(Brush brush, Pen pen, int x, int y, int width, int height)
        {
            if (brush != null)
                brush.RenderRectangle(bitmap, pen, translationX + x, translationY + y, width, height);

            if (pen != null && pen.Thickness > 0)
                bitmap.DrawRectangle((MSMedia.Color)pen.Color, pen.Thickness, translationX + x, translationY + y, width, height, 0, 0, (MSMedia.Color)0, 0, 0, (MSMedia.Color)0, 0, 0, 0);
        }
        public void DrawFrame(Pen pen, int x, int y, int width, int height, int xCornerRadius, int yCornerRadius)
        {
            if (pen != null && pen.Thickness > 0)
                bitmap.DrawRectangle((MSMedia.Color)pen.Color, pen.Thickness, translationX + x, translationY + y, width, height, xCornerRadius, yCornerRadius, (MSMedia.Color)0, 0, 0, (MSMedia.Color)0, 0, 0, 0);
        }
        public void DrawPolygon(Brush brush, Pen pen, int[] pts)
        {
            if (brush != null)
                brush.RenderPolygon(bitmap, pen, pts);

            if (pen != null && pen.Thickness > 0)
            {
                int nPts = pts.Length / 2;

                for (int i = 0; i < nPts - 1; i++)
                    DrawLine(pen, pts[i * 2], pts[i * 2 + 1], pts[i * 2 + 2], pts[i * 2 + 3]);

                if (nPts > 2)
                    DrawLine(pen, pts[nPts * 2 - 2], pts[nPts * 2 - 1], pts[0], pts[1]);
            }
        }
        public void DrawLine(Pen pen, int x0, int y0, int x1, int y1)
        {
            if (pen != null)
                bitmap.DrawLine((MSMedia.Color)pen.Color, pen.Thickness, translationX + x0, translationY + y0, translationX + x1, translationY + y1);
        }
        public void DrawEllipse(Brush brush, Pen pen, int x, int y, int xRadius, int yRadius)
        {
            if (brush != null)
                brush.RenderEllipse(bitmap, pen, translationX + x, translationY + y, xRadius, yRadius);

            if (pen != null && pen.Thickness > 0)
                bitmap.DrawEllipse((MSMedia.Color)pen.Color, pen.Thickness, translationX + x, translationY + y, xRadius, yRadius, (MSMedia.Color)0x0, 0, 0, (MSMedia.Color)0x0, 0, 0, 0);
        }
        public void DrawImage(Bitmap source, int x, int y)
        {
            bitmap.DrawImage(translationX + x, translationY + y, source, 0, 0, source.Width, source.Height);
        }
        public void DrawImage(Bitmap source, int destinationX, int destinationY, int sourceX, int sourceY, int sourceWidth, int sourceHeight)
        {
            bitmap.DrawImage(translationX + destinationX, translationY + destinationY, source, sourceX, sourceY, sourceWidth, sourceHeight);
        }
        public void BlendImage(Bitmap source, int destinationX, int destinationY, int sourceX, int sourceY, int sourceWidth, int sourceHeight, ushort opacity)
        {
            bitmap.DrawImage(translationX + destinationX, translationY + destinationY, source, sourceX, sourceY, sourceWidth, sourceHeight, opacity);
        }
        public void RotateImage(int angle, int destinationX, int destinationY, Bitmap bitmap, int sourceX, int sourceY, int sourceWidth, int sourceHeight, ushort opacity)
        {
            bitmap.RotateImage(angle, translationX + destinationX, translationY + destinationY, bitmap, sourceX, sourceY, sourceWidth, sourceHeight, opacity);
        }
        public void StretchImage(int xDst, int yDst, int widthDst, int heightDst, Bitmap bitmap, int xSrc, int ySrc, int widthSrc, int heightSrc, ushort opacity)
        {
            bitmap.StretchImage(translationX + xDst, translationY + yDst, widthDst, heightDst, bitmap, xSrc, ySrc, widthSrc, heightSrc, opacity);
        }
        public void TileImage(int xDst, int yDst, Bitmap bitmap, int width, int height, ushort opacity)
        {
            bitmap.TileImage(translationX + xDst, translationY + yDst, bitmap, width, height, opacity);
        }
        public void Scale9Image(int xDst, int yDst, int widthDst, int heightDst, Bitmap bitmap, int leftBorder, int topBorder, int rightBorder, int bottomBorder, ushort opacity)
        {
            bitmap.Scale9Image(translationX + xDst, translationY + yDst, widthDst, heightDst, bitmap, leftBorder, topBorder, rightBorder, bottomBorder, opacity);
        }
        public void DrawText(string text, Font font, Color color, int x, int y)
        {
            bitmap.DrawText(text, font, (MSMedia.Color)color, translationX + x, translationY + y);
        }
        public bool DrawText(ref string text, Font font, Color color, int x, int y, int width, int height, TextAlignment alignment, TextTrimming trimming, bool wordWrap)
        {
            uint flags = 0;// Bitmap.DT_IgnoreHeight;

            if (wordWrap)
                flags |= Bitmap.DT_WordWrap;

            // Text alignment
            switch (alignment)
            {
                case TextAlignment.Left: flags |= Bitmap.DT_AlignmentLeft; break;
                case TextAlignment.Center: flags |= Bitmap.DT_AlignmentCenter; break;
                case TextAlignment.Right: flags |= Bitmap.DT_AlignmentRight; break;
                default: throw new NotSupportedException();
            }

            // Trimming
            switch (trimming)
            {
                case TextTrimming.CharacterEllipsis: flags |= Bitmap.DT_TrimmingCharacterEllipsis; break;
                case TextTrimming.WordEllipsis: flags |= Bitmap.DT_TrimmingWordEllipsis; break;
            }

            int xRelStart = 0;
            int yRelStart = 0;
            return bitmap.DrawTextInRect(ref text, ref xRelStart, ref yRelStart, translationX + x, translationY + y, width, height, flags, (MSMedia.Color)color, font);
        }
        #endregion

        #region Clipping
        public void PushClippingRectangle(Rect rect) // in screen coordinates
        {
            if (rect.Width < 0 || rect.Height < 0)
                throw new ArgumentException();

            //dx = rect.X - translationX;
            //dy = rect.Y - translationY;
            translationX = rect.X;
            translationY = rect.Y;

            Rect res = rect;
            if (clippingRectangles.Count > 0)
            {
                Rect previousRect = (Rect)clippingRectangles.Peek();
                //Rect previousRect = (Rect)clippingRectangles[clippingRectangles.Count - 1];
                res = rect.Intersection(previousRect);
            }
            clippingRectangles.Push(res);
            //clippingRectangles.Add(res);
            clipRect = res;

            bitmap.SetClippingRectangle(res.X, res.Y, res.Width, res.Height);
        }
        public void PopClippingRectangle()
        {
            if (clippingRectangles.Count > 0)
            {
                clippingRectangles.Pop();
                //clippingRectangles.RemoveAt(clippingRectangles.Count - 1);

                //Rect res;
                //if (clippingRectangles.Count == 0)
                //    res = new Rect(0, 0, bitmap.Width, bitmap.Height);
                //else
                //    res = (Rect)clippingRectangles.Peek();

                //translationX -= dx;
                //translationY -= dy;

                //clipRect = res;

                //bitmap.SetClippingRectangle(res.X, res.Y, res.Width, res.Height);
            }
            else
            {
                Rect res = new Rect(0, 0, bitmap.Width, bitmap.Height);
                clipRect = res;
                bitmap.SetClippingRectangle(res.X, res.Y, res.Width, res.Height);
            }
        }
        #endregion
    }
}
