using System;

namespace MFE.LCD
{
    [Serializable]
    public sealed class CalibrationPoints
    {
        public short[] ScreenX;
        public short[] ScreenY;
        public short[] TouchX;
        public short[] TouchY;

        public int Count
        {
            get { return ScreenX == null ? 0 : ScreenX.Length; }
        }
    }
}
