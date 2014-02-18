using System;
using Microsoft.SPOT;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;

namespace MFE.GraphicsDemo.Demos
{
    class SliderDemo : Panel
    {
        public SliderDemo()
            : base(0, 0, 0, 0)
        {
            //Slider sldr;


            Children.Add(new Slider(20, 20, 150, 30, 15, Orientation.Horizontal)
            {
                Value = 80,
                Background = new ImageBrush(Program.GetBitmap(Resources.BinaryResources.Bar, Bitmap.BitmapImageType.Bmp)),
                Foreground = new LinearGradientBrush(Color.LightGray, Color.Black) { Opacity = 50 }
            });

            Children.Add(new Slider(20, 60, 30, 150, 12, Orientation.Vertical)
            {
                Value = 70,
                Background = new SolidColorBrush(Color.CornflowerBlue) { Opacity = 100 }
            });

            Slider slider = new Slider(250, 20, 150, 32, 32, Orientation.Horizontal)
            {
                Value = 80,
                Background = new ImageBrush(Program.GetBitmap(Resources.BinaryResources.Bar, Bitmap.BitmapImageType.Bmp)),
                //Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Logo)) { Opacity = 200 },
                Foreground = new ImageBrush(Program.GetBitmap(Resources.BinaryResources.PowerOn, Bitmap.BitmapImageType.Gif)) { Opacity = 200 },
                ThumbBorder = null
            };
            Children.Add(slider);
            //Label lbl = new Label(250, 100, font, slider.Value.ToString()) { ForeColor = Color.White };
            //Children.Add(lbl);
            //slider.ValueChanged += delegate(object sender, ValueChangedEventArgs e) { lbl.Text = e.Value.ToString(); };


            Children.Add(new Slider(250, 100, 150, 30, 15, Orientation.Horizontal)
            {
                Value = 30,
                Background = new LinearGradientBrush(Color.LimeGreen, Color.Blue) { Opacity = 150 },
                Foreground = new LinearGradientBrush(Color.Red, Color.CornflowerBlue) { Opacity = 150 },
                ThumbBorder = new Pen(Color.Green, 1)
            });
        }
    }
}
