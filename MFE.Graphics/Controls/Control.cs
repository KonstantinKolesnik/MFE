using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;

namespace MFE.Graphics.Controls
{
    #region Event Delegates
    //[Serializable]
    //public delegate void OnTap(object sender, Point e);

    //[Serializable]
    //public delegate void OnTapHold(object sender, Point e);

    //[Serializable]
    //public delegate void OnFormTap(Point e);

    //[Serializable]
    //public delegate void OnNodeTap(TreeviewNode node, Point e);

    //[Serializable]
    //public delegate void OnNodeExpanded(object sender, TreeviewNode node);

    //[Serializable]
    //public delegate void OnNodeCollapsed(object sender, TreeviewNode node);

    //[Serializable]
    //public delegate void OnSelectedIndexChange(object sender, int index);

    //[Serializable]
    //public delegate void OnSelectedFileChanged(object sender, string path);

    //[Serializable]
    //public delegate void OnTextChanged(object sender);

    //[Serializable]
    //public delegate void OnVirtualKeyboardClosed(object sender);

    //[Serializable]
    //public delegate void OnValueChanged(object sender, int value);
    #endregion

    public abstract class Control
    {
        #region Fields
        private Control parent = null;
        protected internal ControlCollection children;
        private Rect area; // in client coordinates
        private Rect dirtyArea = Rect.Empty; // in screen coordinates
        private bool isEnabled = true;
        private bool isVisible = true;
        private bool isSuspended = false;
        #endregion

        #region  Properties
        public string Name
        {
            get;
            set;
        }
        public object Tag
        {
            get;
            set;
        }
        public virtual Control Parent
        {
            get { return parent; }
            internal set
            {
                if (parent != value)
                {
                    parent = value;
                    Invalidate();
                }
            }
        }
        public virtual ControlCollection Children
        {
            get { return children; }
        }
        public virtual bool IsEnabled
        {
            get
            {
                return parent != null ? isEnabled && parent.IsEnabled : isEnabled;
            }
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    if (isVisible)
                        Invalidate();
                    else if (parent != null)
                        parent.Invalidate();
                }
            }
        }
        public virtual bool Visible
        {
            get
            {
                return isVisible;
            }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    Invalidate();
                }
            }
        }
        public virtual Rect Area
        {
            get { return area; }
        }
        public virtual Rect ScreenArea
        {
            get
            {
                Rect psa = parent != null ? parent.ScreenArea : Rect.Empty;
                //return psa.IsZero ? Rect.Empty : new Rect(area.X + psa.X, area.Y + psa.Y, area.Width, area.Height);
                return new Rect(area.X + psa.X, area.Y + psa.Y, area.Width, area.Height);
            }
        }
        public virtual int X
        {
            get
            {
                return area.X;
            }
            set
            {
                if (area.X != value)
                {
                    area.X = value;
                    Invalidate();
                }
            }
        }
        public virtual int Y
        {
            get { return area.Y; }
            set
            {
                if (area.Y != value)
                {
                    area.Y = value;
                    Invalidate();
                }
            }
        }
        public virtual int Width
        {
            get
            {
                return area.Width;
            }
            set
            {
                if (area.Width != value)
                {
                    area.Width = value;
                    //SizeChanged(area.Width, area.Height);
                    //OnSizeChange(area.Width, area.Height);
                    Invalidate();
                }
            }
        }
        public virtual int Height
        {
            get { return area.Height; }
            set
            {
                if (area.Height != value)
                {
                    area.Height = value;
                    //SizeChanged(area.Width, area.Height);
                    //OnSizeChange(area.Width, area.Height);
                    Invalidate();
                }
            }
        }
        public int ScreenX
        {
            get { return ScreenArea.X; }
        }
        public int ScreenY
        {
            get { return ScreenArea.Y; }
        }
        private Control RootControl
        {
            get
            {
                Control p = null;
                Control pp = this;
                do
                {
                    p = pp;
                    pp = p.Parent;
                } while (pp != null);

                return p != this ? p : null;
                //return p; // old
            }
        }
        #endregion

        #region Events
        //OnSizeChange = function (width, height) { }


        //public event TouchEventHandler SizeChanged;

        public event TouchEventHandler TouchDown;
        public event TouchEventHandler TouchMove;
        public event TouchEventHandler TouchUp;

        public event TouchGestureEventHandler TouchGestureStarted;
        public event TouchGestureEventHandler TouchGestureChanged;
        public event TouchGestureEventHandler TouchGestureEnded;
        #endregion

        #region Constructor
        protected Control(int x, int y, int width, int height)
        {
            area = new Rect(x, y, width, height);
            children = new ControlCollection(this);

            //X = x;
            //Y = y;
            //Width = width;
            //Height = height;
        }
        #endregion

        #region Public methods
        //this.SetLocation = function (x, y) {
        //    if (area.X != x || area.Y != y) {
        //        area.X = x;
        //        area.Y = y;
        //        this.OnLocationChange(x, y);
        //        this.Invalidate();
        //    }
        //}
        //this.SetSize = function (w, h) {
        //    if (area.Width != w || area.Height != h) {
        //        area.Width = w;
        //        area.Height = h;
        //        this.OnSizeChange(w, h);
        //        this.Invalidate();
        //    }
        //}
        //this.AddChild = function (ctrl) {
        //    if (ctrl) {
        //        children.push(ctrl);
        //        ctrl.SetParent(me);
        //    }
        //}
        //this.ClearChildren = function () {
        //    children = [];
        //    this.Invalidate();
        //}

        public bool ContainsScreenPoint(Point p)
        {
            return ScreenArea.Contains(p);
        }
        public bool IntersectsScreenRect(Rect r)
        {
            return ScreenArea.Intersects(r);
        }

        public void PointToScreen(ref Point p)
        {
            Control client = this;
            while (client != null)
            {
                p.X += client.X;
                p.Y += client.Y;

                client = client.Parent;
            }
        }
        public void PointToClient(ref Point p)
        {
            p.X -= ScreenArea.X;
            p.Y -= ScreenArea.Y;
        }

        public void Translate(int dx, int dy)
        {
            area.Translate(dx, dy);
            Invalidate(Rect.Empty);
        }


        //this.GetValidChildFromScreenRect = function (r) {
        //    var targetCtrl = null;
        //    var sa = this.GetScreenArea();

        //    if (visible && enabled && sa.IntersectsScreenRect(r)) {
        //        targetCtrl = this;

        //        for (var i = children.length - 1; i >= 0; i--) {
        //            var child = children[i];
        //            if (child.GetScreenArea().Intersects(sa)) {
        //                var target = child.GetValidChildFromScreenRect(r);
        //                if (target) {
        //                    targetCtrl = target;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return targetCtrl;
        //}
        public Control GetValidChildFromScreenPoint(Point p)
        {
            Control targetCtrl = null;
            var sa = ScreenArea;

            if (isVisible && isEnabled && sa.Contains(p))
            {
                targetCtrl = this;

                for (int i = children.Count - 1; i >= 0; i--)
                {
                    var child = children[i];
                    if (child.ScreenArea.Intersects(sa))
                    {
                        Control target = child.GetValidChildFromScreenPoint(p);
                        if (target != null)
                        {
                            targetCtrl = target;
                            break;
                        }
                    }
                }
            }

            return targetCtrl;
        }
        public Control GetValidParentFromScreenPoint(Point p)
        {
            Control ppp = this;
            Control pp = null;
            do
            {
                pp = ppp;
                ppp = pp.Parent;
            } while (ppp != null && !ppp.ContainsScreenPoint(p));

            return ppp;
        }

        public void SuspendLayout()
        {
            isSuspended = true;
        }
        public void ResumeLayout(bool dontInvalidate = false)
        {
            isSuspended = false;
            if (!dontInvalidate)
                Invalidate(Rect.Empty);
        }

        public void Invalidate()
        {
            Invalidate(Rect.Empty);
        }
        public void Invalidate(Rect rect) // rect = null if by default
        {
            if (!isSuspended)
            {
                //scrollManager.Update();

                Rect sa = ScreenArea;
                Rect r;
                if (!rect.IsZero)
                    r = rect;
                else
                {
                    r = dirtyArea;
                    r.Combine(sa);
                }

                ProcessTask(new RenderTask(this, r));
                if (!rect.IsZero)
                    dirtyArea = sa;
            }
        }
        #endregion

        #region Touch handlers
        internal void RaiseTouchDownEvent(TouchEventArgs e)
        {
            OnTouchDown(e);
        }
        internal void RaiseTouchMoveEvent(TouchEventArgs e)
        {
            OnTouchMove(e);
        }
        internal void RaiseTouchUpEvent(TouchEventArgs e)
        {
            OnTouchUp(e);
        }
        internal void RaiseTouchGestureStartedEvent(TouchGestureEventArgs e)
        {
            OnTouchGestureStarted(e);
        }
        internal void RaiseTouchGestureChangedEvent(TouchGestureEventArgs e)
        {
            OnTouchGestureChanged(e);
        }
        internal void RaiseTouchGestureEndedEvent(TouchGestureEventArgs e)
        {
            OnTouchGestureEnded(e);
        }

        protected virtual void OnTouchDown(TouchEventArgs e)
        {
            if (TouchDown != null)
                TouchDown(this, e);
        }
        protected virtual void OnTouchMove(TouchEventArgs e)
        {
            if (TouchMove != null)
                TouchMove(this, e);
        }
        protected virtual void OnTouchUp(TouchEventArgs e)
        {
            if (TouchUp != null)
                TouchUp(this, e);
        }
        protected virtual void OnTouchGestureStarted(TouchGestureEventArgs e)
        {
            if (TouchGestureStarted != null)
                TouchGestureStarted(this, e);

            if (parent != null)
                parent.OnTouchGestureStarted(e);
        }
        protected virtual void OnTouchGestureChanged(TouchGestureEventArgs e)
        {
            if (TouchGestureChanged != null)
                TouchGestureChanged(this, e);

            if (parent != null)
                parent.OnTouchGestureChanged(e);
        }
        protected virtual void OnTouchGestureEnded(TouchGestureEventArgs e)
        {
            if (TouchGestureEnded != null)
                TouchGestureEnded(this, e);

            if (parent != null)
                parent.OnTouchGestureEnded(e);
        }
        #endregion

        #region Protected methods
        internal void ProcessTask(RenderTask task)
        {
            if (!isSuspended)
            {
                if (parent != null)
                    parent.ProcessTask(task);
                else
                    GraphicsManager.ProcessTask(task);
            }
        }

        internal void OnChildrenChanged(Control added, Control removed, int indexAffected)
        {
            if (added != null)
                added.Invalidate(Rect.Empty);
            else
                Invalidate(Rect.Empty);
        }

        internal void RenderRecursive(DrawingContext dc)
        {
            if (!isVisible || isSuspended)
                return;

            dc.PushClippingRectangle(ScreenArea);
            if (!dc.ClippingRectangle.IsZero)
            {
                OnRender(dc);

                for (int i = 0; i < children.Count; i++)
                {
                    Control child = children[i];
                    if (child != null)
                        child.RenderRecursive(dc);
                }
            }
            dc.PopClippingRectangle();
        }
        internal void PostRenderRecursive(DrawingContext dc)
        {
            if (!isVisible || isSuspended)
                return;

            dc.PushClippingRectangle(ScreenArea);
            if (!dc.ClippingRectangle.IsZero)
            {
                OnPostRender(dc);

                for (var i = 0; i < children.Count; i++) {
                    var child = children[i];
                    if (child != null)
                        child.PostRenderRecursive(dc);
                }
            }
            dc.PopClippingRectangle();
        }

        protected virtual void OnRender(DrawingContext dc)
        {
        }
        protected virtual void OnPostRender(DrawingContext dc)
        {
        }
        #endregion
    }
}
