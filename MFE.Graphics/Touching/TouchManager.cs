using MFE.Core;
using Microsoft.SPOT;
using Microsoft.SPOT.Touch;

namespace MFE.Graphics.Touching
{
    public static class TouchManager
    {
        private class TouchListener : IEventListener
        {
            enum TouchMessages : byte
            {
                Down = 1,
                Up = 2,
                Move = 3,
            }

            public void InitializeForEventSource()
            {
            }
            public bool OnEvent(BaseEvent baseEvent)
            {
                if (baseEvent is TouchEvent)
                {
                    TouchEvent e = (TouchEvent)baseEvent;
                    switch (e.EventMessage)
                    {
                        case (byte)TouchMessages.Down: HandleTouchDownEvent(e); break;
                        case (byte)TouchMessages.Move: HandleTouchMoveEvent(e); break;
                        case (byte)TouchMessages.Up: HandleTouchUpEvent(e); break;
                    }
                }
                else if (baseEvent is GenericEvent)
                {
                    GenericEvent e = (GenericEvent)baseEvent;
                    if (e.EventCategory == (byte)EventCategory.Gesture)
                    {
                        TouchGestureEventArgs args = new TouchGestureEventArgs((TouchGesture)e.EventMessage, e.X, e.Y, (ushort)e.EventData, e.Time);
                        switch (args.Gesture)
                        {
                            case TouchGesture.Begin: OnTouchGestureStarted(args); break;
                            case TouchGesture.End: OnTouchGestureEnded(args); break;
                            default: OnTouchGestureChanged(args); break;
                        }
                    }
                }

                return true;
            }

            private void HandleTouchDownEvent(TouchEvent e)
            {
                lastTouchX = e.Touches[0].X;
                lastTouchY = e.Touches[0].Y;
                OnTouchDown(new TouchEventArgs(e.Touches, e.Time));
            }
            private void HandleTouchMoveEvent(TouchEvent e)
            {
                if (e.Touches[0].X != lastTouchX || e.Touches[0].Y != lastTouchY)
                {
                    lastTouchX = e.Touches[0].X;
                    lastTouchY = e.Touches[0].Y;
                    //Debug.Print(lastTouchX.ToString() + "; " + lastTouchY.ToString());
                    OnTouchMove(new TouchEventArgs(e.Touches, e.Time));
                }
            }
            private void HandleTouchUpEvent(TouchEvent e)
            {
                OnTouchUp(new TouchEventArgs(e.Touches, e.Time));
            }
        }

        #region Fields
        private static TouchListener touchListener;
        private static int lastTouchX;
        private static int lastTouchY;
        #endregion

        #region Events
        public static event TouchEventHandler TouchDown;
        public static event TouchEventHandler TouchUp;
        public static event TouchEventHandler TouchMove;
        public static event TouchGestureEventHandler TouchGestureStarted;
        public static event TouchGestureEventHandler TouchGestureChanged;
        public static event TouchGestureEventHandler TouchGestureEnded;
        #endregion

        #region Public methods
        public static void Initialize()
        {
            //new Thread(delegate()
            //{
                touchListener = new TouchListener();
                Touch.Initialize(touchListener);

                TouchCollectorConfiguration.CollectionMode = CollectionMode.GestureOnly;
                TouchCollectorConfiguration.CollectionMethod = CollectionMethod.Native;
                TouchCollectorConfiguration.SamplingFrequency = 50; // 50...200; default 100; best 50; worse 200;
                if (!Utils.IsEmulator)
                {
                    TouchCollectorConfiguration.TouchMoveFrequency = 20;// ... // in ms; default 50;
                }

            //    Thread.Sleep(-1);
            //}
            //).Start();
        }
        #endregion

        #region Private methods
        private static void OnTouchDown(TouchEventArgs e)
        {
            if (TouchDown != null)
                TouchDown(null, e);
        }
        private static void OnTouchUp(TouchEventArgs e)
        {
            if (TouchUp != null)
                TouchUp(null, e);
        }
        private static void OnTouchMove(TouchEventArgs e)
        {
            if (TouchMove != null)
                TouchMove(null, e);
        }
        private static void OnTouchGestureStarted(TouchGestureEventArgs e)
        {
            if (TouchGestureStarted != null)
                TouchGestureStarted(null, e);
        }
        private static void OnTouchGestureChanged(TouchGestureEventArgs e)
        {
            if (TouchGestureChanged != null)
                TouchGestureChanged(null, e);
        }
        private static void OnTouchGestureEnded(TouchGestureEventArgs e)
        {
            if (TouchGestureEnded != null)
                TouchGestureEnded(null, e);
        }
        #endregion
    }
}
