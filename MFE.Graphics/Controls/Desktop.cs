using MFE.Graphics.Media;
using MFE.LCD;

namespace MFE.Graphics.Controls
{
    internal class Desktop : Container
    {
        #region Constructors
        public Desktop()
            : base(0, 0, LCDManager.ScreenWidth, LCDManager.ScreenHeight)
        {
            Name = "Desktop";
            Background = new SolidColorBrush(Color.Black);
        }
        #endregion
    }
}
