

using Gadgeteer.Modules.KKS;
using MFE.Graphics.Geometry;
using MFE.GraphicsDemo.Content;
using Microsoft.SPOT;
namespace MFE.GraphicsDemo.ArgonR1
{
    public partial class Program
    {
        bool on = false;
        private DisplayS22 display;
        private DemoManager demo;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            //GT.Timer t = new GT.Timer(500);
            //t.Tick += (GT.Timer tt) =>
            //{
            //    Mainboard.SetDebugLED(on);
            //    on = !on;
            //};
            //t.Start();

            Demo22SPI();


        }

        private void Demo22SPI()
        {
            display = new DisplayS22(5);

            demo = new DemoManager(240, 320, delegate(Bitmap bitmap, Rect dirtyArea)
            {
                display.SimpleGraphics.DisplayImage(bitmap, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.X, (uint)dirtyArea.Y, (uint)dirtyArea.Width, (uint)dirtyArea.Height);
            });
            demo.QuickDemo();
        }

    }
}
