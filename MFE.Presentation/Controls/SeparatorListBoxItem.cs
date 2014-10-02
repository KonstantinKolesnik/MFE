using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace Typhoon.MF.Presentation.Controls
{
    public class SeparatorListBoxItem : ListBoxItem
    {
        private Color color;

        public SeparatorListBoxItem(Color color)
        {
            this.color = color;
            Height = 1;
            IsSelectable = false;
        }

        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(null, new Pen(color), 0, 0, ActualWidth, ActualHeight);
        }
    }
}
