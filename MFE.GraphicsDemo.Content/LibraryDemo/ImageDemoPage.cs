using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using Microsoft.SPOT;

namespace MFE.GraphicsDemo.Content.LibraryDemo
{
    class ImageDemoPage : DemoPage
    {
        public ImageDemoPage()
        {
            Image img;
            int size = 64;
            Bitmap bmp = DemoManager.GetBitmap(Resources.BinaryResources.Operation, Bitmap.BitmapImageType.Gif);

            img = new Image(20, 10, size, size, bmp);
            Content.Children.Add(img);

            img = new Image(120, 10, size+100, size, bmp);
            Content.Children.Add(img);


            img = new Image(20, 80, size, size, bmp) { Border = new Pen(Color.Red, 1) };
            Content.Children.Add(img);

            img = new Image(120, 80, size, size, bmp) { Opacity = 80 };
            Content.Children.Add(img);

            img = new Image(240, 80, size, size, bmp) { Opacity = 150, Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue) { Opacity = 80 } };
            Content.Children.Add(img);
        }
    }
}
