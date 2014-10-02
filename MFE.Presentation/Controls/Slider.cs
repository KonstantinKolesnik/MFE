using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;

namespace Typhoon.MF.Presentation.Controls
{
    public class Slider : UIElement
    {
        #region Fields
        //private Color highlightColor = Colors.Blue;
        //private bool hasHighlight = true;

        private float scale = 1f;
        private float max = 100f;
        private float value = 0f;
        private bool isSliding = false;
        private float slideOffset = 0;
        private float thumbPos; // center of thumb; relative

        private Rectangle dstRectLeft;
        private Rectangle dstRectMid;
        private Rectangle dstRectRight;
        private Rectangle dstRectMidHighlight;
        private Rectangle dstRectThumb;

        //private Rectangle srcRectThumb = new Rectangle(21, 36, 12, 12);
        //private Rectangle srcRectLeft = new Rectangle(11, 36, 4, 8);
        //private Rectangle srcRectMid = new Rectangle(17, 36, 1, 8);

        private Brush trackBrush;
        private int trackSize = 2;
        private Brush thumbBrush;
        #endregion

        #region Properties
        public float Value
        {
            get { return value; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > max)
                    value = max;
                if (this.value != value)
                {
                    float old = this.value;
                    this.value = value;
                    Invalidate();
                    if (ValueChanged != null)
                        ValueChanged(this, new PropertyChangedEventArgs("Value", old, value));
                }
            }
        }
        //public Color HighlightColor
        //{
        //    set
        //    {
        //        if (highlightColor != value)
        //            highlightColor = value;
        //    }
        //}
        //public bool HasHighlight
        //{
        //    set
        //    {
        //        if (hasHighlight != value)
        //            hasHighlight = value;
        //    }
        //}

        public Brush Background
        {
            get { return trackBrush; }
            set
            {
                if (trackBrush != value)
                {
                    trackBrush = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler ValueChanged;
        #endregion

        #region Constructors
        public Slider(int width, int height)
        {
            Width = width;
            Height = height;
            trackBrush = new SolidColorBrush(Colors.DarkGray);
            thumbBrush = new SolidColorBrush(Colors.Orange);

            //this.TouchMove

            Invalidate();
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            int r = Height / 2 - 1;

            // track
            int w = Width - 2 * r;
            dc.DrawRectangle(trackBrush, null, r, (Height - trackSize) / 2, w, trackSize);


            // thumb
            int x = (int)(r + (Width - 2 * r - 1) * value / 100);
            int y = r;
            dc.DrawEllipse(thumbBrush, null, x, y, r, r);


            ////Draw Slider
            //spriteBatch.Draw(GUIManager.skin, dstRectLeft, srcRectLeft, (hasHighlight ? highlightColor : BackColor));
            //spriteBatch.Draw(GUIManager.skin, dstRectMid, srcRectMid, BackColor);
            //spriteBatch.Draw(GUIManager.skin, dstRectRight, srcRectLeft, BackColor, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);

            ////Draw Slider Highlight
            //if (hasHighlight)
            //    spriteBatch.Draw(GUIManager.skin, dstRectMidHighlight, srcRectMid, highlightColor);

            ////Draw Thumb
            //spriteBatch.Draw(GUIManager.skin, dstRectThumb, srcRectThumb, BackColor);
        }

        internal void Update()
        {
            //scale = Height / (float)srcRectLeft.Height;

            //dstRectLeft = new Rectangle((int)ScreenPosition.X, (int)ScreenPosition.Y, (int)(srcRectLeft.Width * scale), (int)(srcRectLeft.Height * scale));
            //dstRectMid = new Rectangle(dstRectLeft.X + dstRectLeft.Width - 1, dstRectLeft.Y, (int)Width - dstRectLeft.Width * 2, dstRectLeft.Height);
            //dstRectRight = new Rectangle(dstRectMid.X + dstRectMid.Width - 1, dstRectLeft.Y, dstRectLeft.Width, dstRectLeft.Height);

            //dstRectMidHighlight = dstRectMid;
            //dstRectMidHighlight.Width = (int)((float)dstRectMid.Width * value / max);

            //dstRectThumb = new Rectangle((int)ScreenPosition.X, (int)ScreenPosition.Y, (int)(srcRectThumb.Width * scale), (int)(srcRectThumb.Height * scale));
            //dstRectThumb.X += (int)(thumbPos - dstRectThumb.Width / 2f);
            //dstRectThumb.Y -= (int)((dstRectThumb.Height - dstRectLeft.Height) / 2f);

            //UpdateSliding();
        }

        #region Private methods
        private void UpdateSliding()
        {
            //if (InputSystem.Mouse.InBox(dstRectThumb) && InputSystem.Mouse.LBJustPressed)
            //{
            //    isSliding = true;
            //    slideOffset = InputSystem.Mouse.Position.X - ScreenPosition.X - thumbPos;
            //}
            //if (isSliding && InputSystem.Mouse.LBReleased)
            //    isSliding = false;

            //if (!isSliding)
            //    thumbPos = (Value / max) * Width;
            //else
            //{
            //    thumbPos = InputSystem.Mouse.Position.X - ScreenPosition.X - slideOffset;
            //    if (thumbPos < 0)
            //        thumbPos = 0;
            //    else if (thumbPos > Width)
            //        thumbPos = Width;
            //}

            //Value = thumbPos * max / Width;
        }
        #endregion






    }
}
