using Gadgeteer.Modules.KKS;
using GHI.Premium.Hardware;
using GHI.Premium.System;
using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using Microsoft.SPOT;
using System;
using System.Threading;

namespace MFE.GraphicsDemo
{
    public partial class Program
    {
        private GraphicsManager gm;
        private Desktop desktop;
        private DisplayS22 display;

        private Font fontRegular;
        private Font fontCourierNew10;
        private Font fontTitle;
        private Point p;
        private Panel panelPresentation;
        private Panel tlb2;
        private Label lblActiveDemo;
        private Panel panelActiveDemo;

        void ProgramStarted()
        {
            fontRegular = Resources.GetFont(Resources.FontResources.LucidaSansUnicode_8);
            fontCourierNew10 = Resources.GetFont(Resources.FontResources.CourierNew_10);
            fontTitle = Resources.GetFont(Resources.FontResources.SegoeUI_BoldItalian_32);

            //Demo22SPI();
            Demo32();

            Mainboard.SetDebugLED(true);
        }

        private void Demo22SPI()
        {
            display = new DisplayS22(1);

            gm = new GraphicsManager(240, 320);
            gm.OnRender += delegate(Bitmap bitmap, Rect dirtyArea)
            {
                display.SimpleGraphics.DisplayImage(bitmap, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.Width, (uint)dirtyArea.Height);
            };
            desktop = gm.Desktop;


            desktop.SuspendLayout();

            ImageBrush brush = new ImageBrush(GetBitmap(Resources.BinaryResources.reWalls, Bitmap.BitmapImageType.Jpeg));
            brush.Stretch = Stretch.Fill;
            desktop.Background = brush;

            ImageBrush bar = new ImageBrush(GetBitmap(Resources.BinaryResources.Bar, Bitmap.BitmapImageType.Bmp));

            int statusbarHeight = 24;
            Panel statusbar = new Panel(0, desktop.Height - statusbarHeight, desktop.Width, statusbarHeight);
            statusbar.Background = bar;
            desktop.Children.Add(statusbar);

            Label lblClock = new Label(statusbar.Width - 50, 4, fontRegular, "00:00:00");
            lblClock.ForeColor = Color.White;
            statusbar.Children.Add(lblClock);

            Level lvl2 = new Level(statusbar.Width - 100, 7, 40, 10, Orientation.Horizontal, 10)
            {
                Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Black),
                Value = 0
            };
            statusbar.Children.Add(lvl2);

            Image img;
            int size = 100;
            Bitmap bmp = GetBitmap(Resources.BinaryResources.Operation, Bitmap.BitmapImageType.Gif);
            //img = new Image(20, 10, size, size, bmp);
            //desktop.Children.Add(img);
            //img = new Image(150, 10, size + 100, size, bmp);
            //desktop.Children.Add(img);
            //img = new Image(20, 150, size, size, bmp) { Border = new Pen(Color.Red, 2) };
            //desktop.Children.Add(img);
            //img = new Image(150, 150, size, size, bmp) { Opacity = 80 };
            //desktop.Children.Add(img);
            img = new Image(20, 20, size, size, bmp)
            {
                Opacity = 210,
                Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue) { Opacity = 180 }
            };
            desktop.Children.Add(img);

            Slider sl = new Slider(160, 20, 30, 100, 12, Orientation.Vertical)
            {
                Value = 0,
                //Background = new SolidColorBrush(Color.CornflowerBlue) { Opacity = 100 }
                Background = bar,
            };
            desktop.Children.Add(sl);

            Level lvl = new Level(20, 150, 30, 60, Orientation.Vertical, 10)
            {
                Value = 0,
                Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue) { Opacity = 180 },
                Foreground = new LinearGradientBrush(Color.Blue, Color.Red) { Opacity = 210 }
            };
            desktop.Children.Add(lvl);

            desktop.Children.Add(new Button(120, 170, 90, 24, fontCourierNew10, "Click me", Color.White) { BackgroundUnpressed = bar });




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

                    lvl.Value = v;
                    //pg.Value = v;
                    lvl2.Value = v;
                    sl.Value = v;

                    Color temp = ((LinearGradientBrush)img.Background).StartColor;
                    ((LinearGradientBrush)img.Background).StartColor = ((LinearGradientBrush)img.Background).EndColor;
                    ((LinearGradientBrush)img.Background).EndColor = temp;

                    desktop.ResumeLayout();

                    Thread.Sleep(400);
                }
            }).Start();
        }
        private void Demo32()
        {
            //display_TE35
            //if (SystemMetrics.ScreenHeight == 0 || SystemMetrics.ScreenWidth == 0)
            //    ResetDisplay();

            Configuration.LCD.EnableLCDBootupMessages(false);

            gm = new GraphicsManager(320, 240);

            desktop = gm.Desktop;


            desktop.SuspendLayout();

            ImageBrush brush = new ImageBrush(GetBitmap(Resources.BinaryResources.reWalls, Bitmap.BitmapImageType.Jpeg));
            brush.Stretch = Stretch.Fill;
            desktop.Background = brush;

            ImageBrush bar = new ImageBrush(GetBitmap(Resources.BinaryResources.Bar, Bitmap.BitmapImageType.Bmp));

            int statusbarHeight = 24;
            Panel statusbar = new Panel(0, desktop.Height - statusbarHeight, desktop.Width, statusbarHeight);
            statusbar.Background = bar;
            desktop.Children.Add(statusbar);

            Label lblClock = new Label(statusbar.Width - 50, 4, fontRegular, "00:00:00");
            lblClock.ForeColor = Color.White;
            statusbar.Children.Add(lblClock);

            Level lvl2 = new Level(statusbar.Width - 100, 7, 40, 10, Orientation.Horizontal, 10)
            {
                Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Black),
                Value = 0
            };
            statusbar.Children.Add(lvl2);

            Image img;
            int size = 100;
            Bitmap bmp = GetBitmap(Resources.BinaryResources.Operation, Bitmap.BitmapImageType.Gif);
            //img = new Image(20, 10, size, size, bmp);
            //desktop.Children.Add(img);
            //img = new Image(150, 10, size + 100, size, bmp);
            //desktop.Children.Add(img);
            //img = new Image(20, 150, size, size, bmp) { Border = new Pen(Color.Red, 2) };
            //desktop.Children.Add(img);
            //img = new Image(150, 150, size, size, bmp) { Opacity = 80 };
            //desktop.Children.Add(img);
            img = new Image(20, 20, size, size, bmp)
            {
                Opacity = 210,
                Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue) { Opacity = 180 }
            };
            desktop.Children.Add(img);

            Slider sl = new Slider(160, 20, 30, 100, 12, Orientation.Vertical)
            {
                Value = 0,
                //Background = new SolidColorBrush(Color.CornflowerBlue) { Opacity = 100 }
                Background = bar,
            };
            desktop.Children.Add(sl);

            Level lvl = new Level(20, 150, 30, 60, Orientation.Vertical, 10)
            {
                Value = 0,
                Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue) { Opacity = 180 },
                Foreground = new LinearGradientBrush(Color.Blue, Color.Red) { Opacity = 210 }
            };
            desktop.Children.Add(lvl);

            desktop.Children.Add(new Button(120, 170, 90, 24, fontCourierNew10, "Click me", Color.White) { BackgroundUnpressed = bar });




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

                    lvl.Value = v;
                    //pg.Value = v;
                    lvl2.Value = v;
                    sl.Value = v;

                    Color temp = ((LinearGradientBrush)img.Background).StartColor;
                    ((LinearGradientBrush)img.Background).StartColor = ((LinearGradientBrush)img.Background).EndColor;
                    ((LinearGradientBrush)img.Background).EndColor = temp;

                    desktop.ResumeLayout();

                    //Thread.Sleep(200);
                }
            }).Start();
        }


        private Bitmap GetBitmap(Resources.BinaryResources id, Bitmap.BitmapImageType type)
        {
            return new Bitmap(Resources.GetBytes(id), type);
        }
        private void ResetDisplay()
        {
            Debug.Print("Display not configured. Configuring mainboard to use display.");
            Debug.Print("Mainboard will need to be rebooted when finished");

            Configuration.LCD.Configurations lcdConfig = new Configuration.LCD.Configurations();

            lcdConfig.Width = 320;
            lcdConfig.Height = 240;

            // Only use if needed, see documentation.
            //lcdConfig.PriorityEnable = false;

            lcdConfig.OutputEnableIsFixed = true;
            lcdConfig.OutputEnablePolarity = true;

            lcdConfig.HorizontalSyncPolarity = false;
            lcdConfig.VerticalSyncPolarity = false;
            lcdConfig.PixelPolarity = true;

            lcdConfig.HorizontalSyncPulseWidth = 41;
            lcdConfig.HorizontalBackPorch = 27;
            lcdConfig.HorizontalFrontPorch = 51;
            lcdConfig.VerticalSyncPulseWidth = 10;
            lcdConfig.VerticalBackPorch = 8;
            lcdConfig.VerticalFrontPorch = 16;

            lcdConfig.PixelClockRateKHz = 10;

            bool bNeedReset = Configuration.LCD.Set(lcdConfig);

            Debug.Print("Mainboard can now be reset");

            if (bNeedReset)
            {
                Thread.Sleep(10000);
                Util.FlushExtendedWeakReferences();
                Microsoft.SPOT.Hardware.PowerState.RebootDevice(false);
            }
        }
    }
}
