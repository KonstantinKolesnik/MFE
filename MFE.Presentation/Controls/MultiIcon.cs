using MFE.Core;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Collections;

namespace MFE.Presentation.Controls
{
    public class MultiIcon : UIElement
    {
        #region Fields
        private Hashtable brushes;
        private int iconSize;
        private string activeBitmapID;
        private ushort opacity = Bitmap.OpacityOpaque;
        #endregion

        #region Properties
        public Bitmap this[string bitmapID]
        {
            get
            {
                if (!brushes.Contains(bitmapID) || (ImageBrush)brushes[bitmapID] == null)
                    return null;
                else
                    return ((ImageBrush)brushes[bitmapID]).BitmapSource;
            }
            set
            {
                if (brushes.Contains(bitmapID))
                    ((ImageBrush)brushes[bitmapID]).BitmapSource = value;
                else
                {
                    ImageBrush brush = new ImageBrush(value);
                    brush.Stretch = Stretch.Fill;
                    brushes.Add(bitmapID, brush);
                }
            }
        }
        public int IconSize
        {
            get { return iconSize; }
            set
            {
                VerifyAccess();
                if (iconSize != value)
                {
                    iconSize = value;
                    Width = Height = iconSize;
                }
            }
        }
        public string ActiveBitmapID
        {
            get { return activeBitmapID; }
            set
            {
                CheckAccess();
                if (activeBitmapID != value)
                {
                    activeBitmapID = value;
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
                    foreach (string key in brushes.Keys)
                    {
                        ImageBrush brush = (ImageBrush)brushes[key];
                        if (brush != null)
                            brush.Opacity = opacity;
                    }
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructors
        public MultiIcon(int iconSize)
            : this(iconSize, Bitmap.OpacityOpaque)
        {
        }
        public MultiIcon(int iconSize, ushort opacity)
        {
            brushes = new Hashtable();

            IconSize = iconSize;
            Opacity = opacity;
        }
        #endregion

        #region Event handlers
        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            desiredWidth = desiredHeight = iconSize;
        }
        public override void OnRender(DrawingContext dc)
        {
            if (!Utils.StringIsNullOrEmpty(activeBitmapID))
            {
                ImageBrush brush = (ImageBrush)brushes[activeBitmapID];
                if (brush != null && brush.BitmapSource != null)
                    dc.DrawRectangle(brush, null, 0, 0, ActualWidth, ActualHeight);
            }
        }
        #endregion
    }
}
