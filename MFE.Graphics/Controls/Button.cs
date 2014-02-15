using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using MFE.Utilities;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class Button : Control
    {
        #region Fields
        private Font font;
        private string text;
        private Color foreColor = Color.Black;
        private bool textBelow = false;
        private Brush foreground;
        private Brush backgroundPressed;
        private Brush backgroundUnpressed;
        private Pen border;
        #endregion

        #region Properties
        public Font Font
        {
            get { return font; }
            set
            {
                if (font != value)
                {
                    font = value;
                    Invalidate();
                }
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    Invalidate();
                }
            }
        }
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    Invalidate();
                }
            }
        }

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

        #region Events
        public event EventHandler Click;
        protected void OnClick(EventArgs e)
        {
            if (Click != null)
                Click(this, e);
        }
        #endregion

        #region Constructors
        public Button(int x, int y, int width, int height)
            : this(x, y, width, height, null, "", Color.Black)
        {
        }
        public Button(int x, int y, int width, int height, Font font, string text, Color foreColor)
            : base(x, y, width, height)
        {
            Font = font;
            Text = text;
            ForeColor = foreColor;

            backgroundPressed = new SolidColorBrush(ColorUtils.ColorFromRGB(128, 255, 0));
            backgroundPressed.Opacity = 170;// 70;

            backgroundUnpressed = new SolidColorBrush(ColorUtils.ColorFromRGB(0, 0, 0));
            backgroundUnpressed.Opacity = 10;

            border = new Pen(Color.DarkGray, 1);
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            #region Background
            int b = border != null ? 1 : 0;
            dc.DrawRectangle(TouchCapture.Captured == this ? backgroundPressed : backgroundUnpressed, null, b, b, Width - 2 * b, Height - 2 * b);
            #endregion

            bool hasForeground = foreground != null;
            bool hasText = font != null && !Utils.IsStringNullOrEmpty(text);

            if (hasForeground && !hasText)
            {
                // image only
                ushort originalOpacity = foreground.Opacity;
                if (!Enabled)
                    foreground.Opacity = (ushort)(originalOpacity / 2);

                int a = (Width < Height ? Width : Height) - 2;
                dc.DrawRectangle(foreground, null, (Width - a) / 2, (Height - a) / 2, a, a);
                
                foreground.Opacity = originalOpacity;
            }
            else if (!hasForeground && hasText)
            {
                // text only
                int w = 0, h = 0;
                font.ComputeExtent(text, out w, out h);
                string s = text;
                dc.DrawText(ref s, font, foreColor, 0, (Height - h) / 2, Width, h, TextAlignment.Center, TextTrimming.None, false);
            }
            else if (hasForeground && hasText)
            {
                if (textBelow)
                {


                }
                else
                {



                }
            }

            #region Border
            if (border != null)
            {
                int corner = 3;
                dc.DrawFrame(border, 0, 0, Width, Height, corner, corner);
            }
            #endregion
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
