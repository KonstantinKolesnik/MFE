using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class HighlightableListBoxItem : ListBoxItem
    {
        private ushort opacity = Bitmap.OpacityOpaque;
        private Bitmap selectionBackgroundImage;

        public Bitmap SelectionBackgroundImage
        {
            get { return selectionBackgroundImage; }
            set
            {
                if (selectionBackgroundImage != value)
                {
                    selectionBackgroundImage = value;
                    Invalidate();
                }
            }
        }
        public ushort Opacity
        {
            get { return opacity; }
            set
            {
                CheckAccess();
                if (opacity != value)
                {
                    opacity = value;
                    Invalidate();
                }
            }
        }

        public HighlightableListBoxItem()
        {
            Background = null;
        }
        public HighlightableListBoxItem(UIElement content)
            : this()
        {
            Child = content;
        }

        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (IsSelected)
            {
                if (selectionBackgroundImage != null)
                    dc.Scale9Image(0, 0, ActualWidth, ActualHeight, selectionBackgroundImage, 0, 0, 0, 0, opacity);
            }
        }

        protected override void OnIsSelectedChanged(bool isSelected)
        {
            Invalidate();
        }
    }
}
