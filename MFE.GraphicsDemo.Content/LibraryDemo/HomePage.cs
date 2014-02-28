using System;
using Microsoft.SPOT;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using MFE.Graphics.Geometry;

namespace MFE.GraphicsDemo.Content.LibraryDemo
{
    class HomePage : Panel
    {
        private Point p;
        private Panel panelNavigation;
        private Panel header;

        private int margin = 5;
        private int x;
        private int y;
        private int w;
        private int h = 24;
        private int step;

        public HomePage()
            : base(0, 0, DemoManager.Desktop.Width, DemoManager.Desktop.Height)
        {
            panelNavigation = new Panel(5, 5, DemoManager.Desktop.Width - 10, DemoManager.Desktop.Height - 10)
            {
                Background = new LinearGradientBrush(Color.Bisque, Color.Black, 0, 0, 1000, 1000, 120),
                Border = new Pen(Color.Black, 1)
            };
            Children.Add(panelNavigation);

            header = new Panel(0, 0, panelNavigation.Width - 0, 25);
            header.Background = DemoManager.Bar;
            header.Children.Add(new Label(10, 5, DemoManager.FontCourierNew10, "Navigation") { ForeColor = Color.CornflowerBlue });
            header.TouchDown += delegate(object sender, TouchEventArgs e) { TouchCapture.Capture(header); p = e.Point; };
            header.TouchMove += delegate(object sender, TouchEventArgs e) { if (TouchCapture.Captured == header) { panelNavigation.Translate(e.Point.X - p.X, e.Point.Y - p.Y); p = e.Point; } };
            header.TouchUp += delegate(object sender, TouchEventArgs e) { if (TouchCapture.Captured == header) TouchCapture.ReleaseCapture(); };
            panelNavigation.Children.Add(header);

            x = margin;
            w = panelNavigation.Width / 2 - 2 * margin;
            y = header.Height + margin;
            step = h + margin;

            AddButton("Level");
            AddButton("ProgressBar");
            AddButton("Image");
            //AddButton("Panel");
            //AddButton("Checkbox");
            //AddButton("Button");
            //AddButton("MultiImage");
            //AddButton("Label");
            //AddButton("RadioButton");
            //AddButton("Slider");
            //AddButton("TextBlock");
            //AddButton("");
            //AddButton("");
            //AddButton("");
            //AddButton("");
            //AddButton("");
            //AddButton("");
        }
        private void AddButton(string title)
        {
            if ((y + step) > panelNavigation.Height)
            {
                y = header.Height + margin;
                x = panelNavigation.Width / 2 + margin;
            }

            Button btn = new Button(x, y, w, h, DemoManager.FontCourierNew10, title, Color.White) { BackgroundUnpressed = DemoManager.Bar };
            btn.Click += new EventHandler(btn_Click);
            panelNavigation.Children.Add(btn);
            y += step;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            DemoPage panelActiveDemo = null;
            switch ((sender as Button).Text)
            {
                //case "Panel": panelActiveDemo = new PanelDemo(); break;
                //case "Checkbox": panelActiveDemo = new CheckboxDemo(); break;
                case "Image": panelActiveDemo = new ImageDemoPage(); break;
                case "Level": panelActiveDemo = new LevelDemoPage(); break;
                //case "Slider": panelActiveDemo = new SliderDemo(); break;
                case "ProgressBar": panelActiveDemo = new ProgressBarDemoPage(); break;


            }

            if (panelActiveDemo != null)
            {
                //DemoManager.Desktop.Children.Add(panelActiveDemo);
                //DemoManager.Desktop.Invalidate();

                Children.Add(panelActiveDemo);
            }
        }
    }
}
