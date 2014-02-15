using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class RadioButton : Control
    {
        #region Fields
        private bool isChecked = false;
        private Brush backgroundUnchecked;
        private Brush backgroundChecked;
        private Pen border;
        #endregion

        #region Properties
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    Invalidate();
                }
            }
        }
        public Brush BackgroundUnchecked
        {
            get { return backgroundUnchecked; }
            set
            {
                if (backgroundUnchecked != value)
                {
                    backgroundUnchecked = value;
                    Invalidate();
                }
            }
        }
        public Brush BackgroundChecked
        {
            get { return backgroundChecked; }
            set
            {
                if (backgroundChecked != value)
                {
                    backgroundChecked = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler CheckedChanged;
        protected void OnCheckedChanged(EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged(this, e);
        }
        #endregion

        #region Constructors
        public RadioButton(int x, int y, int diameter)
            : this(x, y, diameter, false)
        {
        }
        public RadioButton(int x, int y, int diameter, bool isChecked)
            : base(x, y, diameter, diameter)
        {
            border = new Pen(Color.Gray, 1);
            backgroundUnchecked = new SolidColorBrush(Color.White);

            Checked = isChecked;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            int centerX = Width / 2;
            int centerY = Height / 2;
            int radiusX = centerX;
            int radiusY = centerY;
            int ratio = 2;

            if (isChecked)
            {
                if (backgroundChecked != null)
                    dc.DrawRectangle(backgroundChecked, border, 0, 0, Width, Height);
                else
                {
                    dc.DrawEllipse(backgroundUnchecked, border, centerX, centerY, radiusX, radiusY);

                    SolidColorBrush b = new SolidColorBrush(Color.LimeGreen);
                    dc.DrawEllipse(b, border, centerX, centerY, radiusX / ratio, radiusY / ratio);
                }
            }
            else
            {
                dc.DrawEllipse(backgroundUnchecked, border, centerX, centerY, radiusX, radiusY);
            }
        }

        #region Event handlers
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (Enabled)
            {
                Checked = true;
                OnCheckedChanged(EventArgs.Empty);
            }
        }
        #endregion
    }
}
