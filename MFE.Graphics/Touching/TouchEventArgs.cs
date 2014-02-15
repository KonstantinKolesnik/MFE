using System;
using MFE.Graphics.Geometry;
using Microsoft.SPOT.Touch;

namespace MFE.Graphics.Touching
{
    public class TouchEventArgs
    {
        /// <summary>
        /// Time the event occured.
        /// </summary>
        public readonly DateTime Timestamp;

        //public readonly TouchInput[] Touches; // needed???

        /// <summary>
        /// The point of contact.
        /// </summary>
        public readonly Point Point;

        public TouchEventArgs(TouchInput[] touches, DateTime timestamp)
        {
            //Touches = touches;
            Timestamp = timestamp;
            Point = new Point(touches[0].X, touches[0].Y);
        }

        ///// <summary>
        ///// Creates a new TouchEventArgs.
        ///// </summary>
        ///// <param name="point">Point</param>
        //public TouchEventArgs(Point point)
        //{
        //    //Point = point;
        //}


        ///// <summary>
        ///// Stops propagation.
        ///// </summary>
        //public void StopPropagation()
        //{
        //    Propagate = false;
        //}

        //public void GetPosition(UIElement relativeTo, int touchIndex, out int x, out int y)
        //{
        //    x = Touches[touchIndex].X;
        //    y = Touches[touchIndex].Y;

        //    relativeTo.PointToClient(ref x, ref y);
        //}
    }
}
