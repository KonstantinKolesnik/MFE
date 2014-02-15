using MFE.Graphics.Media;
using MFE.Utilities;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class TextBlock : Control
    {
        #region Fields
        private Font font;
        private string text = "";
        private Color foreColor = Color.Black;
        private bool wordWrap = false;
        private TextTrimming trimming = TextTrimming.WordEllipsis;
        private TextAlignment alignment = TextAlignment.Left;
        private VerticalAlignment valignment = VerticalAlignment.Top;
        private int textOffsetY = 0;
        private Brush background = null;
        #endregion

        #region  Properties
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
        public TextTrimming Trimming
        {
            get { return trimming; }
            set
            {
                if (trimming != value)
                {
                    trimming = value;
                    Invalidate();
                }
            }
        }
        public TextAlignment TextAlignment
        {
            get { return alignment; }
            set
            {
                if (alignment != value)
                {
                    alignment = value;
                    Invalidate();
                }
            }
        }
        public bool TextWrap
        {
            get { return wordWrap; }
            set
            {
                if (wordWrap != value)
                {
                    wordWrap = value;
                    Measure();
                    Invalidate();
                }
            }
        }
        public VerticalAlignment TextVerticalAlignment
        {
            get { return valignment; }
            set
            {
                if (valignment != value)
                {
                    valignment = value;
                    Measure();
                    Invalidate();
                }
            }
        }
        public Brush Background
        {
            get { return background; }
            set
            {
                if (background != value)
                {
                    background = value;
                    Invalidate();
                }
            }
        }

        public int LineHeight
        {
            //Don't support IgnoreDescent, etc...
            get { return font != null ? font.Height + font.ExternalLeading : 0; }
        }
        #endregion

        #region Constructors
        public TextBlock(int x, int y, int width, int height, Font font, string text)
            : base(x, y, width, height)
        {
            Font = font;
            Text = text;
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            if (background != null)
                dc.DrawRectangle(background, null, 0, 0, Width, Height);

            if (font != null && !Utils.IsStringNullOrEmpty(text))
            {
                string s = text; // this is important to take a copy!!! don't change!!!
                dc.DrawText(ref s, font, foreColor, 0, textOffsetY, Width, wordWrap ? Height : font.Height, alignment, trimming, wordWrap);
            }
        }

        #region Private methods
        private void Measure()
        {
            int w = 0, h = 0;
            if (font != null && !Utils.IsStringNullOrEmpty(text))
                font.ComputeExtent(text, out w, out h);

            switch (valignment)
            {
                case VerticalAlignment.Top: textOffsetY = 0; break;
                case VerticalAlignment.Bottom: textOffsetY = Height - h; break;
                case VerticalAlignment.Center: textOffsetY = (Height - h) / 2; break;
            }
        }
        #endregion
    }
}
