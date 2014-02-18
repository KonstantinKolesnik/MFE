using Microsoft.SPOT.Emulator;
using Microsoft.SPOT.Emulator.TouchPanel;
using System.Threading;
using System.Windows.Forms;

namespace MFE.CustomEmulator
{
    class Program : Emulator
    {
        private FormMain frmMain;

        static void Main()
        {
            (new Program()).Start();
        }

        public override void SetupComponent()
        {
            RegisterComponent(new TouchGpioPort(TouchGpioPort.DefaultTouchPin));
            base.SetupComponent();
        }
        public override void InitializeComponent()
        {
            base.InitializeComponent();

            //frmMain = new FormMain(Emulator);
            frmMain = new FormMain(this);
            frmMain.OnInitializeComponent();


            // Start the UI in its own thread.
            Thread uiThread = new Thread(StartUI);
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
        }
        public override void UninitializeComponent()
        {
            base.UninitializeComponent();

            // The emulator is stopped. Close the WinForm UI.
            Application.Exit();
        }

        private void StartUI()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(frmMain);

            // When the user closes the WinForm UI, stop the emulator.
            Stop();
            //Emulator.Stop();
        }
    }
}
