using MFE.Core;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class ToolButton : Panel
    {
        #region Fields
        private string name;
        private SolidColorBrush brushPressed;
        private bool pressed = false;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
        }
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
        public ToolButton()
            : this("")
        {
        }
        public ToolButton(string name)
            : this(name, null)
        {
        }
        public ToolButton(string name, UIElement content)
        {
            this.name = name;

            brushPressed = new SolidColorBrush(ColorUtility.ColorFromRGB(128, 255, 0));
            brushPressed.Opacity = 200;

            Content = content;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            //if (pressed)
            if (TouchCapture.Captured == this)
                dc.DrawRectangle(brushPressed, null, 0, 0, ActualWidth, ActualHeight);
        }

        #region Event handlers
        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (IsEnabled && TouchCapture.Captured != this)
            {
                int x, y;
                e.GetPosition(this, 0, out x, out y);
                if (Utils.IsWithinRectangle(x, y, ActualWidth, ActualHeight))
                {
                    TouchCapture.Capture(this);
                    pressed = true;
                    Invalidate();
                }
            }
        }
        protected override void OnTouchMove(TouchEventArgs e)
        {
            //if (pressed)
            if (TouchCapture.Captured == this)
            {
                int x, y;
                e.GetPosition(this, 0, out x, out y); // uses UIElement.PointToClient
                if (!Utils.IsWithinRectangle(x, y, ActualWidth, ActualHeight))
                {
                    TouchCapture.Capture(this, CaptureMode.None);
                    pressed = false;
                    Invalidate();
                }
            }
        }
        protected override void OnTouchUp(TouchEventArgs e)
        {
            //if (pressed)
            if (TouchCapture.Captured == this)
            {
                TouchCapture.Capture(this, CaptureMode.None);
                pressed = false;
                Invalidate();

                int x, y;
                e.GetPosition(this, 0, out x, out y);
                if (Utils.IsWithinRectangle(x, y, ActualWidth, ActualHeight))
                    OnClick(EventArgs.Empty);
            }
        }
        #endregion
    }
}
