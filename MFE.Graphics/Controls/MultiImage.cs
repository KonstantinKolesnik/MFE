using MFE.Core;
using MFE.Graphics.Media;
using System.Collections;

namespace MFE.Graphics.Controls
{
    public class MultiImage : Control
    {
        #region Fields
        private Hashtable brushes;
        private string activeBrushID;
        #endregion

        #region Properties
        public Brush this[string brushID]
        {
            get
            {
                if (!brushes.Contains(brushID) || (Brush)brushes[brushID] == null)
                    return null;
                else
                    return (Brush)brushes[brushID];
            }
            set
            {
                if (brushes.Contains(brushID))
                    brushes[brushID] = value;
                else
                    brushes.Add(brushID, value);
            }
        }
        public string ActiveBrushID
        {
            get { return activeBrushID; }
            set
            {
                if (activeBrushID != value)
                {
                    activeBrushID = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructors
        public MultiImage(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            brushes = new Hashtable();
        }
        #endregion

        #region Event handlers
        public override void OnRender(DrawingContext dc)
        {
            if (!Utils.StringIsNullOrEmpty(activeBrushID))
            {
                Brush brush = (Brush)brushes[activeBrushID];
                if (brush != null)
                    dc.DrawRectangle(brush, null, 0, 0, Width, Height);
            }
        }
        #endregion
    }
}
