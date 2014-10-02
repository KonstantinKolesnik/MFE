using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class ProgressBar : UIElement
    {
        #region Fields
        private Orientation orientation = Orientation.Horizontal;
        private Brush background;
        private Brush foreground;
        private int value = 0;
        #endregion

        #region Properties
        public Brush Background
        {
            get { return background; }
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
            get { return foreground; }
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
                //VerifyAccess();
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
        public int Value
        {
            get { return value; }
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
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructors
        public ProgressBar()
            : this(Orientation.Horizontal)
        {
        }
        public ProgressBar(Orientation orientation)
        {
            Orientation = orientation;
            Background = new SolidColorBrush(Colors.DarkGray);
            Foreground = new SolidColorBrush(Colors.White);
            Value = 0;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            if (background != null)
                dc.DrawRectangle(background, null, 0, 0, ActualWidth, ActualHeight);

            if (foreground != null)
            {
                bool horizontal = orientation == Orientation.Horizontal;
                int w = horizontal ? ActualWidth * value / 100 : ActualWidth;
                int h = horizontal ? ActualHeight : ActualHeight * value / 100;

                dc.DrawRectangle(foreground, null, 0, 0, w, h);
            }
        }
    }
}
