using MFE.Graphics.Media;

namespace MFE.Graphics.Controls
{
    public class Panel : Container
    {
        #region Fields
        private Pen border;
        #endregion

        #region Properties
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

        #region Constructors
        public Panel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (border != null)
            {
                int corner = 2;
                dc.DrawFrame(border, 0, 0, Width, Height, corner, corner);
            }
        }
    }
}
