using Microsoft.SPOT;
using System;

namespace MFE.Graphics.Media
{
    public abstract class Brush
    {
        private ushort opacity = Bitmap.OpacityOpaque;

        public ushort Opacity
        {
            get { return opacity; }
            set
            {
                // clip values              
                if (value > Bitmap.OpacityOpaque)
                    value = Bitmap.OpacityOpaque;

                opacity = value;
            }
        }

        protected internal virtual void RenderRectangle(Bitmap bmp, Pen outline, int x, int y, int width, int height)
        {
            throw new NotSupportedException("RenderRectangle is not supported with this brush.");
        }
        protected internal virtual void RenderEllipse(Bitmap bmp, Pen outline, int x, int y, int xRadius, int yRadius)
        {
            throw new NotSupportedException("RenderEllipse is not supported with this brush.");
        }
        protected internal virtual void RenderPolygon(Bitmap bmp, Pen outline, int[] points)
        {
            throw new NotSupportedException("RenderPolygon is not supported with this brush.");
        }
    }
}
