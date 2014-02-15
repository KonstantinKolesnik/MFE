using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;

namespace MFE.Graphics.Controls
{
    public class Slider : Control
    {
        #region Fields
        private Orientation orientation = Orientation.Horizontal;
        private Brush background;
        private Brush foreground;
        private Pen thumbBorder;
        private int value = 0;
        private int thumbSize;
        private Rect thumbArea = Rect.Empty;
        private Rect trackArea = Rect.Empty;
        private int largeChange = 10;
        private Point p;
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
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    Invalidate();
                }
            }
        }
        public int Value
        {
            get { return value; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;
                if (this.value != value)
                {
                    this.value = value;
                    Invalidate();
                    OnValueChanged(new ValueChangedEventArgs() { Value = value });
                }
            }
        }
        public int ThumbSize
        {
            get { return thumbSize; }
            set
            {
                if (thumbSize != value)
                {
                    thumbSize = value;
                    Invalidate();
                }
            }
        }
        public Pen ThumbBorder
        {
            get { return thumbBorder; }
            set
            {
                if (thumbBorder != value)
                {
                    thumbBorder = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Events
        public event ValueChangedEventHandler ValueChanged;
        protected void OnValueChanged(ValueChangedEventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }
        #endregion

        #region Constructors
        public Slider(int x, int y, int width, int height, int thumbSize)
            : this(x, y, width, height, thumbSize, Orientation.Horizontal)
        {
        }
        public Slider(int x, int y, int width, int height, int thumbSize, Orientation orientation)
            : base(x, y, width, height)
        {
            Orientation = orientation;
            ThumbSize = thumbSize;
            ThumbBorder = new Pen(Color.Gray, 1);
            Value = 0;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            bool horizontal = orientation == Orientation.Horizontal;

            // background
            int trackRatio = 7;
            int a = horizontal ? Height / trackRatio : Width / trackRatio;
            if (horizontal)
                trackArea = new Rect(0, (Height - a) / 2, Width, a);
            else
                trackArea = new Rect((Width - a) / 2, 0, a, Height);
            
            Brush b = background ?? new SolidColorBrush(Color.DarkGray);
            Pen border = new Pen(Color.Gray, 0);
            dc.DrawRectangle(b, border, trackArea.X, trackArea.Y, trackArea.Width, trackArea.Height);
            
            // thumb
            int workarea = horizontal ? Width - thumbSize : Height - thumbSize;
            int x = horizontal ? workarea * value / 100 : 0;
            int y = horizontal ? 0 : Height - thumbSize - workarea * value / 100;
            if (horizontal)
                thumbArea = new Rect(x, y, thumbSize, Height);
            else
                thumbArea = new Rect(x, y, Width, thumbSize);

            Brush bb = foreground ?? new LinearGradientBrush(Color.LightGray, Color.Black);
            dc.DrawRectangle(bb, thumbBorder, thumbArea.X, thumbArea.Y, thumbArea.Width, thumbArea.Height);
        }

        #region Event handlers
        protected override void OnTouchDown(TouchEventArgs e)
        {
            Point pp = e.Point;
            PointToClient(ref pp);
            if (thumbArea.Contains(pp))
            {
                TouchCapture.Capture(this);
                p = e.Point;
            }
            else if (trackArea.Contains(pp))
            {
                if (orientation == Orientation.Horizontal)
                {
                    if (pp.X < thumbArea.X)
                        Value -= largeChange;
                    else
                        Value += largeChange;
                }
                else
                {
                    if (pp.Y < thumbArea.Y)
                        Value += largeChange;
                    else
                        Value -= largeChange;
                }
            }
        }
        protected override void OnTouchMove(TouchEventArgs e)
        {
            if (TouchCapture.Captured == this)
            {
                if (orientation == Orientation.Horizontal)
                    Value += e.Point.X - p.X;
                else
                    Value -= e.Point.Y - p.Y;

                p = e.Point;
            }
        }
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (TouchCapture.Captured == this)
                TouchCapture.ReleaseCapture();
        }
        #endregion
    }
}
