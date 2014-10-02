using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class Icon : UIElement
    {
        #region Fields
        private ImageBrush brush;
        private int iconSize;
        #endregion

        #region Properties
        public Bitmap Bitmap
        {
            get { return brush.BitmapSource; }
            set
            {
                VerifyAccess();
                if (brush.BitmapSource != value)
                {
                    brush.BitmapSource = value;
                    Invalidate();
                }
            }
        }
        public int IconSize
        {
            get { return iconSize; }
            set
            {
                VerifyAccess();
                if (iconSize != value)
                {
                    iconSize = value;
                    Width = Height = iconSize;
                }
            }
        }
        public ushort Opacity
        {
            get { return brush.Opacity; }
            set
            {
                CheckAccess();
                if (brush.Opacity != value)
                {
                    brush.Opacity = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructor
        public Icon(Bitmap bitmap, int iconSize)
            : this(bitmap, iconSize, Bitmap.OpacityOpaque)
        {
        }
        public Icon(Bitmap bitmap, int iconSize, ushort opacity)
        {
            brush = new ImageBrush(bitmap);
            brush.Stretch = Stretch.Fill;

            IconSize = iconSize;
            Opacity = opacity;
        }
        #endregion

        #region Event handlers
        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            desiredWidth = desiredHeight = iconSize;
        }
        public override void OnRender(DrawingContext dc)
        {
            if (Bitmap != null)
                dc.DrawRectangle(brush, null, 0, 0, ActualWidth, ActualHeight);
        }
        #endregion
    }
}
