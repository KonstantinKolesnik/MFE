using MFE.Graphics.Media;

namespace MFE.Graphics.Controls
{
    public class Desktop : Container
    {
        #region Constructors
        public Desktop(int width, int height)
            : base(0, 0, width, height)
        {
            Name = "Desktop";
            Background = new SolidColorBrush(Color.Black);
        }
        #endregion

        internal override void ProcessTask(RenderTask task)
        {
            GraphicsManager.ProcessTask(task);
        }
    }
}
