
namespace MFE.Graphics.Media
{
    public static class ColorUtils
    {
        public static Color ColorFromRGB(byte r, byte g, byte b)
        {
            return (Color)((b << 16) | (g << 8) | r);
        }
        public static byte GetRValue(Color color)
        {
            return (byte)((uint)color & 0xff);
        }
        public static byte GetGValue(Color color)
        {
            return (byte)(((uint)color >> 8) & 0xff);
        }
        public static byte GetBValue(Color color)
        {
            return (byte)(((uint)color >> 16) & 0xff);
        }
    }
}
