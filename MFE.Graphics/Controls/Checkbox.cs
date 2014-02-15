using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class Checkbox : Control
    {
        #region Fields
        private bool isChecked;
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
        public Pen Border
        {
            get { return border; }
            set
            {
                if (border != value)
                {
                    border = value;
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
        public Checkbox(int x, int y, int width, int height)
            : this(x, y, width, height, false)
        {
        }
        public Checkbox(int x, int y, int width, int height, bool isChecked)
            : base(x, y, width, height)
        {
            border = new Pen(Color.Gray, 1);
            backgroundUnchecked = new SolidColorBrush(Color.White);

            Checked = isChecked;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            if (isChecked)
            {
                if (backgroundChecked != null)
                    dc.DrawRectangle(backgroundChecked, border, 0, 0, Width, Height);
                else
                {
                    dc.DrawRectangle(backgroundUnchecked, border, 0, 0, Width, Height);

                    int offset = 2;
                    LinearGradientBrush b = new LinearGradientBrush(Color.LimeGreen, Color.Black);
                    dc.DrawRectangle(b, null, offset, offset, Width - 2 * offset, Height - 2 * offset);
                }
            }
            else
            {
                dc.DrawRectangle(backgroundUnchecked, border, 0, 0, Width, Height);
            }
        }

        #region Event handlers
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (Enabled)
            {
                Checked = !Checked;
                OnCheckedChanged(EventArgs.Empty);
            }
        }
        #endregion
    }
}
