using MFE.Graphics.Calibration;
using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using Microsoft.SPOT;

namespace MFE.Graphics
{
    public class GraphicsManager
    {
        #region Fields
        private Bitmap bitmap;
        private Desktop desktop;
        private CalibrationWindow cw;
        //private Keyboard keyboard;
        //internal Window modalWindow = null;

        private Control lastEventTarget = null;
        //var lastMoveTarget = null;
        //var lastP = new Point(-1, -1);
        //var lastTouchTarget = null;
        //var touchRepeatCount = 0;
        //var lastDownTarget = null;

        //private DateTime dt;
        //private TimeSpan ts;
        #endregion

        #region Properties
        public bool IsTrial
        {
            get;
            set;
        }
        public Desktop Desktop
        {
            get { return desktop; }
        }
        public bool IsCalibrated
        {
            get { return CalibrationManager.IsCalibrated; }
        }
        public CalibrationWindow CalibrationWindow
        {
            get { return cw; }
        }
        #endregion

        #region Events
        public event RenderRequestEventHandler OnRenderRequest;
        #endregion

        #region Constructor
        public GraphicsManager(int width, int height)
        {
            //mouseManager = new MouseManager(canvas);
            //mouseManager.OnMouseDblClick = InterceptMouseDblClick;
            //mouseManager.OnMouseDown = InterceptMouseDown;
            //mouseManager.OnMouseUp = InterceptMouseUp;
            //mouseManager.OnMouseMove = InterceptMouseMove;
            //mouseManager.OnMouseOut = InterceptMouseOut;

            TouchManager.TouchDown += new TouchEventHandler(TouchManager_TouchDown);
            TouchManager.TouchMove += new TouchEventHandler(TouchManager_TouchMove);
            TouchManager.TouchUp += new TouchEventHandler(TouchManager_TouchUp);
            TouchManager.TouchGestureStarted += new TouchGestureEventHandler(TouchManager_TouchGestureStarted);
            TouchManager.TouchGestureChanged += new TouchGestureEventHandler(TouchManager_TouchGestureChanged);
            TouchManager.TouchGestureEnded += new TouchGestureEventHandler(TouchManager_TouchGestureEnded);
            TouchManager.Initialize();

            bitmap = new Bitmap(width, height);
            desktop = new Desktop(width, height, this);
            cw = new CalibrationWindow(width, height, this)
            {
                Background = new SolidColorBrush(Color.CornflowerBlue),
                CrosshairPen = new Pen(Color.Red, 1)
            };

            if (CalibrationManager.IsCalibrated)
                CalibrationManager.ApplyCalibrationPoints();

            desktop.Invalidate();
        }
        #endregion

        #region Touch event handlers
        private void TouchManager_TouchDown(object sender, TouchEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchDownEvent(e);
        }
        private void TouchManager_TouchMove(object sender, TouchEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchMoveEvent(e);
        }
        private void TouchManager_TouchUp(object sender, TouchEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchUpEvent(e);
        }
        private void TouchManager_TouchGestureStarted(object sender, TouchGestureEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchGestureStartedEvent(e);
        }
        private void TouchManager_TouchGestureChanged(object sender, TouchGestureEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchGestureChangedEvent(e);
        }
        private void TouchManager_TouchGestureEnded(object sender, TouchGestureEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchGestureEndedEvent(e);
        }

        //function InterceptMouseDblClick(p) {
        //    var eventTarget = getEventTarget(p);
        //    if (eventTarget)
        //        eventTarget.OnMouseDblClick(p);
        //}
        //function InterceptMouseDown(p) {        
        //    var eventTarget = getEventTarget(p);

        //    if (isIPad && eventTarget) { //AB 2013/0416 Git#137 override iPad hover to clicks
        //        if (eventTarget.Name == "btnExpand" || eventTarget.Name == "btnActionMenu")
        //            eventTarget.OnMouseClick(p);
        //        if (eventTarget.Name == "chbComplete") 
        //            eventTarget.OnMouseUp(p);
        //    }

        //    if (eventTarget) {
        //        // check for mouseenter for touched control:
        //        if (!lastTouchTarget) { // from none to eventTarget
        //            eventTarget.OnMouseEnter(p);
        //            lastTouchTarget = eventTarget;
        //        }
        //        else { // from lastTouchTarget to eventTarget
        //            if (lastTouchTarget == eventTarget) { // within the same control;
        //                eventTarget.OnMouseMove(p);
        //            }
        //            else if (eventTarget.GetParent() == lastTouchTarget) { // from parent to child; don't raise mouseLeave for parent; just mouseEnter for child
        //                lastTouchTarget.OnMouseLeaveToChild(p);
        //                eventTarget.OnMouseEnter(p);
        //                lastTouchTarget = eventTarget;
        //            }
        //            else if (lastTouchTarget.GetParent() == eventTarget) { // from child to parent; raise mouseEnter for parent and mouseLeave for child
        //                lastTouchTarget.OnMouseLeave(p);
        //                eventTarget.OnMouseEnter(p);
        //                lastTouchTarget = eventTarget;
        //            }
        //            else { // between different controls; raise mouseLeave for the previous one and mouseEnter for the new one
        //                raiseMouseLeaveForHierarchy(lastTouchTarget, eventTarget, p);
        //                eventTarget.OnMouseEnter(p);
        //                lastTouchTarget = eventTarget;
        //            }
        //        }

        //        // then anyway raise mousedown event for control:
        //        lastDownTarget = eventTarget;
        //        eventTarget.OnMouseDown(p);
        //    }
        //    else { // touched outside the root control, raise mouseleave for last touched control & all its parents:
        //        if (lastTouchTarget) {
        //            raiseMouseLeaveForHierarchy(lastTouchTarget, lastTouchTarget.GetRootControl(), p);
        //            lastTouchTarget = null;
        //            lastDownTarget = null;
        //        }
        //    }
        //}
        //function InterceptMouseUp(p) {
        //    //alert(p.X + " / " + p.Y);
        //    var eventTarget = getEventTarget(p);
        //    if (eventTarget) {
        //        eventTarget.OnMouseUp(p);
        //        if (lastDownTarget == eventTarget) {
        //            //debugOutput("click " + eventTarget.Name);
        //            //alert("click " + eventTarget.Name);
        //            eventTarget.OnMouseClick(p);

        //            //                if (isIPad) {
        //            //                    touchRepeatCount++;
        //            //                    if (touchRepeatCount == 1) { // first click
        //            //                        var delay = 300;
        //            //                        setTimeout(function afterDelay() {
        //            //                            if (touchRepeatCount > 1) // second click occured within delay
        //            //                                eventTarget.OnMouseDblClick(p);

        //            //                            touchRepeatCount = 0;
        //            //                        }, delay);
        //            //                    }
        //            //                }
        //        }
        //    }
        //    lastDownTarget = null;
        //}
        //function InterceptMouseMove(p) {
        //    //startMeasure();
        //    var eventTarget = getEventTarget(p);
        //    if (eventTarget) {
        //        if (!lastMoveTarget) { // from none to eventTarget
        //            lastMoveTarget = eventTarget;
        //            eventTarget.OnMouseEnter(p);
        //        }
        //        else { // from lastMoveTarget to eventTarget
        //            if (lastMoveTarget == eventTarget) // within the same control; just mouseMove event
        //                eventTarget.OnMouseMove(p);
        //            else if (eventTarget.GetParent() == lastMoveTarget) { // from parent to child; don't raise mouseLeave for parent; just mouseEnter for child
        //                lastMoveTarget.OnMouseLeaveToChild(p);
        //                eventTarget.OnMouseEnter(p);
        //                lastMoveTarget = eventTarget;
        //            }
        //            else if (lastMoveTarget.GetParent() == eventTarget) { // from child to parent; don't raise mouseEnter for parent; just mouseLeave for child
        //                lastMoveTarget.OnMouseLeave(p);
        //                lastMoveTarget = eventTarget;
        //            }
        //            else { // between different controls; raise mouseLeave for the previous one and mouseEnter for the new one
        //                //lastMoveTarget.OnMouseLeave(p);
        //                raiseMouseLeaveForHierarchy(lastMoveTarget, eventTarget, p);

        //                eventTarget.OnMouseEnter(p);
        //                lastMoveTarget = eventTarget;
        //            }
        //        }
        //    }
        //    else {
        //        if (lastMoveTarget) { // from lastMoveTarget to none
        //            //lastMoveTarget.OnMouseLeave(p);
        //            raiseMouseLeaveForHierarchy(lastMoveTarget, lastMoveTarget.GetRootControl(), p);
        //            lastMoveTarget = null;
        //        }
        //    }
        //    //stopMeasure();
        //    //t1 = payload;

        //}
        //function InterceptMouseOut(p) {
        //    var captured = capturer.GetCaptured();
        //    if (captured)
        //        captured.OnMouseUp(p);
        //    if (lastEventTarget) {
        //        lastEventTarget.OnMouseLeave(p);
        //        lastEventTarget = null;
        //    }
        //}

        //function raiseMouseLeaveForHierarchy(ctrl1, ctrl2, p) {
        //    var p = null;
        //    var pp = ctrl1;
        //    do {
        //        p = pp;
        //        p.OnMouseLeave(p);
        //        pp = p.GetParent();
        //    } while (pp && pp.GetParent() != ctrl2.GetParent());
        //}
        #endregion

        #region Private methods
        private Control GetTouchTarget(Point p)
        {
            Control result = null;

            //dt = DateTime.Now;
            if (TouchCapture.Captured != null)
                result = TouchCapture.Captured;
            else if (desktop.Children.Contains(cw))
                return cw;
            //else if (modalWindow != null)
            //    res = FindTouchTarget(modalWindow, p);
            else
                result = FindTouchTarget(desktop, p);
            //ts = DateTime.Now - dt;

            lastEventTarget = result;
            return result;
        }
        private Control FindTouchTarget(Control root, Point p)
        {
            return root.GetValidChildFromScreenPoint(p);

            if (lastEventTarget != null)
            {
                // old:
                Control target = lastEventTarget.GetValidChildFromScreenPoint(p);
                if (target != null)
                    return target;
                else if (lastEventTarget.ContainsScreenPoint(p))
                    return lastEventTarget;
                else
                {
                    Control par = lastEventTarget.GetValidParentFromScreenPoint(p);
                    return par != null ? par.GetValidChildFromScreenPoint(p) : (root != null ? root.GetValidChildFromScreenPoint(p) : null);
                }

                // new:
                //var par = lastEventTarget.GetValidParentFromScreenPoint(p);
                //return par != null ? par.GetValidChildFromScreenPoint(p) : (root != null ? root.GetValidChildFromScreenPoint(p) : null);
            }
            else
                return root.GetValidChildFromScreenPoint(p);
        }

        internal void ProcessTask(RenderTask task /*optional*/)
        {
            if (desktop != null && bitmap != null)
                RenderTask(task);
        }
        private void RenderTask(RenderTask task)
        {
            Rect contentRect = new Rect(0, 0, desktop.Width, desktop.Height);
            var dirtyRect = contentRect;
            if (task != null)
                dirtyRect = task.DirtyArea.Intersection(contentRect);
            if (dirtyRect.IsZero)
                return;

            //dt = DateTime.Now;
            var dc = new DrawingContext(bitmap);
            dc.PushClippingRectangle(dirtyRect);
            if (!dc.ClippingRectangle.IsZero)
            {
                desktop.RenderRecursive(dc);
                desktop.PostRenderRecursive(dc);
            }
            dc.PopClippingRectangle();
            dc.Close();
            //ts = DateTime.Now - dt;

            if (OnRenderRequest != null)
                OnRenderRequest(bitmap, dirtyRect);
            else
                bitmap.Flush(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);


            //Rect dirtyRect2 = new Rect(0, 0, 300, 20);
            //dc = new DrawingContext(screen);
            //dc.PushClippingRectangle(dirtyRect2);
            //Font font = Resources.GetFont(Resources.FontResources.CourierNew_10);
            //dc.DrawRectangle(new SolidColorBrush(Color.White), null, 0, 0, 300, 20);
            //dc.DrawText(dirtyRect.ToString() + "; Render: " + ts.ToString(), font, Color.Red, 3, 3);
            //dc.PopClippingRectangle();
            //dc.Close();
            //screen.Flush(dirtyRect2.X, dirtyRect2.Y, dirtyRect2.Width, dirtyRect2.Height);
            //Debug.Print("Render task: " + ts.ToString());
        }
        #endregion
    }
}
