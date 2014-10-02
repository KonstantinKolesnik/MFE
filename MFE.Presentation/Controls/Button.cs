using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using MFE.Core;

namespace MFE.Presentation.Controls
{
    public class Button : Panel
    {
        struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        #region Fields
        private SolidColorBrush brushUnpressed;
        private SolidColorBrush brushPressed;
        private SolidColorBrush brushTransparent;
        private Pen border;

        private bool pressed = false;
        #endregion

        #region Properties
        public UIElement Content
        {
            get { return LogicalChildren.Count > 0 ? LogicalChildren[0] : null; }
            set
            {
                VerifyAccess();

                LogicalChildren.Clear();
                if (value != null)
                    LogicalChildren.Add(value);
            }
        }
        #endregion

        #region Events
        public event EventHandler Click;
        protected void OnClick(EventArgs e)
        {
            if (Click != null)
                Click(this, e);
        }
        #endregion

        #region Constructors
        public Button()
            : this(null)
        {
        }
        public Button(UIElement content)
        {
            brushUnpressed = new SolidColorBrush(Colors.Black);
            brushUnpressed.Opacity = 10;

            brushPressed = new SolidColorBrush(ColorUtility.ColorFromRGB(128, 255, 0));
            brushPressed.Opacity = 200;

            brushTransparent = new SolidColorBrush(Colors.Red);
            brushTransparent.Opacity = Bitmap.OpacityTransparent;

            border = new Pen(Colors.DarkGray, 1);

            Content = content;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(pressed ? brushPressed : brushUnpressed, null, 0, 0, ActualWidth, ActualHeight);
            //if (pressed)
            //    dc.Bitmap.DrawRectangle(Color.Black, 0, 0, 0, ActualWidth, ActualHeight, 0, 0, ColorUtility.ColorFromRGB(128, 255, 0), 0, 0, ColorUtility.ColorFromRGB(128, 255, 0), ActualWidth, ActualHeight, 150);
            //else
            //    dc.Bitmap.DrawRectangle(Color.Black, 0, 0, 0, ActualWidth, ActualHeight, 0, 0, Colors.Black, 0, 0, Colors.Black, ActualWidth, ActualHeight, 10);

            // border
            int corner = 2;
            int right = ActualWidth - 1;
            int bottom = ActualHeight - 1;
            dc.DrawLine(border, corner, 0, right - corner, 0); // top
            dc.DrawLine(border, corner, bottom, right - corner, bottom); // bottom
            dc.DrawLine(border, 0, corner, 0, bottom - corner); // left
            dc.DrawLine(border, right, corner, right, bottom - corner); // rigth
            dc.DrawLine(border, corner, 0, 0, corner); // upper-left
            dc.DrawLine(border, right - corner, 0, right, corner); // upper-right
            dc.DrawLine(border, 0, bottom - corner, corner, bottom); // bottom-left
            dc.DrawLine(border, right, bottom - corner, right - corner, bottom); // bottom-right

            // transparent corners
            ArrayList transparentPoints = new ArrayList();
            transparentPoints.Add(new Point(0, 0));
            transparentPoints.Add(new Point(1, 0));
            transparentPoints.Add(new Point(0, 1));

            transparentPoints.Add(new Point(right, 0));
            transparentPoints.Add(new Point(right - 1, 0));
            transparentPoints.Add(new Point(right, 1));

            transparentPoints.Add(new Point(0, bottom));
            transparentPoints.Add(new Point(1, bottom));
            transparentPoints.Add(new Point(0, bottom - 1));

            transparentPoints.Add(new Point(right, bottom));
            transparentPoints.Add(new Point(right - 1, bottom));
            transparentPoints.Add(new Point(right, bottom - 1));

            foreach (Point p in transparentPoints)
                //dc.DrawRectangle(brushTransparent, null, p.X, p.Y, 1, 1);
                dc.Bitmap.DrawRectangle(Color.Black, 0, p.X, p.Y, 1, 1, 0, 0, Colors.Red, 0, 0, Colors.Red, 0, 0, 0);
        }

        #region Event handlers
        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                pressed = true;
                Invalidate();
                TouchCapture.Capture(this);
            }
        }
        protected override void OnTouchMove(TouchEventArgs e)
        {
            if (pressed)
            {
                int x, y;
                e.GetPosition(this, 0, out x, out y); // uses UIElement.PointToClient
                if (!Utils.IsWithinRectangle(x, y, ActualWidth, ActualHeight))
                {
                    pressed = false;
                    Invalidate();
                    TouchCapture.Capture(this, CaptureMode.None);
                }
            }
        }
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (pressed)
            {
                pressed = false;
                Invalidate();
                TouchCapture.Capture(this, CaptureMode.None);

                int x, y;
                e.GetPosition(this, 0, out x, out y);
                if (Utils.IsWithinRectangle(x, y, ActualWidth, ActualHeight))
                    OnClick(EventArgs.Empty);
            }
        }
        #endregion
    }
}
