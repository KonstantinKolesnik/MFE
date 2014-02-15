
namespace MFE.Graphics.Media
{
    public class Pen
    {
        public Color Color;
        public ushort Thickness;

        public Pen(Color color)
            : this(color, 1)
        {
        }
        public Pen(Color color, ushort thickness)
        {
            Color = color;
            Thickness = thickness;
        }
    }
}
