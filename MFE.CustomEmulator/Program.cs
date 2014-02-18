using Microsoft.SPOT.Emulator;
using Microsoft.SPOT.Emulator.TouchPanel;
using System.Threading;
using System.Windows.Forms;

namespace MFE.CustomEmulator
{
    class Program : Emulator
    {
        public override void SetupComponent()
        {
            this.GpioPorts.MaxPorts = 128;

            //GpioPort motorUp = new GpioPort();
            //motorUp.ComponentId = "MotorUpButton";
            //motorUp.Pin = (Microsoft.SPOT.Hardware.Cpu.Pin)20;
            //motorUp.ModesAllowed = GpioPortMode.InputPort;
            //motorUp.ModesExpected = GpioPortMode.InputPort;

            //GpioPort motorDown = new GpioPort();
            //motorDown.ComponentId = "MotorDownButton";
            //motorDown.Pin = (Microsoft.SPOT.Hardware.Cpu.Pin)21;
            //motorDown.ModesAllowed = GpioPortMode.InputPort;
            //motorDown.ModesExpected = GpioPortMode.InputPort;

            //RegisterComponent(motorUp);
            //RegisterComponent(motorDown);

            RegisterComponent(new TouchGpioPort(TouchGpioPort.DefaultTouchPin));


            base.SetupComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            // Start the UI in its own thread.
            Thread uiThread = new Thread(StartForm);
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
        }
        public override void UninitializeComponent()
        {
            base.UninitializeComponent();

            // The emulator is stopped. Close the WinForm UI.
            Application.Exit();
        }

        private void StartForm()
        {
            // Some initial setup for the WinForm UI
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the WinForm UI. Run() returns when the form is closed.
            frmMain frm = new frmMain(this);
            frm.OnInitializeComponent();
            Application.Run(frm);

            // When the user closes the WinForm UI, stop the emulator.
            Stop();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            (new Program()).Start();
        }
    }
}
