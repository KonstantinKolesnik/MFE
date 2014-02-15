using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;

namespace MFE.Graphics
{
    class RenderTask
    {
        public Control Sender;
        public Rect DirtyArea;

        public RenderTask(Control sender, Rect dirtyArea)
        {
            Sender = sender;
            DirtyArea = dirtyArea;
        }
    }
}
