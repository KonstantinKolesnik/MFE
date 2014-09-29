using Gadgeteer.Modules.KKS;
using GHI.Premium.System;
using MFE.Core;
using MFE.Graphics.Geometry;
using MFE.GraphicsDemo.Content;
using MFE.LCD;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace MFE.GraphicsDemo
{
    public partial class Program
    {
        private DemoManager demo;
        private DisplayS22 display;

        void ProgramStarted()
        {
            //Demo22SPI();
            Demo35();
            //Demo35Video();

            ////Mainboard.SetDebugLED(true);
        }

        private void Demo22SPI()
        {
            display = new DisplayS22(11);

            demo = new DemoManager(240, 320, delegate(Bitmap bitmap, Rect dirtyArea)
            {
                display.SimpleGraphics.DisplayImage(bitmap, dirtyArea.X, dirtyArea.Y, dirtyArea.X, dirtyArea.Y, dirtyArea.Width, dirtyArea.Height);
            });
            demo.QuickDemo();
        }
        private void Demo35()
        {
            //if (!Utils.IsEmulator)
            //{
            //    bool reboot = false;
            //    //reboot |= LCDManager.SetLCDConfiguration_800_480();
            //    reboot |= LCDManager.SetLCDConfiguration_320_240();
            //    //reboot |= LCDManager.SetBootLogo(null, 0, 0);
            //    reboot |= LCDManager.SetLCDBootupMessages(false);

            //    if (reboot)
            //    {
            //        Thread.Sleep(10000);
            //        Util.FlushExtendedWeakReferences();
            //        PowerState.RebootDevice(false);
            //    }
            //}




            demo = new DemoManager(320, 240);
            //demo.QuickDemo();
            demo.CrashDemo();
        }
        private void Demo35Video()
        {
            //Configuration.LCD.EnableLCDBootupMessages(false);

            demo = new DemoManager(320, 240);
            demo.VideoDemo();
        }
    }
}
