using MFE.Graphics.Media;
using MFE.Utilities;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class Label : Control
    {
        #region Fields
        private Font font;
        private string text = "";
        private Color foreColor = Color.Black;
        #endregion

        #region  Properties
        public override int Width
        {
            get { return base.Width; }
        }
        public override int Height
        {
            get { return base.Height; }
        }

        public Font Font
        {
            get { return font; }
            set
            {
                if (font != value)
                {
                    font = value;
                    Measure();
                    Invalidate();
                }
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;

                    Measure();
                    Invalidate();
                }
            }
        }
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructors
        public Label(int x, int y, Font font, string text)
            : base(x, y, 0, 0)
        {
            Text = text;
            Font = font;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            if (font != null && !Utils.IsStringNullOrEmpty(text))
                dc.DrawText(text, font, foreColor, 0, 0);
        }

        #region Private methods
        private void Measure()
        {
            int w = 0, h = 0;

            if (font != null && !Utils.IsStringNullOrEmpty(text))
                font.ComputeExtent(text, out w, out h);

            Width = w;
            Height = h;
        }
        #endregion
    }
}
