using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using System.Threading;

namespace MFE.GraphicsDemo.Content.LibraryDemo
{
    class ProgressBarDemoPage : DemoPage
    {
        public ProgressBarDemoPage()
        {
            ProgressBar ctrl;

            ctrl = new ProgressBar(20, 20, 50, 30) { Value = 60 };
            Content.Children.Add(ctrl);

            ctrl = new ProgressBar(80, 20, 100, 60) { Value = 60 };
            Content.Children.Add(ctrl);

            ctrl = new ProgressBar(200, 20, 50, 30, Orientation.Horizontal) { Value = 60, Foreground = new SolidColorBrush(Color.Blue) };
            Content.Children.Add(ctrl);

            ctrl = new ProgressBar(20, 100, 30, 50, Orientation.Vertical) { Value = 60 };
            Content.Children.Add(ctrl);

            ctrl = new ProgressBar(80, 100, 60, 30, Orientation.Horizontal) { Value = 60, Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue) { Opacity = 120 }, Foreground = new LinearGradientBrush(Color.Blue, Color.Red) { Opacity = 150 } };
            Content.Children.Add(ctrl);

            ctrl = new ProgressBar(80, 150, 100, 10, Orientation.Horizontal) { Value = 60, Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue), Foreground = new LinearGradientBrush(Color.Blue, Color.Red) };
            Content.Children.Add(ctrl);

            new Thread(delegate()
            {
                int v = 0;
                while (true)
                {
                    v += 5;
                    if (v > 100)
                        v = 0;

                    foreach (ProgressBar c in Content.Children)
                        c.Value = v;

                    Thread.Sleep(100);
                }
            }).Start();
        }
    }
}
