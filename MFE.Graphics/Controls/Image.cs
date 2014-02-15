using MFE.Graphics.Media;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class Image : Control
    {
        #region Fields
        private Brush background = null;
        private ImageBrush brush;
        private Pen border;
        #endregion

        #region Properties
        public Brush Background
        {
            get { return background; }
            set
            {
                if (background != value)
                {
                    background = value;
                    Invalidate();
                }
            }
        }
        public Bitmap Bitmap
        {
            get { return brush.BitmapSource; }
            set
            {
                if (brush.BitmapSource != value)
                {
                    brush.BitmapSource = value;
                    Invalidate();
                }
            }
        }
        public ushort Opacity
        {
            get { return brush.Opacity; }
            set
            {
                if (brush.Opacity != value)
                {
                    brush.Opacity = value;
                    Invalidate();
                }
            }
        }
        public Pen Border
        {
            get { return border; }
            set
            {
                if (border != value)
                {
                    border = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructor
        public Image(int x, int y, int width, int height, Bitmap bitmap)
            : this(x, y, width, height, bitmap, Bitmap.OpacityOpaque)
        {
        }
        public Image(int x, int y, int width, int height, Bitmap bitmap, ushort opacity)
            : base(x, y, width, height)
        {
            brush = new ImageBrush(bitmap);
            brush.Stretch = Stretch.Fill;
            Opacity = opacity;
        }
        #endregion

        #region Event handlers
        public override void OnRender(DrawingContext dc)
        {
            if (background != null)
                dc.DrawRectangle(background, null, 0, 0, Width, Height);

            if (Bitmap != null)
                dc.DrawRectangle(brush, null, 0, 0, Width, Height);

            if (border != null)
                dc.DrawRectangle(null, border, 0, 0, Width, Height);
        }
        #endregion
    }
}
