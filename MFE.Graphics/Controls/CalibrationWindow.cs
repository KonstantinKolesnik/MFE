using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;

namespace MFE.Graphics.Controls
{
    public sealed class CalibrationWindow : Container
    {
        #region Fields
        private int idx;
        private Pen pen;
        #endregion

        #region Properties
        public Pen CrosshairPen
        {
            get { return pen; }
            set { pen = value; }
        }
        #endregion

        #region Constructor
        internal CalibrationWindow(int width, int height)
            : base(0, 0, width, height)
        {
            pen = new Pen(Color.Red, 1);

            CalibrationManager.PrepareCalibrationPoints();
            CalibrationManager.StartCalibration();
            idx = 0;
        }
        #endregion

        #region Event handlers
        protected override void OnRender(DrawingContext dc)
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
                
                //Close();
                Parent.Children.Remove(this);
            }
            else
                Invalidate();
        }
        #endregion
    }
}
