using MFE.Graphics.Controls;
using MFE.Graphics.Media;

namespace MFE.GraphicsDemo.Content.LibraryDemo
{
    class SplashPage : Panel
    {
        private TextBlock tbTitle;

        public SplashPage()
            : base(0, 0, DemoManager.Desktop.Width, DemoManager.Desktop.Height)
        {
            tbTitle = new TextBlock(0, 0, DemoManager.Desktop.Width, DemoManager.Desktop.Height, DemoManager.FontTitle, "MFE Graphics Library")
            {
                ForeColor = Color.Blue,//.CornflowerBlue,
                TextAlignment = TextAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Center,
                TextWrap = true
            };
            Children.Add(tbTitle);
        }
    }
}
