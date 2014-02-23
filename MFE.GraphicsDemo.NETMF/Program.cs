using GHI.Premium.Hardware;
using MFE.GraphicsDemo.Content;

namespace MFE.GraphicsDemo.NETMF
{
    public class Program
    {
        private Demo demo;

        public static void Main()
        {
            new Program().Run();
        }

        private void Run()
        {
            //bool reset = false;
            //if (reset)
            //    SetLCDConfiguration_800_480();
            //if (reset)
            //{
            //    Util.FlushExtendedWeakReferences();
            //    Microsoft.SPOT.Hardware.PowerState.RebootDevice(false);
            //}

            demo = new Demo(800, 480, null);
            demo.QuickDemo();
        }

        private bool SetLCDConfiguration_800_480() // 7" LCD with resolution of 800x480
        {
            Configuration.LCD.Configurations lcdConfig = new Configuration.LCD.Configurations();

            lcdConfig.Width = 800;
            lcdConfig.Height = 480;

            //lcdConfig.PriorityEnable = true;
            lcdConfig.OutputEnableIsFixed = false;
            lcdConfig.OutputEnablePolarity = true;
            lcdConfig.PixelPolarity = false;

            lcdConfig.HorizontalSyncPolarity = false;
            lcdConfig.VerticalSyncPolarity = false;

            lcdConfig.HorizontalSyncPulseWidth = 150; // For EMX
            // lcdConfig.HorizontalSyncPulseWidth = 60; // On ChipworkX, there is a limited range for the HorizontalSyncPulseWidth. Set it to 60 instead.
            lcdConfig.HorizontalBackPorch = 150;
            lcdConfig.HorizontalFrontPorch = 150;

            lcdConfig.VerticalSyncPulseWidth = 2;
            lcdConfig.VerticalBackPorch = 2;
            lcdConfig.VerticalFrontPorch = 2;

            //lcdConfig.PixelClockDivider = 4;
            lcdConfig.PixelClockRateKHz = 18000;

            return Configuration.LCD.Set(lcdConfig);
        }
    }
}
