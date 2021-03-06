using MFE.Graphics.Media;

namespace MFE.Graphics.Controls
{
    public class ProgressBar : Control
    {
        #region Fields
        private Orientation orientation = Orientation.Horizontal;
        private Brush background;
        private Brush foreground;
        private Pen border;
        private int value = 0;
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
                }
            }
        }
        #endregion

        #region Constructors
        public ProgressBar(int x, int y, int width, int height, Orientation orientation = Orientation.Horizontal)
            : base(x, y, width, height)
        {
            Orientation = orientation;
            Background = new SolidColorBrush(Color.DarkGray);
            Foreground = new SolidColorBrush(Color.White);
            Border = new Pen(Color.Gray, 1);
            Value = 0;
        }
        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(background, null, 0, 0, Width, Height);

            if (foreground != null)
            {
                bool horizontal = orientation == Orientation.Horizontal;
                int w = horizontal ? Width * value / 100 : Width;
                int h = horizontal ? Height : Height * value / 100;
                int y = horizontal ? 0 : Height - h;

                dc.DrawRectangle(foreground, null, 0, y, w, h);
            }

            dc.DrawRectangle(null, border, 0, 0, Width, Height);
        }
    }
}
