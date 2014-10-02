using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;

namespace Typhoon.MF.Presentation.Controls
{
    public class ParameterValue : DockPanel
    {
        #region Fields
        private Bitmap background;
        private Pen border;
        private ushort opacity = 255;
        #endregion

        #region Properties
        public Bitmap Background
        {
            get { return background; }
            set
            {
                CheckAccess();
                if (background != value)
                {
                    background = value;
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
        public ushort Opacity
        {
            get { return opacity; }
            set
            {
                CheckAccess();
                if (opacity != value)
                {
                    opacity = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructor
        public ParameterValue(UIElement ctrl1, UIElement ctrl2)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;

            ctrl1.HorizontalAlignment = HorizontalAlignment.Left;
            ctrl1.VerticalAlignment = VerticalAlignment.Center;
            Children.Add(ctrl1);
            SetDock(ctrl1, Dock.Left);

            ctrl2.HorizontalAlignment = HorizontalAlignment.Right;
            ctrl2.VerticalAlignment = VerticalAlignment.Center;
            Children.Add(ctrl2);
            SetDock(ctrl2, Dock.Right);

            SetMargin(1);
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (background != null)
                dc.Scale9Image(0, 0, ActualWidth, ActualHeight, background, 0, 0, 0, 0, opacity);
            if (border != null)
                dc.DrawRectangle(null, border, 0, 0, ActualWidth, ActualHeight);
        }
    }
}
