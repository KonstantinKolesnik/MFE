using System;
using Microsoft.SPOT;
using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using MFE.GraphicsDemo.Demos;
using MFE.Graphics;
using Gadgeteer.Modules.KKSolutions;
using System.Threading;

namespace MFE.GraphicsDemo
{
    class MainWindow
    {
        private GraphicsManager gm;
        private Desktop desktop;
        private Display_SP22 display;

        private Font fontRegular;
        private Font fontCourierNew10;
        private Font fontTitle;
        private Point p;
        private Panel panelPresentation;
        private Panel tlb2;
        private Label lblActiveDemo;
        private Panel panelActiveDemo;

        public MainWindow()
        {
            fontRegular = Resources.GetFont(Resources.FontResources.LucidaSansUnicode_8);
            fontCourierNew10 = Resources.GetFont(Resources.FontResources.CourierNew_10);
            fontTitle = Resources.GetFont(Resources.FontResources.SegoeUI_BoldItalian_32);
        }

        public void Demo()
        {
            gm = new GraphicsManager(800, 480);
            desktop = gm.Desktop;


            desktop.SuspendLayout();

            desktop.Background = new ImageBrush(Program.GetBitmap(Resources.BinaryResources.reWalls, Bitmap.BitmapImageType.Jpeg)) { Stretch = Stretch.Fill };

            ImageBrush bar = new ImageBrush(Program.GetBitmap(Resources.BinaryResources.Bar, Bitmap.BitmapImageType.Bmp));

            // left panel
            Panel panelNavigation = new Panel(10, 10, 150, desktop.Height - 20) //460
            {
                Background = new LinearGradientBrush(Color.Bisque, Color.Black, 0, 500, 1000, 500, 120),
                Border = new Pen(Color.Black, 1)
            };
            desktop.Children.Add(panelNavigation);

            Panel tlb = new Panel(1, 1, panelNavigation.Width - 2, 25);
            tlb.Background = bar;
            tlb.Children.Add(new Label(10, 5, fontCourierNew10, "Navigation") { ForeColor = Color.CornflowerBlue });
            //tlb.TouchDown += delegate(object sender, TouchEventArgs e) { TouchCapture.Capture(tlb); p = e.Point; };
            //tlb.TouchMove += delegate(object sender, TouchEventArgs e) { if (TouchCapture.Captured == tlb) { panelNavigation.Translate(e.Point.X - p.X, e.Point.Y - p.Y); p = e.Point; } };
            //tlb.TouchUp += delegate(object sender, TouchEventArgs e) { if (TouchCapture.Captured == tlb) TouchCapture.ReleaseCapture(); };
            panelNavigation.Children.Add(tlb);

            //int d = 15;
            //int x = 0;
            ////int y = 5;
            //RadioButtonGroup rbgNavigation = new RadioButtonGroup(10, 30, 15, panelNavigation.Height - 40);
            //panelNavigation.Children.Add(rbgNavigation);
            //rbgNavigation.AddRadioButton(new RadioButton(x, 5, d, true));
            //rbgNavigation.AddRadioButton(new RadioButton(x, 25, d));
            //rbgNavigation.AddRadioButton(new RadioButton(x, 45, d));
            //rbgNavigation.AddRadioButton(new RadioButton(x, 65, d));
            //rbgNavigation.AddRadioButton(new RadioButton(x, 85, d));
            //rbgNavigation.AddRadioButton(new RadioButton(x, 105, d));

            //panelNavigation.Children.Add(new Label(30, 35, fontCourierNew10, "Checkboxes") { ForeColor = Color.White });
            //panelNavigation.Children.Add(new Label(30, 55, fontCourierNew10, "Buttons") { ForeColor = Color.White });

            int x = 5;
            int w = panelNavigation.Width - 2 * x;
            int h = 18;
            int y = 35;
            int step = h + 5;

            //Font fnt = fontRegular;
            Font fnt = fontCourierNew10;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Checkbox", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Button", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Image", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Label", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Level", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "MultiImage", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Panel", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "ProgressBar", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "RadioButton", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "Slider", Color.White) { BackgroundUnpressed = bar }); y += step;
            panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "TextBlock", Color.White) { BackgroundUnpressed = bar }); y += step;
            //panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "", Color.White) { BackgroundUnpressed = bar }); y += step;
            //panelNavigation.Children.Add(new Button(x, y, w, h, fnt, "", Color.White) { BackgroundUnpressed = bar }); y += step;
            foreach (Control ctrl in panelNavigation.Children)
            {
                if (ctrl is Button)
                    (ctrl as Button).Click += new EventHandler(Button_Click);
            }

            // title
            TextBlock txtTitle = new TextBlock(panelNavigation.Right + 10, panelNavigation.Y, desktop.Width - (panelNavigation.Right + 20), 60, fontTitle, "MFE graphics")
            {
                ForeColor = Color.CornflowerBlue,
                TextAlignment = TextAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Center
            };
            desktop.Children.Add(txtTitle);

            // right panel
            int yy = txtTitle.Height + 10;
            panelPresentation = new Panel(txtTitle.X, panelNavigation.Y + yy, txtTitle.Width, panelNavigation.Height - yy);
            panelPresentation.Background = new SolidColorBrush(Color.Black);
            panelPresentation.Background.Opacity = 50;
            panelPresentation.Border = new Pen(Color.Black, 1);
            desktop.Children.Add(panelPresentation);

            tlb2 = new Panel(1, 1, panelPresentation.Width - 2, 25);
            tlb2.Background = bar;
            tlb2.Children.Add(lblActiveDemo = new Label(10, 5, fontCourierNew10, "Presentation: [Please select control type on the left]") { ForeColor = Color.CornflowerBlue });
            //tlb2.TouchDown += delegate(object sender, TouchEventArgs e) { TouchCapture.Capture(tlb2); p = e.Point; };
            //tlb2.TouchMove += delegate(object sender, TouchEventArgs e) { if (TouchCapture.Captured == tlb2) { panelPresentation.Translate(e.Point.X - p.X, e.Point.Y - p.Y); p = e.Point; } };
            //tlb2.TouchUp += delegate(object sender, TouchEventArgs e) { if (TouchCapture.Captured == tlb2) TouchCapture.ReleaseCapture(); };
            panelPresentation.Children.Add(tlb2);

            desktop.ResumeLayout();
        }
        private void Button_Click(object sender, EventArgs e)
        {
            panelPresentation.SuspendLayout();

            lblActiveDemo.Text = "Presentation: " + (sender as Button).Text;

            if (panelActiveDemo != null)
            {
                panelPresentation.Children.Remove(panelActiveDemo);
                panelActiveDemo = null;
            }

            switch ((sender as Button).Text)
            {
                case "Checkbox": panelActiveDemo = new CheckboxDemo(); break;
                case "Image": panelActiveDemo = new ImageDemo(); break;
                case "Level": panelActiveDemo = new LevelDemo(); break;
                case "Panel": panelActiveDemo = new PanelDemo(); break;
                case "Slider": panelActiveDemo = new SliderDemo(); break;



            }

            if (panelActiveDemo != null)
            {
                panelActiveDemo.X = 1;
                panelActiveDemo.Y = tlb2.Height + 1;
                panelActiveDemo.Width = panelPresentation.Width - 2;
                panelActiveDemo.Height = panelPresentation.Height - tlb2.Height - 2;
                panelPresentation.Children.Add(panelActiveDemo);
            }

            panelPresentation.ResumeLayout();
        }

        public void Demo22()
        {
            display = new Display_SP22(1);

            gm = new GraphicsManager(240, 320);
            gm.OnRender += delegate(Bitmap bitmap, Rect dirtyArea)
            {
                display.SimpleGraphics.DisplayImage(bitmap, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.Width, (uint)dirtyArea.Height);
            };

            desktop.SuspendLayout();

            ImageBrush brush = new ImageBrush(Program.GetBitmap(Resources.BinaryResources.reWalls, Bitmap.BitmapImageType.Jpeg));
            brush.Stretch = Stretch.Fill;
            //desktop.Background = brush;

            int statusbarHeight = 24;
            Panel statusbar = new Panel(0, desktop.Height - statusbarHeight, desktop.Width, statusbarHeight);
            statusbar.Background = new ImageBrush(new Bitmap(Resources.GetBytes(Resources.BinaryResources.Bar), Bitmap.BitmapImageType.Bmp));
            desktop.Children.Add(statusbar);

            Label lblClock = new Label(statusbar.Width - 70, 4, fontRegular, "00:00:00");
            lblClock.ForeColor = Color.White;
            statusbar.Children.Add(lblClock);

            Level lvl2 = new Level(statusbar.Width - 120, 7, 40, 10, Orientation.Horizontal, 10);
            lvl2.Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Black);
            lvl2.Value = 50;
            statusbar.Children.Add(lvl2);



            desktop.ResumeLayout();



            new Thread(() =>
            {
                int v = 0;
                while (true)
                {
                    desktop.SuspendLayout();

                    DateTime dt = DateTime.Now;

                    string hour = (dt.Hour < 10) ? "0" + dt.Hour.ToString() : dt.Hour.ToString();
                    string minute = (dt.Minute < 10) ? "0" + dt.Minute.ToString() : dt.Minute.ToString();
                    string second = (dt.Second < 10) ? "0" + dt.Second.ToString() : dt.Second.ToString();
                    string result = hour + ":" + minute + ":" + second;
                    lblClock.Text = result;

                    v += 10;
                    if (v > 100)
                        v = 0;

                    //lvl.Value = v;
                    //pg.Value = v;
                    lvl2.Value = v;

                    //Color temp = ((LinearGradientBrush)pnl.Background).StartColor;
                    //((LinearGradientBrush)pnl.Background).StartColor = ((LinearGradientBrush)pnl.Background).EndColor;
                    //((LinearGradientBrush)pnl.Background).EndColor = temp;
                    //pnl.Invalidate();

                    desktop.ResumeLayout();

                    Thread.Sleep(500);
                }
            }).Start();



        }
    }
}
