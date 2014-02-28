using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using System.Threading;

namespace MFE.GraphicsDemo.Content.LibraryDemo
{
    class LevelDemoPage : DemoPage
    {
        public LevelDemoPage()
        {
            Level lvl;

            lvl = new Level(20, 20, 50, 30) { Value = 60 };
            Content.Children.Add(lvl);

            lvl = new Level(80, 20, 100, 60) { Value = 60 };
            Content.Children.Add(lvl);

            lvl = new Level(200, 20, 50, 30, Orientation.Horizontal, 10) { Value = 60 };
            Content.Children.Add(lvl);

            lvl = new Level(20, 100, 30, 50, Orientation.Vertical, 10) { Value = 60 };
            Content.Children.Add(lvl);

            lvl = new Level(80, 100, 60, 30, Orientation.Horizontal, 10) { Value = 60, Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue) { Opacity = 120 }, Foreground = new LinearGradientBrush(Color.Blue, Color.Red) { Opacity = 150 } };
            Content.Children.Add(lvl);

            lvl = new Level(80, 150, 100, 30, Orientation.Horizontal, 30) { Value = 60, Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue), Foreground = new LinearGradientBrush(Color.Blue, Color.Red) };
            Content.Children.Add(lvl);

            new Thread(delegate()
            {
                int v = 0;
                while (true)
                {
                    v += 5;
                    if (v > 100)
                        v = 0;

                    foreach (Level l in Content.Children)
                        l.Value = v;

                    Thread.Sleep(100);
                }
            }).Start();
        }
    }
}
