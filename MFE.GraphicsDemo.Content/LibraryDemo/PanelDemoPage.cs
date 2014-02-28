using MFE.Graphics.Controls;
using MFE.Graphics.Media;

namespace MFE.GraphicsDemo.Demos
{
    class PanelDemo : Panel
    {
        public PanelDemo()
            : base(0, 0, 0, 0)
        {
            Panel pnl;

            pnl = new Panel(10, 10, 100, 40);
            pnl.Background = new LinearGradientBrush(Color.Bisque, Color.Green, 0, 500, 1000, 500);
            pnl.Background.Opacity = 80;
            pnl.Border = new Pen(Color.Blue, 2);
            Children.Add(pnl);
        }
    }
}
