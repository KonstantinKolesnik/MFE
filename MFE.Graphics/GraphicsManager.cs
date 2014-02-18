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
        internal Desktop desktop;
        //internal Window modalWindow = null;

        private Control lastEventTarget = null;
        //var lastMoveTarget = null;
        //var lastP = new Point(-1, -1);
        //var lastTouchTarget = null;
        //var touchRepeatCount = 0;
        //var lastDownTarget = null;

        //public DateTime dt0;
        //public TimeSpan ts;
        #endregion

        #region Properties
        public Desktop Desktop
        {
            get { return desktop; }
        }
        #endregion

        #region Events
        public event RenderEventHandler OnRender;
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
            desktop.Invalidate();

            //if (CalibrationManager.IsCalibrated)
            //    CalibrationManager.ApplyCalibrationPoints();
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
        #endregion

        #region Private methods
        private Control GetTouchTarget(Point p)
        {
            Control res = null;
            if (TouchCapture.Captured != null)
                res = TouchCapture.Captured;
            else if (lastEventTarget != null)
            {
                var par = lastEventTarget.GetValidParentFromScreenPoint(p);
                res = par != null ? par.GetValidChildFromScreenPoint(p) : (desktop != null ? desktop.GetValidChildFromScreenPoint(p) : null);
            }
            else
                res = desktop != null ? desktop.GetValidChildFromScreenPoint(p) : null;

            lastEventTarget = res;
            return res;


            //Control res = null;
            ////dt0 = DateTime.Now;
            //if (TouchCapture.Captured != null)
            //    res = TouchCapture.Captured;
            //else if (modalWindow != null)
            //    res = modalWindow is CalibrationWindow ? modalWindow : FindTouchTarget(modalWindow, p);
            //else
            //    res = FindTouchTarget(Desktop, p);
            //lastEventTarget = res;
            ////ts = DateTime.Now - dt0;
            //return res;
        }
        //private Control FindTouchTarget(Control root, Point p)
        //{
        //    if (lastEventTarget != null)
        //    {
        //        Control target = lastEventTarget.GetValidChildFromScreenPoint(p);
        //        if (target != null)
        //            return target;
                
        //        Control par = lastEventTarget.GetValidParentFromScreenPoint(p);
        //        return par.GetValidChildFromScreenPoint(p);
        //    }
        //    else
        //        return root.GetValidChildFromScreenPoint(p);
        //}

        //function InterceptMouseDblClick(p, isIPad) {
        //    var eventTarget = getEventTarget(p);
        //    if (eventTarget)
        //        eventTarget.OnMouseDblClick(p);
        //}
        //function InterceptMouseDown(p, isIPad) {        
        //    var eventTarget = getEventTarget(p);

        //    if (isIPad && eventTarget) { //AB 2013/0416 Git#137 override iPad hover to clicks
        //        if (eventTarget.Name == "btnExpand" || eventTarget.Name == "btnActionMenu")
        //            eventTarget.OnMouseClick(p, isIPad);
        //        if (eventTarget.Name == "chbComplete") 
        //            eventTarget.OnMouseUp(p, isIPad);
        //    }

        //    if (eventTarget) {
        //        // check for mouseenter for touched control:
        //        if (!lastTouchTarget) { // from none to eventTarget
        //            eventTarget.OnMouseEnter(p, isIPad);
        //            lastTouchTarget = eventTarget;
        //        }
        //        else { // from lastTouchTarget to eventTarget
        //            if (lastTouchTarget == eventTarget) { // within the same control;
        //                eventTarget.OnMouseMove(p, isIPad);
        //            }
        //            else if (eventTarget.GetParent() == lastTouchTarget) { // from parent to child; don't raise mouseLeave for parent; just mouseEnter for child
        //                lastTouchTarget.OnMouseLeaveToChild(p, isIPad);
        //                eventTarget.OnMouseEnter(p, isIPad);
        //                lastTouchTarget = eventTarget;
        //            }
        //            else if (lastTouchTarget.GetParent() == eventTarget) { // from child to parent; raise mouseEnter for parent and mouseLeave for child
        //                lastTouchTarget.OnMouseLeave(p, isIPad);
        //                eventTarget.OnMouseEnter(p, isIPad);
        //                lastTouchTarget = eventTarget;
        //            }
        //            else { // between different controls; raise mouseLeave for the previous one and mouseEnter for the new one
        //                raiseMouseLeaveForHierarchy(lastTouchTarget, eventTarget, p, isIPad);
        //                eventTarget.OnMouseEnter(p, isIPad);
        //                lastTouchTarget = eventTarget;
        //            }
        //        }

        //        // then anyway raise mousedown event for control:
        //        lastDownTarget = eventTarget;
        //        eventTarget.OnMouseDown(p, isIPad);
        //    }
        //    else { // touched outside the root control, raise mouseleave for last touched control & all its parents:
        //        if (lastTouchTarget) {
        //            raiseMouseLeaveForHierarchy(lastTouchTarget, lastTouchTarget.GetRootControl(), p, isIPad);
        //            lastTouchTarget = null;
        //            lastDownTarget = null;
        //        }
        //    }
        //}
        //function InterceptMouseUp(p, isIPad) {
        //    //alert(p.X + " / " + p.Y);
        //    var eventTarget = getEventTarget(p);
        //    if (eventTarget) {
        //        eventTarget.OnMouseUp(p, isIPad);
        //        if (lastDownTarget == eventTarget) {
        //            //debugOutput("click " + eventTarget.Name);
        //            //alert("click " + eventTarget.Name);
        //            eventTarget.OnMouseClick(p, isIPad);

        //            //                if (isIPad) {
        //            //                    touchRepeatCount++;
        //            //                    if (touchRepeatCount == 1) { // first click
        //            //                        var delay = 300;
        //            //                        setTimeout(function afterDelay() {
        //            //                            if (touchRepeatCount > 1) // second click occured within delay
        //            //                                eventTarget.OnMouseDblClick(p, isIPad);

        //            //                            touchRepeatCount = 0;
        //            //                        }, delay);
        //            //                    }
        //            //                }
        //        }
        //    }
        //    lastDownTarget = null;
        //}
        //function InterceptMouseMove(p, isIPad) {
        //    //startMeasure();
        //    var eventTarget = getEventTarget(p);
        //    if (eventTarget) {
        //        if (!lastMoveTarget) { // from none to eventTarget
        //            lastMoveTarget = eventTarget;
        //            eventTarget.OnMouseEnter(p, isIPad);
        //        }
        //        else { // from lastMoveTarget to eventTarget
        //            if (lastMoveTarget == eventTarget) // within the same control; just mouseMove event
        //                eventTarget.OnMouseMove(p, isIPad);
        //            else if (eventTarget.GetParent() == lastMoveTarget) { // from parent to child; don't raise mouseLeave for parent; just mouseEnter for child
        //                lastMoveTarget.OnMouseLeaveToChild(p, isIPad);
        //                eventTarget.OnMouseEnter(p, isIPad);
        //                lastMoveTarget = eventTarget;
        //            }
        //            else if (lastMoveTarget.GetParent() == eventTarget) { // from child to parent; don't raise mouseEnter for parent; just mouseLeave for child
        //                lastMoveTarget.OnMouseLeave(p, isIPad);
        //                lastMoveTarget = eventTarget;
        //            }
        //            else { // between different controls; raise mouseLeave for the previous one and mouseEnter for the new one
        //                //lastMoveTarget.OnMouseLeave(p, isIPad);
        //                raiseMouseLeaveForHierarchy(lastMoveTarget, eventTarget, p, isIPad);

        //                eventTarget.OnMouseEnter(p, isIPad);
        //                lastMoveTarget = eventTarget;
        //            }
        //        }
        //    }
        //    else {
        //        if (lastMoveTarget) { // from lastMoveTarget to none
        //            //lastMoveTarget.OnMouseLeave(p, isIPad);
        //            raiseMouseLeaveForHierarchy(lastMoveTarget, lastMoveTarget.GetRootControl(), p, isIPad);
        //            lastMoveTarget = null;
        //        }
        //    }
        //    //stopMeasure();
        //    //t1 = payload;

        //}
        //function InterceptMouseOut(p, isIPad) {
        //    var captured = capturer.GetCaptured();
        //    if (captured)
        //        captured.OnMouseUp(p, isIPad);
        //    if (lastEventTarget) {
        //        lastEventTarget.OnMouseLeave(p, isIPad);
        //        lastEventTarget = null;
        //    }
        //}

        //function raiseMouseLeaveForHierarchy(ctrl1, ctrl2, p, isIPad) {
        //    var p = null;
        //    var pp = ctrl1;
        //    do {
        //        p = pp;
        //        p.OnMouseLeave(p, isIPad);
        //        pp = p.GetParent();
        //    } while (pp && pp.GetParent() != ctrl2.GetParent());
        //}

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

            //dt0 = DateTime.Now;
            var dc = new DrawingContext(bitmap);
            dc.PushClippingRectangle(dirtyRect);
            if (!dc.ClippingRectangle.IsZero)
            {
                desktop.RenderRecursive(dc);
                desktop.PostRenderRecursive(dc);
            }
            dc.PopClippingRectangle();
            dc.Close();
            //ts = DateTime.Now - dt0;

            if (OnRender != null)
                OnRender(bitmap, dirtyRect);
            else
                bitmap.Flush(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);


            //Rect dirtyRect2 = new Rect(0, 0, 300, 20);
            //dc = new DrawingContext(screen);
            //dc.PushClippingRectangle(dirtyRect2);
            //Font font = Resources.GetFont(Resources.FontResources.Regular);
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

/*
Bitmap Clear, 800x480: 00:00:00.0120309
Bitmap Flush, 800x480: 00:00:00.0371465
Bitmap GetPixel, 800x480: 00:00:00.0003250
Bitmap SetPixel, 800x480: 00:00:00.0003901
Bitmap Blend a=256, 800x480: 00:00:00.0399479
Bitmap Blend a=127, 800x480: 00:00:01.2195883
Bitmap Blend a=10, 800x480: 00:00:01.2195683
Bitmap Blend a=0, 800x480: 00:00:00.0006304
Bitmap DrawImage 1, 800x480: 00:00:00.0400499
Bitmap DrawImage 2, 800x480: 00:00:00.0403511
Bitmap DrawImage 2, 200x200, center: 00:00:00.0046052
Bitmap DrawImage 2, 20x20, center: 00:00:00.0006930
Bitmap DrawRectangle, 800x480: 00:00:00.1247743
new Bitmap: 00:00:00.0117430
Bitmap Dispose: 00:00:00.0003555
*/
