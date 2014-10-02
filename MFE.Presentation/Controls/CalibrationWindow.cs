using MFE.LCD;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class CalibrationWindow : Window
    {
        #region Fields
        private int idx;
        private Pen pen;
        #endregion

        #region Events
        public event EventHandler CalibrationComplete;
        #endregion

        #region Constructor
        public CalibrationWindow(Font font, string txt)
        {
            Height = SystemMetrics.ScreenHeight;
            Width = SystemMetrics.ScreenWidth;
            Visibility = Visibility.Visible;
            //Buttons.Focus(this);

            pen = new Pen(ColorUtility.ColorFromRGB(255, 0, 0), 1);

            Text text = new Text();
            text.Font = font;
            text.ForeColor = Colors.Blue;
            text.TextContent = txt;
            text.TextWrap = true;
            text.SetMargin(0, 0, 0, SystemMetrics.ScreenHeight / 2);
            text.TextAlignment = TextAlignment.Center;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            Child = text;

            CalibrationManager.PrepareCalibrationPoints();
            CalibrationManager.StartCalibration();
            idx = 0;

            Invalidate();
        }
        #endregion

        #region Event handlers
        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // draw crosshair
            if (idx < CalibrationManager.CalibrationPoints.Count)
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
            base.OnTouchUp(e);

            int x, y;
            e.GetPosition(this, 0, out x, out y);

            CalibrationManager.CalibrationPoints.TouchX[idx] = (short)x;
            CalibrationManager.CalibrationPoints.TouchY[idx] = (short)y;

            idx++;

            if (idx == CalibrationManager.CalibrationPoints.Count)
            {
                // The last point has been reached.
                CalibrationManager.ApplyCalibrationPoints();
                CalibrationManager.SaveCalibrationPoints();
                
                Close();

                if (CalibrationComplete != null)
                    CalibrationComplete(this, EventArgs.Empty);
            }

            Invalidate();
        }
        #endregion
    }
}
