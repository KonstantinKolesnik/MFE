using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class Level : UIElement
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
            get
            {
                VerifyAccess();
                return background;
            }
            set
            {
                VerifyAccess();
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
                VerifyAccess();
                return foreground;
            }
            set
            {
                VerifyAccess();
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
                VerifyAccess();
                return orientation;
            }
            set
            {
                VerifyAccess();
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
                VerifyAccess();
                return barCount;
            }
            set
            {
                VerifyAccess();
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
                VerifyAccess();
                return value;
            }
            set
            {
                VerifyAccess();

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
        public Level()
            : this(Orientation.Horizontal)
        {
        }
        public Level(Orientation orientation)
            : this(orientation, 5)
        {
        }
        public Level(Orientation orientation, int barCount)
        {
            Orientation = orientation;
            BarCount = barCount;
            Background = new SolidColorBrush(Colors.DarkGray);
            Foreground = new SolidColorBrush(Colors.White);
            Value = 0;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            if (Orientation == Orientation.Horizontal)
            {
                barThickness = (ActualWidth - gap * (barCount - 1)) / barCount;
                int x = 0;
                for (int i = 1; i <= barCount; i++)
                {
                    int val = ActualHeight * i / barCount;
                    dc.DrawRectangle(
                        i <= activeBarCount ? foreground : background,
                        null,
                        x, ActualHeight - val,
                        barThickness, val);
                    x += barThickness + gap;
                }
            }
            else
            {
                barThickness = (ActualHeight - gap * (barCount - 1)) / barCount;
                int y = ActualHeight - barThickness;
                for (int i = 1; i <= barCount; i++)
                {
                    int val = ActualWidth * i / barCount;
                    dc.DrawRectangle(
                        i <= activeBarCount ? foreground : background,
                        null,
                        ActualWidth - val, y,
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
