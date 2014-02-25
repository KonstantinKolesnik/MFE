using System;
using Microsoft.SPOT;
using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using System.Threading;
using System.IO;
using GHI.Premium.IO;

namespace MFE.GraphicsDemo.Content
{
    public class Demo
    {
        private static GraphicsManager gm;
        public static Desktop desktop;

        public static Font fontRegular;
        public static Font fontCourierNew10;
        public static Font fontTitle;
        //private Point p;
        //private Panel panelPresentation;
        //private Panel tlb2;
        //private Label lblActiveDemo;
        //private Panel panelActiveDemo;

        public Demo(int width, int height, RenderRequestEventHandler renderHandler = null)
        {
            fontRegular = Resources.GetFont(Resources.FontResources.LucidaSansUnicode_8);
            fontCourierNew10 = Resources.GetFont(Resources.FontResources.CourierNew_10);
            fontTitle = Resources.GetFont(Resources.FontResources.SegoeUI_BoldItalian_32);

            gm = new GraphicsManager(width, height);
            desktop = gm.Desktop;
            if (renderHandler != null)
                gm.OnRenderRequest += renderHandler;
        }

        public void QuickDemo()
        {
            //CheckCalibration();

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

            desktop.Children.Add(new Button(120, 170, 90, 24, fontRegular, "Click me", Color.White) { BackgroundUnpressed = bar });

            desktop.ResumeLayout();



            new Thread(() =>
            {
                int v = 0;
                string hour;
                string minute;
                string second;

                while (true)
                {
                    desktop.SuspendLayout();

                    //DateTime dt = RealTimeClock.GetTime();
                    DateTime dt = DateTime.Now;

                    hour = (dt.Hour < 10) ? "0" + dt.Hour.ToString() : dt.Hour.ToString();
                    minute = (dt.Minute < 10) ? "0" + dt.Minute.ToString() : dt.Minute.ToString();
                    second = (dt.Second < 10) ? "0" + dt.Second.ToString() : dt.Second.ToString();
                    lblClock.Text = hour + ":" + minute + ":" + second;

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
        public void VideoDemo()
        {
            //int bufferLength = 720 * 1280;

            //using (Microsoft.SPOT.Hardware.LargeBuffer lb = new Microsoft.SPOT.Hardware.LargeBuffer(bufferLength))
            //{
            //    // use the buffer
            //    lb.Bytes[5] = 123;
            //    // ...        
            //}

            //long readMs, readMsAvg = 0;

            //// Create front and back buffers to use for the LCD controller.
            ////Configuration.Heap.SetCustomHeapSize(1024 * 1024 * 4);
            //byte[] frontBuffer = new byte[bufferLength];
            //byte[] backBuffer = new byte[bufferLength];
            //// Create a boolean to use to know which buffer to use next.
            //bool activeBuffer = true;
            //// Remember how many frames we have played.
            //int frames = 0;

            //PersistentStorage sdCard = new PersistentStorage("SD");
            //sdCard.MountFileSystem();

            //// Open the file containing the image array.
            //Stream stream = new FileStream(@"\SD\mfe.wmv", FileMode.Open, FileAccess.Read, FileShare.None);


            //// Read through stream.
            //while (stream.Position < stream.Length)
            //{
            //    readMs = DateTime.Now.Ticks;
            //    // Read the image file bgr565le data into the next screen buffer.
            //    stream.Read(activeBuffer ? frontBuffer : backBuffer, 0, bufferLength);
            //    readMsAvg += (DateTime.Now.Ticks - readMs) / TimeSpan.TicksPerMillisecond;

            //    // Change the active screen buffer to display this frame.
            //    //LcdController.SetUpperFrameBuffer(activeBuffer ? frontBuffer : backBuffer);

            //    Bitmap bmp = new Bitmap(activeBuffer ? frontBuffer : backBuffer, Bitmap.BitmapImageType.Bmp);
            //    desktop.Background = new ImageBrush(bmp);



            //    // Swap the next active buffer for the next image.
            //    activeBuffer = !activeBuffer;

            //    frames++;
            //}

            //readMsAvg /= frames;
            //Debug.Print("Average Read Speed of 255Kb: " + (int)(readMsAvg) + "ms");
            //Debug.Print("Works out at: " + (((1000.0f / (float)readMsAvg) * 255f) / 1024f).ToString() + "MB/s");
        }
        public void CrashDemo()
        {
            Panel pnl = new Panel(0, 0, 150, 150);
            //Panel pnl = new Panel(0, 0, 50, 50);
            pnl.Background = new SolidColorBrush(Color.CornflowerBlue);

            int translateValue = 3;

            desktop.SuspendLayout();
            for (int i = 0; i < 10; i++)
                pnl.Children.Add(new TextBlock(10, 15 * i, 140, 20, fontRegular, "label" + i.ToString()) { ForeColor = Color.Brown });
            desktop.Children.Add(pnl);
            desktop.ResumeLayout();

            new Thread(delegate()
            {
                //int n = 0;
                while (true)
                {
                    desktop.SuspendLayout();

                    DateTime dt = DateTime.Now;
                    int c = pnl.Children.Count;
                    for (int j = 0; j < c; j++)
                    {
                        //dt0 = DateTime.Now;

                        //Label lbl = (Label)Children.GetAt(j);
                        TextBlock lbl = (TextBlock)pnl.Children[j];
                        //Debug.Print("Label text set");
                        lbl.Text = dt.Ticks.ToString();
                        //Thread.Sleep(10);

                        //ts = DateTime.Now - dt0;
                        //n++;
                        //sum += ts;

                        //Debug.Print("MFE: " + ts.ToString());
                        //Debug.Print("MFE (average): " + new TimeSpan(sum.Ticks / n).ToString());
                    }

                    pnl.Translate(translateValue, translateValue);
                    if (pnl.X + pnl.Width >= desktop.Width - 1)
                        pnl.X = 0;
                    if (pnl.Y + pnl.Height >= desktop.Height - 1)
                        pnl.Y = 0;

                    desktop.ResumeLayout();
                }
            }
            ).Start();

            //new Thread(delegate()
            //{
            //    while (true)
            //    {
            //        pnl.Translate(1, 1);

            //        if (pnl.X + pnl.Width >= desktop.Width - 1)
            //            pnl.X = 0;
            //        if (pnl.Y + pnl.Height >= desktop.Height - 1)
            //            pnl.Y = 0;

            //        //Thread.Sleep(100);
            //    }
            //}).Start();
        }
        public void DisplayDemo()
        {
            //GT.Modules.Module.DisplayModule.SimpleGraphicsInterface graphics = display.SimpleGraphics;
            //int buf[318];
            //int x, x2;
            //int y, y2;
            //int r;

            // Clear the screen and draw the frame
            //graphics.Clear();

            //graphics.BackgroundColor = GT.Color.Red;
            //  graphics.fillRect(0, 0, 319, 13);
            //  graphics.setColor(64, 64, 64);
            //  graphics.fillRect(0, 226, 319, 239);
            //  graphics.setColor(255, 255, 255);
            //  graphics.setBackColor(255, 0, 0);
            //  graphics.print("* Universal Color TFT Display Library *", CENTER, 1);
            //  graphics.setBackColor(64, 64, 64);
            //  graphics.setColor(255,255,0);
            //  graphics.print("<http://electronics.henningkarlsen.com>", CENTER, 227);

            //  graphics.setColor(0, 0, 255);
            //  graphics.drawRect(0, 14, 319, 225);

            //// Draw crosshairs
            //  graphics.setColor(0, 0, 255);
            //graphics.BackgroundColor = GT.Color.Black;
            //  graphics.drawLine(159, 15, 159, 224);
            //  graphics.drawLine(1, 119, 318, 119);
            //  for (int i=9; i<310; i+=10)
            //    graphics.drawLine(i, 117, i, 121);
            //  for (int i=19; i<220; i+=10)
            //    graphics.drawLine(157, i, 161, i);

            //// Draw sin-, cos- and tan-lines  
            //  graphics.setColor(0,255,255);
            //  graphics.print("Sin", 5, 15);
            //  for (int i=1; i<318; i++)
            //  {
            //    graphics.drawPixel(i,119+(sin(((i*1.13)*3.14)/180)*95));
            //  }

            //  graphics.setColor(255,0,0);
            //  graphics.print("Cos", 5, 27);
            //  for (int i=1; i<318; i++)
            //  {
            //    graphics.drawPixel(i,119+(cos(((i*1.13)*3.14)/180)*95));
            //  }

            //  graphics.setColor(255,255,0);
            //  graphics.print("Tan", 5, 39);
            //  for (int i=1; i<318; i++)
            //  {
            //    graphics.drawPixel(i,119+(tan(((i*1.13)*3.14)/180)));
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);
            //  graphics.setColor(0, 0, 255);
            //  graphics.setBackColor(0, 0, 0);
            //  graphics.drawLine(159, 15, 159, 224);
            //  graphics.drawLine(1, 119, 318, 119);

            //// Draw a moving sinewave
            //  x=1;
            //  for (int i=1; i<(318*20); i++) 
            //  {
            //    x++;
            //    if (x==319)
            //      x=1;
            //    if (i>319)
            //    {
            //      if ((x==159)||(buf[x-1]==119))
            //        graphics.setColor(0,0,255);
            //      else
            //        graphics.setColor(0,0,0);
            //      graphics.drawPixel(x,buf[x-1]);
            //    }
            //    graphics.setColor(0,255,255);
            //    y=119+(sin(((i*1.1)*3.14)/180)*(90-(i / 100)));
            //    graphics.drawPixel(x,y);
            //    buf[x-1]=y;
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //// Draw some filled rectangles
            //  for (int i=1; i<6; i++)
            //  {
            //    switch (i)
            //    {
            //      case 1:
            //        graphics.setColor(255,0,255);
            //        break;
            //      case 2:
            //        graphics.setColor(255,0,0);
            //        break;
            //      case 3:
            //        graphics.setColor(0,255,0);
            //        break;
            //      case 4:
            //        graphics.setColor(0,0,255);
            //        break;
            //      case 5:
            //        graphics.setColor(255,255,0);
            //        break;
            //    }
            //    graphics.fillRect(70+(i*20), 30+(i*20), 130+(i*20), 90+(i*20));
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //// Draw some filled, rounded rectangles
            //  for (int i=1; i<6; i++)
            //  {
            //    switch (i)
            //    {
            //      case 1:
            //        graphics.setColor(255,0,255);
            //        break;
            //      case 2:
            //        graphics.setColor(255,0,0);
            //        break;
            //      case 3:
            //        graphics.setColor(0,255,0);
            //        break;
            //      case 4:
            //        graphics.setColor(0,0,255);
            //        break;
            //      case 5:
            //        graphics.setColor(255,255,0);
            //        break;
            //    }
            //    graphics.fillRoundRect(190-(i*20), 30+(i*20), 250-(i*20), 90+(i*20));
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //// Draw some filled circles
            //  for (int i=1; i<6; i++)
            //  {
            //    switch (i)
            //    {
            //      case 1:
            //        graphics.setColor(255,0,255);
            //        break;
            //      case 2:
            //        graphics.setColor(255,0,0);
            //        break;
            //      case 3:
            //        graphics.setColor(0,255,0);
            //        break;
            //      case 4:
            //        graphics.setColor(0,0,255);
            //        break;
            //      case 5:
            //        graphics.setColor(255,255,0);
            //        break;
            //    }
            //    graphics.fillCircle(100+(i*20),60+(i*20), 30);
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //// Draw some lines in a pattern
            //  graphics.setColor (255,0,0);
            //  for (int i=15; i<224; i+=5)
            //  {
            //    graphics.drawLine(1, i, (i*1.44)-10, 224);
            //  }
            //  graphics.setColor (255,0,0);
            //  for (int i=224; i>15; i-=5)
            //  {
            //    graphics.drawLine(318, i, (i*1.44)-11, 15);
            //  }
            //  graphics.setColor (0,255,255);
            //  for (int i=224; i>15; i-=5)
            //  {
            //    graphics.drawLine(1, i, 331-(i*1.44), 15);
            //  }
            //  graphics.setColor (0,255,255);
            //  for (int i=15; i<224; i+=5)
            //  {
            //    graphics.drawLine(318, i, 330-(i*1.44), 224);
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //// Draw some random circles
            //  for (int i=0; i<100; i++)
            //  {
            //    graphics.setColor(random(255), random(255), random(255));
            //    x=32+random(256);
            //    y=45+random(146);
            //    r=random(30);
            //    graphics.drawCircle(x, y, r);
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //// Draw some random rectangles
            //  for (int i=0; i<100; i++)
            //  {
            //    graphics.setColor(random(255), random(255), random(255));
            //    x=2+random(316);
            //    y=16+random(207);
            //    x2=2+random(316);
            //    y2=16+random(207);
            //    graphics.drawRect(x, y, x2, y2);
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //// Draw some random rounded rectangles
            //  for (int i=0; i<100; i++)
            //  {
            //    graphics.setColor(random(255), random(255), random(255));
            //    x=2+random(316);
            //    y=16+random(207);
            //    x2=2+random(316);
            //    y2=16+random(207);
            //    graphics.drawRoundRect(x, y, x2, y2);
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //  for (int i=0; i<100; i++)
            //  {
            //    graphics.setColor(random(255), random(255), random(255));
            //    x=2+random(316);
            //    y=16+random(209);
            //    x2=2+random(316);
            //    y2=16+random(209);
            //    graphics.drawLine(x, y, x2, y2);
            //  }

            Thread.Sleep(2000);

            //  graphics.setColor(0,0,0);
            //  graphics.fillRect(1,15,318,224);

            //  for (int i=0; i<10000; i++)
            //  {
            //    graphics.setColor(random(255), random(255), random(255));
            //    graphics.drawPixel(2+random(316), 16+random(209));
            //  }

            Thread.Sleep(2000);

            //  graphics.fillScr(0, 0, 255);
            //  graphics.setColor(255, 0, 0);
            //  graphics.fillRoundRect(80, 70, 239, 169);

            //  graphics.setColor(255, 255, 255);
            //  graphics.setBackColor(255, 0, 0);
            //  graphics.print("That's it!", CENTER, 93);
            //  graphics.print("Restarting in a", CENTER, 119);
            //  graphics.print("few seconds...", CENTER, 132);

            //  graphics.setColor(0, 255, 0);
            //  graphics.setBackColor(0, 0, 255);
            //  graphics.print("Runtime: (msecs)", CENTER, 210);
            //  graphics.printNumI(millis(), CENTER, 225);

            Thread.Sleep(10000);
        }
        public void LibraryDemo()
        {



        }

        private void CheckCalibration()
        {
            if (!gm.IsCalibrated)
            {
                var cw = gm.CalibrationWindow;
                cw.Background = new SolidColorBrush(Color.CornflowerBlue);
                cw.CrosshairPen = new Pen(Color.Red, 1);

                TextBlock text = new TextBlock(0, 0, cw.Width, cw.Height / 2, Resources.GetFont(Resources.FontResources.CourierNew_10), "Please touch the crosshair")
                {
                    ForeColor = Color.White,
                    TextAlignment = TextAlignment.Center,
                    TextVerticalAlignment = VerticalAlignment.Center,
                    TextWrap = true
                };
                cw.Children.Add(text);

                cw.Show();
            }
        }
        private Bitmap GetBitmap(Resources.BinaryResources id, Bitmap.BitmapImageType type)
        {
            return new Bitmap(Resources.GetBytes(id), type);
        }
    }
}
