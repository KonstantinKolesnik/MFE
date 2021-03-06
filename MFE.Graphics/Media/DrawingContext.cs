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
        private int dx = 0;
        private int dy = 0;
        private Rect clipRect = Rect.Empty; // in screen coords

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
        public Rect ClippingRectangle // in screen coords
        {
            get { return clipRect; }
        }
        
        //this.GetTranslation = function () { return new Point(translationX, translationY); }
        public Point Translation
        {
            get { return new Point(translationX, translationY); }
        }
        #endregion

        #region Constructors
        public DrawingContext(Bitmap bmp)
        {
            bitmap = bmp;
            clipRect = new Rect(0, 0, bitmap.Width, bitmap.Height); // in screen coords
        }
        public DrawingContext(int width, int height)
            : this(new Bitmap(width, height))
        {
        }
        #endregion

        public void Dispose()
        {
            bitmap.Dispose();
            bitmap = null;
            GC.SuppressFinalize(this);
        }
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

        public void DrawRectangle(Brush brush, Pen pen, int x, int y, int width, int height, int xCornerRadius = 0, int yCornerRadius = 0)
        {
            if (brush != null)
                brush.RenderRectangle(bitmap, pen, translationX + x, translationY + y, width, height, xCornerRadius, yCornerRadius);
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
            if (pen != null && pen.Thickness > 0)
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
            if (source != null)
                bitmap.DrawImage(translationX + x, translationY + y, source, 0, 0, source.Width, source.Height);
        }
        public void DrawImage(Bitmap source, int destinationX, int destinationY, int sourceX, int sourceY, int sourceWidth, int sourceHeight)
        {
            if (source != null)
                bitmap.DrawImage(translationX + destinationX, translationY + destinationY, source, sourceX, sourceY, sourceWidth, sourceHeight);
        }
        public void BlendImage(Bitmap source, int destinationX, int destinationY, int sourceX, int sourceY, int sourceWidth, int sourceHeight, ushort opacity)
        {
            if (source != null)
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
            if (font != null)
                bitmap.DrawText(text, font, (MSMedia.Color)color, translationX + x, translationY + y);
        }
        public bool DrawText(ref string text, Font font, Color color, int x, int y, int width, int height, TextAlignment alignment, TextTrimming trimming, bool wordWrap)
        {
            if (font != null)
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

            return false;
        }
        #endregion

        #region Clipping
        public void PushClippingRectangle(Rect ctrlScreenArea) // in screen coordinates
        {
            if (ctrlScreenArea.Width < 0 || ctrlScreenArea.Height < 0)
                throw new ArgumentException();

            dx = ctrlScreenArea.X - translationX;
            dy = ctrlScreenArea.Y - translationY;
            translationX += dx;
            translationY += dy;

            Rect rect = ctrlScreenArea;
            if (clippingRectangles.Count > 0)
            {
                Rect previousRect = (Rect)clippingRectangles.Peek();
                //Rect previousRect = (Rect)clippingRectangles[clippingRectangles.Count - 1];
                rect = ctrlScreenArea.Intersection(previousRect);
            }
            clippingRectangles.Push(rect);
            //clippingRectangles.Add(rect);
            clipRect = rect;

            bitmap.SetClippingRectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public void PopClippingRectangle()
        {
            if (clippingRectangles.Count > 0)
            {
                clippingRectangles.Pop();
                //clippingRectangles.RemoveAt(clippingRectangles.Count - 1);

                Rect rect;
                if (clippingRectangles.Count == 0)
                    rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
                else
                    rect = (Rect)clippingRectangles.Peek();

                translationX -= dx;
                translationY -= dy;

                clipRect = rect;

                //bitmap.SetClippingRectangle(res.X, res.Y, res.Width, res.Height);
            }
            else
            {
                Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
                clipRect = rect;

                translationX -= dx;
                translationY -= dy;

                //bitmap.SetClippingRectangle(rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
        #endregion
    }
}
