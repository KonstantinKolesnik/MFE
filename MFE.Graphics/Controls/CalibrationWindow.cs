using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using MFE.LCD;

namespace MFE.Graphics.Controls
{
    public sealed class CalibrationWindow : Window
    {
        #region Fields
        private int idx;
        private Pen pen;
        #endregion

        #region Properties
        public override int X
        {
            get { return base.X; }
        }
        public override int Y
        {
            get { return base.Y; }
        }
        public override int Width
        {
            get { return base.Width; }
        }
        public override int Height
        {
            get { return base.Height; }
        }
        public Pen CrosshairPen
        {
            get { return pen; }
            set { pen = value; }
        }
        #endregion

        #region Constructor
        public CalibrationWindow()
            : base(0, 0, LCDManager.ScreenWidth, LCDManager.ScreenHeight)
        {
            pen = new Pen(Color.Red, 1);

            CalibrationManager.PrepareCalibrationPoints();
            CalibrationManager.StartCalibration();
            idx = 0;
        }
        #endregion

        #region Event handlers
        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // draw crosshair
            if (pen != null && idx < CalibrationManager.CalibrationPoints.Count)
            {
                int x = CalibrationManager.CalibrationPoints.ScreenX[idx];
                int y = CalibrationManager.CalibrationPoints.ScreenY[idx];

                dc.DrawLine(pen, x - 10, y, x - 2, y);
                dc.DrawLine(pen, x + 10, y, x + 2, y);

                dc.DrawLine(pen, x, y - 10, x, y - 2);
                dc.DrawLine(pen, x, y + 10, x, y + 2);
            }
        }
        protected override void OnTouchUp(TouchEventArgs e)
        {
            Point p = e.Point;
            PointToClient(ref p);

            CalibrationManager.CalibrationPoints.TouchX[idx] = (short)p.X;
            CalibrationManager.CalibrationPoints.TouchY[idx] = (short)p.Y;

            idx++;

            if (idx == CalibrationManager.CalibrationPoints.Count)
            {
                // The last point has been reached.
                CalibrationManager.ApplyCalibrationPoints();
                CalibrationManager.SaveCalibrationPoints();
                Close();
            }
            else
                Invalidate();
        }
        #endregion
    }
}
