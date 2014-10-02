using MFE.Core;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class Label : UIElement
    {
        #region Fields
        private Font font;
        private string text;
        private Color foreColor;
        #endregion

        #region Properties
        public Font Font
        {
            get { return font; }
            set
            {
                VerifyAccess();
                if (font != value)
                {
                    font = value;
                    if (font != null)
                    {
                        Height = font.Height;
                        Invalidate();
                    }
                }
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                VerifyAccess();
                if (text != value)
                {
                    text = value;
                    CalculateSize();
                    Invalidate();
                }
            }
        }
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                VerifyAccess();
                if (foreColor != value)
                {
                    foreColor = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructor
        public Label(Font font, string text, Color foreColor)
        {
            Font = font;
            ForeColor = foreColor;
            Text = text;
        }
        #endregion

        #region Event handlers
        public override void OnRender(DrawingContext dc)
        {
            if (font != null && !Utils.StringIsNullOrEmpty(text))
                dc.DrawText(text, font, foreColor, 0, 0);
        }
        #endregion

        #region Private methods
        private void CalculateSize()
        {
            if (font != null && !Utils.StringIsNullOrEmpty(text))
            {
                int w, h;
                font.ComputeExtent(text, out w, out h);
                if (ActualWidth < w)
                    Width = w;
            }
        }
        #endregion
    }
}
