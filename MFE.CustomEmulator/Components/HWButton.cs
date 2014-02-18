using System.Windows.Forms;
using Microsoft.SPOT.Emulator.Gpio;

namespace MFE.CustomEmulator.Components
{
    /// <summary>
    /// A WinForm Button that connects a button to a GPIO pin.
    /// </summary>
    public class HWButton : Button
    {
        delegate void PortWriteDelegate(bool fState);

        private GpioPort port;

        public GpioPort Port
        {
            get { return port; }
            set { port = value; }
        }

        private void OnButtonStateChanged(bool pressed)
        {
            if (port != null)
            {
                bool val = false;

                switch (port.Resistor)
                {
                    case GpioResistorMode.Disabled:
                    case GpioResistorMode.PullUp:
                        val = pressed;
                        break;
                    case GpioResistorMode.PullDown:
                        val = !pressed;
                        break;
                }

                // Marshal the port-write to the Micro Framework thread.  
                // There's no need to wait for a response.
                port.BeginInvoke(new PortWriteDelegate(port.Write), !val);

                Invalidate();
            }
        }


        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    OnButtonStateChanged(true);
        //    base.OnKeyDown(e);
        //}
        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    OnButtonStateChanged(false);
        //    base.OnKeyUp(e);
        //}
        protected override void OnMouseDown(MouseEventArgs e)
        {
            OnButtonStateChanged(true);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            OnButtonStateChanged(false);
            base.OnMouseUp(e);
        }
    }
}
