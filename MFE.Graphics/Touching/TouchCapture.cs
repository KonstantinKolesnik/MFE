using System;
using MFE.Graphics.Controls;

namespace MFE.Graphics.Touching
{
    public static class TouchCapture
    {
        private static Control capturedCtrl = null;

        public static Control Captured
        {
            get { return capturedCtrl; }
        }

        public static void Capture(Control ctrl)
        {
            if (ctrl == null)
                throw new ArgumentNullException();

            capturedCtrl = ctrl;
        }
        public static void ReleaseCapture()
        {
            capturedCtrl = null;
        }
    }
}
