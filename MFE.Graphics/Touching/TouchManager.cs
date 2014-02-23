using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
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

            private int lastTouchX;
            private int lastTouchY;
            private bool lastMoveEvent = false;

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
                            case TouchGesture.Begin: NotifyTouchGestureStarted(args); break;
                            case TouchGesture.End: NotifyTouchGestureEnded(args); break;
                            default: NotifyTouchGestureChanged(args); break;
                        }
                    }
                }

                return true;
            }

            private void HandleTouchDownEvent(TouchEvent e)
            {
                lastMoveEvent = false;
                NotifyTouchDown(new TouchEventArgs(e.Touches, e.Time));
            }
            private void HandleTouchMoveEvent(TouchEvent e)
            {
                if (lastMoveEvent && (e.Touches[0].X == lastTouchX) && (e.Touches[0].Y == lastTouchY))
                {
                }
                else
                {
                    lastMoveEvent = true;
                    lastTouchX = e.Touches[0].X;
                    lastTouchY = e.Touches[0].Y;
                    NotifyTouchMove(new TouchEventArgs(e.Touches, e.Time));
                }
            }
            private void HandleTouchUpEvent(TouchEvent e)
            {
                lastMoveEvent = false;
                NotifyTouchUp(new TouchEventArgs(e.Touches, e.Time));
            }
        }

        #region Fields
        private static TouchListener touchListener = null;
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
            if (touchListener == null)
            {
                touchListener = new TouchListener();
                Touch.Initialize(touchListener);

                TouchCollectorConfiguration.CollectionMode = CollectionMode.InkAndGesture;
                TouchCollectorConfiguration.CollectionMethod = CollectionMethod.Native;
                TouchCollectorConfiguration.SamplingFrequency = 50; // 50...200; default 50; best 50; worse 200;
                //if (SystemInfo.SystemID.SKU != 3)//!Utils.IsEmulator)
                //    TouchCollectorConfiguration.TouchMoveFrequency = 20; // in ms; default 20;
            }
        }
        #endregion

        #region Private event notifyers
        private static void NotifyTouchDown(TouchEventArgs e)
        {
            if (TouchDown != null)
                TouchDown(null, e);
        }
        private static void NotifyTouchUp(TouchEventArgs e)
        {
            if (TouchUp != null)
                TouchUp(null, e);
        }
        private static void NotifyTouchMove(TouchEventArgs e)
        {
            if (TouchMove != null)
                TouchMove(null, e);
        }
        private static void NotifyTouchGestureStarted(TouchGestureEventArgs e)
        {
            if (TouchGestureStarted != null)
                TouchGestureStarted(null, e);
        }
        private static void NotifyTouchGestureChanged(TouchGestureEventArgs e)
        {
            if (TouchGestureChanged != null)
                TouchGestureChanged(null, e);
        }
        private static void NotifyTouchGestureEnded(TouchGestureEventArgs e)
        {
            if (TouchGestureEnded != null)
                TouchGestureEnded(null, e);
        }
        #endregion
    }
}
