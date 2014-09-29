using MFE.GraphicsDemo.Content;

namespace MFE.GraphicsDemo.NETMF
{
    public class Program
    {
        private DemoManager demo;

        public static void Main()
        {
            new Program().Run();
        }

        private void Run()
        {
            //bool reset = false;
            //if (reset)
            //    LCDManager.SetLCDConfiguration_800_480();
            //if (reset)
            //{
            //    Util.FlushExtendedWeakReferences();
            //    Microsoft.SPOT.Hardware.PowerState.RebootDevice(false);
            //}



            //demo = new Demo(800, 480);
            //demo.QuickDemo();

            demo = new DemoManager(320, 240);
            demo.LibraryDemo();
        }
    }
}
