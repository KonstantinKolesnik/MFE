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
        private Panel panelNavigation;
        Point p;

        public HomePage()
            : base(0, 0, DemoManager.Desktop.Width, DemoManager.Desktop.Height)
        {
            panelNavigation = new Panel(5, 5, DemoManager.Desktop.Width - 10, DemoManager.Desktop.Height - 10)
            {
                Background = new LinearGradientBrush(Color.Bisque, Color.Black, 0, 0, 1000, 1000, 120),
                //Background = new SolidColorBrush(Color.CornflowerBlue) { Opacity = 120 },
                Border = new Pen(Color.Black, 1)
            };
            Children.Add(panelNavigation);

            Panel tlb = new Panel(0, 0, panelNavigation.Width - 0, 25);
            tlb.Background = DemoManager.Bar;
            tlb.Children.Add(new Label(10, 5, DemoManager.FontCourierNew10, "Navigation") { ForeColor = Color.CornflowerBlue });
            tlb.TouchDown += delegate(object sender, TouchEventArgs e) { TouchCapture.Capture(tlb); p = e.Point; };
            tlb.TouchMove += delegate(object sender, TouchEventArgs e) { if (TouchCapture.Captured == tlb) { panelNavigation.Translate(e.Point.X - p.X, e.Point.Y - p.Y); p = e.Point; } };
            tlb.TouchUp += delegate(object sender, TouchEventArgs e) { if (TouchCapture.Captured == tlb) TouchCapture.ReleaseCapture(); };
            panelNavigation.Children.Add(tlb);

            int x = 5;
            int w = panelNavigation.Width - 2 * x;
            int h = 18;
            int y = 35;
            int step = h + 5;

            Font fnt = DemoManager.FontCourierNew10;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Checkbox", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Button", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Image", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Label", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Level", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "MultiImage", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Panel", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "ProgressBar", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "RadioButton", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Slider", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "TextBlock", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            //panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            //panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "", Color.White) { BackgroundUnpressed = DemoManager.Bar }); y += step;
            
            foreach (Control ctrl in panelNavigation.Children)
                if (ctrl is Button)
                    (ctrl as Button).Click += new EventHandler(Button_Click);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            //panelPresentation.SuspendLayout();

            //lblActiveDemo.Text = "Presentation: " + (sender as Button).Text;

            //if (panelActiveDemo != null)
            //{
            //    panelPresentation.Children.Remove(panelActiveDemo);
            //    panelActiveDemo = null;
            //}

            //switch ((sender as Button).Text)
            //{
            //    case "Checkbox": panelActiveDemo = new CheckboxDemo(); break;
            //    case "Image": panelActiveDemo = new ImageDemo(); break;
            //    case "Level": panelActiveDemo = new LevelDemo(); break;
            //    case "Panel": panelActiveDemo = new PanelDemo(); break;
            //    case "Slider": panelActiveDemo = new SliderDemo(); break;



            //}

            //if (panelActiveDemo != null)
            //{
            //    panelActiveDemo.X = 1;
            //    panelActiveDemo.Y = tlb2.Height + 1;
            //    panelActiveDemo.Width = panelPresentation.Width - 2;
            //    panelActiveDemo.Height = panelPresentation.Height - tlb2.Height - 2;
            //    panelPresentation.Children.Add(panelActiveDemo);
            //}

            //panelPresentation.ResumeLayout();
        }
    }
}
