using MFE.Graphics.Media;

namespace MFE.Graphics.Controls
{
    public class Level : Control
    {
        #region Fields
        private Brush background;
        private Brush foreground;
        private Orientation orientation;
        private int value = 0;
        private int barCount = 5;
        private int activeBarCount = 0;

        private int gap = 1;
        private int barThickness;
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
            get
            {
                return foreground;
            }
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
            get
            {
                return orientation;
            }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    Invalidate();
                }
            }
        }
        public int BarCount
        {
            get
            {
                return barCount;
            }
            set
            {
                if (value < 1)
                    value = 1;
                //else if (value > 10)
                //    value = 10;
                if (barCount != value)
                {
                    barCount = value;
                    SetActiveBarCount();
                }
            }
        }
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;
                if (this.value != value)
                {
                    this.value = value;
                    SetActiveBarCount();
                }
            }
        }
        #endregion

        #region Constructors
        public Level(int x, int y, int width, int height)
            : this(x, y, width, height, Orientation.Horizontal)
        {
        }
        public Level(int x, int y, int width, int height, Orientation orientation)
            : this(x, y, width, height, orientation, 5)
        {
        }
        public Level(int x, int y, int width, int height, Orientation orientation, int barCount)
            : base(x, y, width, height)
        {
            Orientation = orientation;
            BarCount = barCount;
            Background = new SolidColorBrush(Color.DarkGray);
            Foreground = new SolidColorBrush(Color.White);
            Value = 0;
        }
        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            if (Orientation == Orientation.Horizontal)
            {
                barThickness = (Width - gap * (barCount - 1)) / barCount;
                int x = 0;
                for (int i = 1; i <= barCount; i++)
                {
                    int val = Height * i / barCount;
                    dc.DrawRectangle(
                        i <= activeBarCount ? foreground : background,
                        null,
                        x, Height - val,
                        barThickness, val);
                    x += barThickness + gap;
                }
            }
            else
            {
                barThickness = (Height - gap * (barCount - 1)) / barCount;
                int y = Height - barThickness;
                for (int i = 1; i <= barCount; i++)
                {
                    int val = Width * i / barCount;
                    dc.DrawRectangle(
                        i <= activeBarCount ? foreground : background,
                        null,
                        Width - val, y,
                        val, barThickness);
                    y -= barThickness + gap;
                }
            }
        }

        #region Private methods
        private void SetActiveBarCount()
        {
            int n = 0;
            int step = 100 / barCount;
            bool set = false;
            for (int i = 0; i < barCount; i++)
            {
                if (value <= i * step)
                {
                    n = i;
                    set = true;
                    break;
                }
            }
            if (!set)
                n = barCount;

            if (n != activeBarCount)
            {
                activeBarCount = n;
                Invalidate();
            }
        }
        #endregion
    }
}
