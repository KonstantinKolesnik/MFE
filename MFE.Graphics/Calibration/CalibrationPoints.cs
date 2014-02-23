using System;

namespace MFE.Graphics.Calibration
{
    [Serializable]
    sealed class CalibrationPoints
    {
        public short[] ScreenX = null;
        public short[] ScreenY = null;
        public short[] TouchX = null;
        public short[] TouchY = null;

        public int Count
        {
            get { return ScreenX == null ? 0 : ScreenX.Length; }
        }
    }
}
