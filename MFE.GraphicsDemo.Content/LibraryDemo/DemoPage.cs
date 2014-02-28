using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using Microsoft.SPOT;

namespace MFE.GraphicsDemo.Content.LibraryDemo
{
    class DemoPage : Panel
    {
        private Button btnBack;

        public Panel Content
        {
            get;
            private set;
        }

        public DemoPage()
            : base(0, 0, DemoManager.Desktop.Width, DemoManager.Desktop.Height)
        {
            //Background = new SolidColorBrush(Color.CornflowerBlue) { Opacity = 256 };
            Background = new LinearGradientBrush(Color.Bisque, Color.Black, 0, 0, 1000, 1000, 220);

            btnBack = new Button(0, 0, 70, 24, DemoManager.FontCourierNew10, "Back", Color.White) { BackgroundUnpressed = DemoManager.Bar };
            btnBack.Click += btnBack_Click;
            Children.Add(btnBack);

            int y = btnBack.Height + 10;
            Content = new Panel(0, y, Width, Height - y);
            Children.Add(Content);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (Parent != null)
                Parent.Children.Remove(this);
        }
    }
}
