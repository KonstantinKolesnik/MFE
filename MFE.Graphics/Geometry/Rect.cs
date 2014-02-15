
namespace MFE.Graphics.Geometry
{
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static Rect Empty
        {
            get { return new Rect(0, 0, 0, 0); }
        }
        public bool IsZero
        {
            get { return Width <= 0 || Height <= 0; }
        }
        public int Right
        {
            get { return X + Width - 0; }
        }
        public int Bottom
        {
            get { return Y + Height - 0; }
        }

        public static bool operator != (Rect a, Rect b)
        {
            return a.X != b.X || a.Y != b.Y || a.Width != b.Width || a.Height != b.Height;
        }
        public static bool operator == (Rect a, Rect b)
        {
            return !(a != b);
        }
        public override string ToString()
        {
            return "Rect: X=" + X + " Y=" + Y + " W=" + Width + " H=" + Height;
        }

        public bool Contains(int x, int y)
        {
            //return (x >= X && x <= Right && y >= Y && y <= Bottom);
            return Intersects(new Rect(x, y, 1, 1));

            //if (x < X)
            //    return false;
            //else if (x > Right)
            //    return false;
            //else if (y < Y)
            //    return false;
            //else if (y > Bottom)
            //    return false;
            //else
            //    return true;
        }
        public bool Contains(Point e)
        {
            return Contains(e.X, e.Y);
        }
        public void Combine(Rect newRect)
        {
            if (IsZero)
            {
                X = newRect.X;
                Y = newRect.Y;
                Width = newRect.Width;
                Height = newRect.Height;
                return;
            }

            if (this == newRect)
                return;

            int x1, y1, x2, y2;
            x1 = X < newRect.X ? X : newRect.X;
            y1 = Y < newRect.Y ? Y : newRect.Y;
            x2 = Right > newRect.Right ? Right : newRect.Right;
            y2 = Bottom > newRect.Bottom ? Bottom : newRect.Bottom;

            X = x1;
            Y = y1;
            Width = x2 - x1;
            Height = y2 - y1;
        }
        public bool Intersects(Rect rect)
        {
            //DateTime dt = DateTime.Now;
            //bool res = !(rect.X > Right || rect.Right < X || rect.Y > Bottom || rect.Bottom < Y);

            bool res;
            if (rect.X > Right)
                res = false;
            else if (rect.Right < X)
                res = false;
            else if (rect.Y > Bottom)
                res = false;
            else if (rect.Bottom < Y)
                res = false;
            else
                res = true;
            
            //TimeSpan ts = DateTime.Now - dt;
            //Debug.Print("Rect::Intersects: " + ts.ToString());
            return res;
        }
        public Rect Intersection(Rect rect)
        {
            if (!Intersects(rect))
                return Rect.Empty;

            //DateTime dt = DateTime.Now;
            int x1 = rect.X > X ? rect.X : X;
            int x2 = rect.Right < Right ? rect.Right : Right;
            int y1 = rect.Y > Y ? rect.Y : Y;
            int y2 = rect.Bottom < Bottom ? rect.Bottom : Bottom;

            //TimeSpan ts = DateTime.Now - dt;
            //Debug.Print("Rect::Intersection: " + ts.ToString());

            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }
        public void Translate(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        public static Rect Combine(Rect rect1, Rect rect2)
        {
            rect1.Combine(rect2);
            return rect1;
        }
        public static Rect Intersection(Rect rect1, Rect rect2)
        {
            return rect1.Intersection(rect2);
        }
    }
}
