using MFE.Graphics.Media;

namespace MFE.Graphics.Controls
{
    public class Panel : Control
    {
        #region Fields
        private Brush background = null;
        private int borderRadius = 0;
        private Pen border = null;
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
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                if (borderRadius != value)
                {
                    borderRadius = value;
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

        #region Constructors
        public Panel(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }
        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(background, border, 0, 0, Width, Height, borderRadius, borderRadius);
        }
    }
}
