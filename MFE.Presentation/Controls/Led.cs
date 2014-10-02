using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class Led : UIElement
    {
        #region Fields
        private Brush activeBrush;
        private Brush inactiveBrush;
        private Pen border;
        private bool isActive = false;
        private bool isCircle = false;
        #endregion

        #region Properties
        public Brush ActiveBrush
        {
            get { return activeBrush; }
            set
            {
                CheckAccess();
                if (activeBrush != value)
                {
                    activeBrush = value;
                    Invalidate();
                }
            }
        }
        public Brush InactiveBrush
        {
            get { return inactiveBrush; }
            set
            {
                CheckAccess();
                if (inactiveBrush != value)
                {
                    inactiveBrush = value;
                    Invalidate();
                }
            }
        }
        public Pen Border
        {
            get { return border; }
            set
            {
                CheckAccess();
                if (border != value)
                {
                    border = value;
                    Invalidate();
                }
            }
        }
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                CheckAccess();
                if (isActive != value)
                {
                    isActive = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructors
        public Led(int radius)
            :this(radius, radius)
        {
            isCircle = true;
            Invalidate();
        }
        public Led(int width, int height)
        {
            Width = width;
            Height = height;
            activeBrush = new SolidColorBrush(ColorUtility.ColorFromRGB(0, 240, 0));
            //activeBrush = new SolidColorBrush(ColorUtility.ColorFromRGB(90, 90, 255));
            inactiveBrush = new SolidColorBrush(Colors.DarkGray);
            border = new Pen(Colors.DarkGray, 1);

            Invalidate();
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            if (isCircle)
                dc.DrawEllipse(isActive ? activeBrush : inactiveBrush, border, ActualWidth / 2, ActualHeight / 2, (ActualWidth - 1) / 2, (ActualHeight - 1) / 2);
            else
                dc.DrawRectangle(isActive ? activeBrush : inactiveBrush, border, 0, 0, ActualWidth, ActualHeight);
        }
    }
}
