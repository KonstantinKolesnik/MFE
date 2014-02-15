using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class ToolButton : Control
    {
        #region Fields
        private Brush foreground;
        private Brush backgroundUnpressed;
        private Brush backgroundPressed;
        #endregion

        #region Properties
        public Brush BackgroundUnpressed
        {
            get { return backgroundUnpressed; }
            set
            {
                if (backgroundUnpressed != value)
                {
                    backgroundUnpressed = value;
                    Invalidate();
                }
            }
        }
        public Brush BackgroundPressed
        {
            get { return backgroundPressed; }
            set
            {
                if (backgroundPressed != value)
                {
                    backgroundPressed = value;
                    Invalidate();
                }
            }
        }
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (foreground != value)
                {
                    foreground = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler Click;
        protected void OnClick(EventArgs e)
        {
            if (Click != null)
                Click(this, e);
        }
        #endregion

        #region Constructors
        public ToolButton(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            backgroundPressed = new SolidColorBrush(ColorUtils.ColorFromRGB(128, 255, 0));
            backgroundPressed.Opacity = 170;

            //backgroundUnpressed = new SolidColorBrush(Color.Black);
            //backgroundUnpressed.Opacity = 70;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(TouchCapture.Captured == this ? backgroundPressed : backgroundUnpressed, null, 0, 0, Width, Height);

            if (foreground != null)
            {
                ushort originalOpacity = foreground.Opacity;
                if (!Enabled)
                    foreground.Opacity = (ushort)(originalOpacity / 2);
                
                //dc.DrawRectangle(foreground, null, 1, 1, Width - 2, Height - 2);

                int a = (Width < Height ? Width : Height) - 2;
                dc.DrawRectangle(foreground, null, (Width - a) / 2, (Height - a) / 2, a, a);

                foreground.Opacity = originalOpacity;
            }
        }

        #region Event handlers
        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (Enabled)
            {
                TouchCapture.Capture(this);
                Invalidate();
            }
        }
        protected override void OnTouchMove(TouchEventArgs e)
        {
            if (TouchCapture.Captured == this && !ContainsScreenPoint(e.Point))
            {
                TouchCapture.ReleaseCapture();
                Invalidate();
            }
        }
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (TouchCapture.Captured == this)
            {
                TouchCapture.ReleaseCapture();
                Invalidate();

                if (ContainsScreenPoint(e.Point))
                    OnClick(EventArgs.Empty);
            }
        }
        #endregion
    }
}
