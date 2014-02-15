
namespace MFE.Graphics.Geometry
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point Zero
        {
            get { return new Point(0, 0); }
        }

        public static bool operator != (Point a, Point b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        public static bool operator == (Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static Point operator + (Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
        public static Point operator - (Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Point))
                return false;
            else
                return this == ((Point)obj);
        }
    }
}
