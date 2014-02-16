using MFE.Graphics.Media;

namespace MFE.Graphics.Controls
{
    public abstract class Container : Control
    {
        #region Fields
        private Brush background = null;
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
        //public ControlCollection Children
        //{
        //    get { return children; }
        //}
        #endregion

        #region Constructors
        public Container(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }
        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            if (background != null)
                dc.DrawRectangle(background, null, 0, 0, Width, Height);
        }
    }
}
