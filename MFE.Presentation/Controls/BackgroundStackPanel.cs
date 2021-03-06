using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class BackgroundStackPanel : StackPanel
    {
        #region Fields
        private Brush background = null;
        private Pen border = null;
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
        public Pen Border
        {
            get
            {
                VerifyAccess();
                return border;
            }
            set
            {
                VerifyAccess();
                if (border != value)
                {
                    border = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructors
        public BackgroundStackPanel()
            : base()
        {
        }
        public BackgroundStackPanel(Orientation orientation, Brush background, Pen border)
            : base(orientation)
        {
            Background = background;
            Border = border;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            if (background != null || border != null)
                dc.DrawRectangle(background, border, 0, 0, ActualWidth, ActualHeight);
        }
    }
}
