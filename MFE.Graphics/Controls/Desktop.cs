using MFE.Graphics.Media;

namespace MFE.Graphics.Controls
{
    public class Desktop : Container
    {
        #region Fields
        private GraphicsManager gm = null;
        #endregion

        #region Constructors
        internal Desktop(int width, int height, GraphicsManager gm)
            : base(0, 0, width, height)
        {
            this.gm = gm;

            Name = "Desktop";
            Background = new SolidColorBrush(Color.Black);
        }
        #endregion

        #region Private methods
        internal override void ProcessTask(RenderTask task)
        {
            if (gm != null)
                gm.ProcessTask(task);
        }
        protected override void OnPostRender(DrawingContext dc)
        {
            base.OnPostRender(dc);

            // draw trial version text here
        }
        #endregion
    }
}
