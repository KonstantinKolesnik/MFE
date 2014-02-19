using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using Microsoft.SPOT;
using System;
using System.Threading;

namespace MFE.GraphicsDemo
{
    public class Program
    {
        //private static GraphicsManager gm;

        public static void Main()
        {
            //gm = new GraphicsManager(320, 240);
            //gm = new GraphicsManager(800, 480);
            //gm.OnRender += delegate(Bitmap bitmap, Rect dirtyArea)
            //{
            //    display.SimpleGraphics.DisplayImage(bitmap, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.Width, (uint)dirtyArea.Height);
            //};

            //Demo1();
            new MainWindow().Demo22();

            Thread.Sleep(-1);
        }

        public static void Demo1()
        {
            GraphicsManager gm = new GraphicsManager(800, 480);

            Desktop desktop = gm.Desktop;

            int k = desktop.Height / 240;
            Font font = Resources.GetFont(Resources.FontResources.CourierNew_10);

            desktop.SuspendLayout();

            ImageBrush brush = new ImageBrush(GetBitmap(Resources.BinaryResources.Background_800_600, Bitmap.BitmapImageType.Jpeg));
            brush.Stretch = Stretch.Fill;
            desktop.Background = brush;

            int statusbarHeight = 24;
            Panel statusbar = new Panel(0, desktop.Height - statusbarHeight, desktop.Width, statusbarHeight);
            statusbar.Background = new ImageBrush(GetBitmap(Resources.BinaryResources.Bar, Bitmap.BitmapImageType.Bmp));
            desktop.Children.Add(statusbar);

            Label lblClock = new Label(statusbar.Width - 70, 4, font, "00:00:00");
            lblClock.ForeColor = Color.White;
            statusbar.Children.Add(lblClock);

            Level lvl2 = new Level(statusbar.Width - 120, 7, 40, 10, Orientation.Horizontal, 10);
            lvl2.Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Black);
            lvl2.Value = 50;
            statusbar.Children.Add(lvl2);

            statusbar.Children.Add(new Image(statusbar.Width - 160, 1, 23, 23, GetBitmap(Resources.BinaryResources.Drive, Bitmap.BitmapImageType.Gif)));
            statusbar.Children.Add(new Image(statusbar.Width - 185, 1, 23, 23, GetBitmap(Resources.BinaryResources.Mouse, Bitmap.BitmapImageType.Gif)));
            statusbar.Children.Add(new Image(statusbar.Width - 210, 1, 23, 23, GetBitmap(Resources.BinaryResources.Keyboard, Bitmap.BitmapImageType.Gif)));

            ////ToolButton btnHome = new ToolButton(10, 0, 70, statusbar.Height);
            //Button btnHome = new Button(10, 0, 70, statusbar.Height, null, "", Color.Black);
            //btnHome.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Home));
            //btnHome.Border = null;
            ////btnHome.Enabled = false;
            //statusbar.Children.Add(btnHome);

            //-------------------------------

            //desktop.Children.Add(new Checkbox(20*k, 20*k, 20*k, 20*k));
            desktop.Children.Add(new Checkbox(20, 20, 20, 20));


            //return;
            desktop.Children.Add(new TextBlock(50, 10, 100, 100, font, "Hello world! I'm a text block. I'm very cool!")
            {
                ForeColor = Color.White,
                Background = new LinearGradientBrush(Color.Aquamarine, Color.Yellow) { Opacity = 70 },
                TextAlignment = TextAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Top,
                TextWrap = true
            });


            //    Level lvl = new Level(20, 40, 60, 20, Orientation.Horizontal, 10);
            //    lvl.Foreground = new LinearGradientBrush(Color.Blue, Color.Black);
            //    //lvl.Value = 0;
            //    Children.Add(lvl);




            ProgressBar pg = new ProgressBar(20, 80, 100, 10);
            pg.Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Red);
            //pg.Foreground.Opacity = 220;
            desktop.Children.Add(pg);

            //    Panel pnl = new Panel(20, 100, 100, 100);
            //    pnl.Background = new LinearGradientBrush(Color.Blue, Color.LimeGreen);
            //    //pnl.Background.Opacity = 80;
            //    Children.Add(pnl);

            //    Button btn = new Button(20, 220, 80, 30, font, "<", Color.White);
            //    Children.Add(btn);

            //    Button btn2 = new Button(60, 0, 80, 25, font, "Button 2 wwwwwwww", Color.White)
            //    {
            //        //BackgroundUnpressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 100 };
            //        //BackgroundPressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 220 };
            //    };
            //    btn2.Click += delegate(object sender, EventArgs e)
            //    {
            //        wndModal dlg = new wndModal(0, 0, 0, 0);
            //        dlg.ShowModal();

            //        int a = 0;
            //        int b = a;

            //        //Close();
            //    };
            //    statusbar.Children.Add(btn2);

            //    RadioButtonGroup rbg = new RadioButtonGroup(20, 260, 25, 70);
            //    rbg.Background = new LinearGradientBrush(Color.White, Color.DarkGray);
            //    //rbg.Background.Opacity = 120;
            //    rbg.AddRadioButton(new RadioButton(5, 5, 15, true));
            //    rbg.AddRadioButton(new RadioButton(5, 25, 15));
            //    rbg.AddRadioButton(new RadioButton(5, 45, 15));
            //    Children.Add(rbg);


            //    ToolButton tbtn;

            //    tbtn = new ToolButton(300, 150, 128, 128);
            //    //tbtn.BackgroundUnpressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 100 };
            //    //tbtn.BackgroundPressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 220 };
            //    tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Database));
            //    tbtn.Foreground.Opacity = 200;
            //    Children.Add(tbtn);

            //    tbtn = new ToolButton(450, 150, 128, 128);
            //    tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Operation));
            //    tbtn.Foreground.Opacity = 200;
            //    Children.Add(tbtn);

            //    tbtn = new ToolButton(600, 150, 128, 128);
            //    tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Settings));
            //    tbtn.Foreground.Opacity = 200;
            //    Children.Add(tbtn);

            desktop.Children.Add(new Slider(250, 20, 150, 30, 15, Orientation.Horizontal)
            {
                Value = 80,
                Background = new ImageBrush(GetBitmap(Resources.BinaryResources.Bar, Bitmap.BitmapImageType.Bmp)),
                Foreground = new LinearGradientBrush(Color.LightGray, Color.Black) { Opacity = 50 }
            });
            desktop.Children.Add(new Slider(200, 20, 30, 150, 12, Orientation.Vertical)
            {
                Value = 70,
                Background = new SolidColorBrush(Color.White) { Opacity = 100 }
            });

            //    Slider slider = new Slider(250, 60, 150, 30, 30, Orientation.Horizontal)
            //    {
            //        Value = 80,
            //        Background = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Bar)),
            //        Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.GHILogo)) { Opacity = 200 },
            //    };
            //    Children.Add(slider);
            //    Label lbl = new Label(250, 100, font, slider.Value.ToString()) { ForeColor = Color.White };
            //    Children.Add(lbl);
            //    slider.ValueChanged += delegate(object sender, ValueChangedEventArgs e) { lbl.Text = e.Value.ToString(); };




            new Thread(delegate()
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
                    pg.Value = v;
                    lvl2.Value = v;

                    //Color temp = ((LinearGradientBrush)pnl.Background).StartColor;
                    //((LinearGradientBrush)pnl.Background).StartColor = ((LinearGradientBrush)pnl.Background).EndColor;
                    //((LinearGradientBrush)pnl.Background).EndColor = temp;
                    //pnl.Invalidate();

                    desktop.ResumeLayout();

                    Thread.Sleep(500);
                }
            }).Start();

            //wndModal wndModal = new wndModal();
            //wndModal.Show();

            desktop.ResumeLayout();
        }
        
        
        internal static Bitmap GetBitmap(Resources.BinaryResources id, Bitmap.BitmapImageType type)
        {
            return new Bitmap(Resources.GetBytes(id), type);
        }
    }
}
