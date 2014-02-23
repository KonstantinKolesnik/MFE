using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using System.Threading;

namespace MFE.Graphics.Calibration
{
    public sealed class CalibrationWindow : Container
    {
        #region Fields
        private GraphicsManager gm;
        private ManualResetEvent block;
        private int idx = 0;
        private Pen crosshairPen;
        #endregion

        #region Properties
        public Pen CrosshairPen
        {
            get { return crosshairPen; }
            set { crosshairPen = value; }
        }

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
        #endregion

        #region Constructor
        internal CalibrationWindow(int width, int height, GraphicsManager gm)
            : base(0, 0, width, height)
        {
            this.gm = gm;

            Background = new SolidColorBrush(Color.CornflowerBlue);
            CrosshairPen = new Pen(Color.Red, 1);
        }
        #endregion

        #region Public methods
        public void Show()
        {
            if (Background == null)
                Background = new SolidColorBrush(Color.CornflowerBlue);
            if (CrosshairPen == null)
                CrosshairPen = new Pen(Color.Red, 1);

            idx = 0;
            CalibrationManager.PrepareCalibrationPoints();
            CalibrationManager.StartCalibration();

            gm.Desktop.Children.Add(this);

            block = new ManualResetEvent(false);
            block.WaitOne();
        }
        #endregion

        #region Event handlers
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // draw crosshair
            if (crosshairPen != null && CalibrationManager.CalibrationPoints != null && idx < CalibrationManager.CalibrationPoints.Count)
            {
                int x = CalibrationManager.CalibrationPoints.ScreenX[idx];
                int y = CalibrationManager.CalibrationPoints.ScreenY[idx];

                dc.DrawLine(crosshairPen, x - 10, y, x - 2, y);
                dc.DrawLine(crosshairPen, x + 10, y, x + 2, y);

                dc.DrawLine(crosshairPen, x, y - 10, x, y - 2);
                dc.DrawLine(crosshairPen, x, y + 10, x, y + 2);
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

                block.Set();
                Parent.Children.Remove(this);
            }
            else
                Invalidate();
        }
        #endregion
    }
}
