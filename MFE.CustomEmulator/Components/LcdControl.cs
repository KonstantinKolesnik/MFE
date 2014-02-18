using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.SPOT.Emulator.Lcd;
using Microsoft.SPOT.Emulator.TouchPanel;

namespace MFE.CustomEmulator.Components
{
    /// <summary>
    /// A WinForm control to display the contents of an LCD of a .NET Micro 
    /// Framework application.
    /// </summary>
    public class LcdControl : Control
    {
        ///
        /// Make sure that this enum is kept up to date with TinyCore's Gesture enum
        ///
        public enum TouchGesture : uint
        {
            NoGesture = 0,          //Can be used to represent an error gesture or unknown gesture

            //Standard Win7 Gestures
            Begin = 1,       //Used to identify the beginning of a Gesture Sequence; App can use this to highlight UIElement or some other sort of notification.
            End = 2,       //Used to identify the end of a gesture sequence; Fired when last finger involved in a gesture is removed.

            // Standard stylus (single touch) gestues
            Right = 3,
            UpRight = 4,
            Up = 5,
            UpLeft = 6,
            Left = 7,
            DownLeft = 8,
            Down = 9,
            DownRight = 10,
            Tap = 11,
            DoubleTap = 12,

            // Multi-touch gestures
            Zoom = 114,      //Equivalent to your "Pinch" gesture
            Pan = 115,      //Equivalent to your "Scroll" gesture
            Rotate = 116,
            TwoFingerTap = 117,
            Rollover = 118,      // Press and tap               

            //Additional NetMF gestures
            UserDefined = 65537
        }


        #region Fields
        private LcdDisplay lcd;// The .NET Micro Framework LCD emulator component.
        private TouchGpioPort touchPort = null;
        private Bitmap _bitmap;// A bitmap to store the current LCD contents.

        private int lastMouseDownX = 0;
        private int lastMouseDownY = 0;

        private double lastDist = 0.0;
        private double lastAngle = 0.0;

        private TouchGesture m_currentGesture = TouchGesture.NoGesture;
        private int m_GestureNumKeysPressed = 0;
        private const int c_GestureRotateXOffset = 20;

        /// <summary>
        /// Send mouse events as touch events to PAL.
        /// _touchPort.WriteTouchData works like an actual hardware event.
        /// </summary>

        private const int TouchSampleValidFlag = 0x01;
        private const int TouchSampleDownFlag = 0x02;
        private const int TouchSampleIsCalibratedFlag = 0x04;
        private const int TouchSamplePreviousDownFlag = 0x08;
        private const int TouchSampleIgnore = 0x10;
        private const int TouchSampleMouse = 0x40000000;

        private int flags = 0;
        #endregion

        #region Properties
        public LcdDisplay LcdDisplay
        {
            get { return lcd; }
            set
            {
                if (lcd != null)
                    lcd.OnDevicePaint -= new OnDevicePaintEventHandler(OnDevicePaint);
                lcd = value;
                if (lcd != null)
                    lcd.OnDevicePaint += new OnDevicePaintEventHandler(OnDevicePaint);
            }
        }
        public TouchGpioPort TouchPort
        {
            get { return touchPort; }
            set { touchPort = value; }
        }
        #endregion

        public LcdControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
        }


        /// <summary>
        /// Callback that runs when the application flushes the LCD buffer to 
        /// the screen.  Copies the emulator's internal bitmap to the control.
        /// </summary>
        /// <param name="sender">The emulator component firing the event.</param>
        /// <param name="args">What is being redrawn.</param>
        private void OnDevicePaint(object sender, OnDevicePaintEventArgs args)
        {
            Bitmap bitmap = args.Bitmap;

            if (_bitmap == null)
            {
                // The first time the callback occurs, simply make a copy of the 
                // LCD bitmap.  This is necessary so the .NET Micro Framework 
                // can draw on its frame buffer after this callback returns.
                _bitmap = (Bitmap)bitmap.Clone();
            }
            else
            {
                // Synchronize the _bitmap object to prevent conflict between 
                // the Micro Framework thread (which this callback runs on) and 
                // the UI thread (which runs during paint).
                lock (_bitmap)
                {
                    Rectangle rectangle = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);

                    // Lock the source and target bitmaps in memory so they 
                    // can't move while we're copying them.
                    BitmapData bdSrc = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    BitmapData bdDst = _bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, bitmap.PixelFormat);

                    // Copy the bitmap data, 4 bytes (an int) at a time, using 
                    // unsafe code.  Copying the entire frame buffer can be 
                    // substantially slower in safe code
                    unsafe
                    {
                        int* src = (int*)bdSrc.Scan0.ToPointer();
                        int* dst = (int*)bdDst.Scan0.ToPointer();
                        int cInts = bdSrc.Stride / 4 * bitmap.Height;

                        Debug.Assert(bdSrc.Stride > 0);
                        Debug.Assert(bitmap.Width == _bitmap.Width);
                        Debug.Assert(bitmap.Height == _bitmap.Height);
                        Debug.Assert(bitmap.PixelFormat == _bitmap.PixelFormat);

                        for (int i = 0; i < cInts; i++)
                            *dst++ = *src++;
                    }

                    // Unlock the source and target bitmaps.
                    bitmap.UnlockBits(bdSrc);
                    _bitmap.UnlockBits(bdDst);
                }
            }

            // Force this control to be redrawn.
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Paint the LCD control.  Use the contents of the LCD if available; 
            // otherwise call through to the base class.
            if (_bitmap != null)
            {
                // Synchonize access to the bitmap with the .NET Micro Framework 
                // thread.
                lock (_bitmap)
                {
                    // Draw the LCD contents.
                    e.Graphics.DrawImage(_bitmap, 0, 0);

                    if (m_currentGesture == TouchGesture.Rotate && (flags & (TouchSampleValidFlag | TouchSampleDownFlag)) == (TouchSampleValidFlag | TouchSampleDownFlag))
                        e.Graphics.DrawEllipse(Pens.Red, lastMouseDownX - c_GestureRotateXOffset, lastMouseDownY, 3, 3);
                }
            }
            else
            // We have no private LCD bitmap, because the .NET Micro Framework hasn't called OnDevicePaint yet.
            {
                base.OnPaintBackground(e);
            }

            if (this.DesignMode)
            {
                // At design time, paint a dotted outline around the control.
                OnPaintDesignMode(e);
            }

            base.OnPaint(e);
        }
        private void OnPaintDesignMode(PaintEventArgs e)
        {
            Rectangle rc = ClientRectangle;
            Color penColor;

            // Choose black or white pen, depending on the color of the control.
            if (BackColor.GetBrightness() < .5)
                penColor = ControlPaint.Light(BackColor);
            else
                penColor = ControlPaint.Dark(BackColor); ;

            using (Pen pen = new Pen(penColor))
            {
                pen.DashStyle = DashStyle.Dash;

                rc.Width--;
                rc.Height--;
                e.Graphics.DrawRectangle(pen, rc);
            }
        }

        ///
        /// Emulate multitouch gestures - By pressing CTRL+x (where x is one of the keyboard keys in the switch statement below).
        /// This currently enables all multitouch gestures supported by .Net MF.
        ///
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Control && m_currentGesture == TouchGesture.NoGesture)
            {
                switch (e.KeyCode)
                {
                    case Keys.NumPad2: m_GestureNumKeysPressed = 2; break;
                    case Keys.NumPad3: m_GestureNumKeysPressed = 3; break;
                    case Keys.P: m_currentGesture = TouchGesture.Pan; break;
                    case Keys.Z: m_currentGesture = TouchGesture.Zoom; break;
                    case Keys.R: m_currentGesture = TouchGesture.Rotate; break;
                    case Keys.T: m_currentGesture = TouchGesture.TwoFingerTap; break;
                    case Keys.O: m_currentGesture = TouchGesture.Rollover; break;
                }
                if (m_currentGesture != TouchGesture.NoGesture)
                {
                    touchPort.PostGesture((int)TouchGesture.Begin, lastMouseDownX, lastMouseDownY, 0);
                    if (m_currentGesture == TouchGesture.TwoFingerTap || m_currentGesture == TouchGesture.Rollover)
                        touchPort.PostGesture((int)m_currentGesture, lastMouseDownX, lastMouseDownY, 0);
                }
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.NumPad2:
                case Keys.NumPad3:
                    m_GestureNumKeysPressed = 0;
                    break;

                case Keys.P:
                case Keys.Z:
                case Keys.R:
                case Keys.T:
                case Keys.O:
                    m_currentGesture = TouchGesture.NoGesture;
                    touchPort.PostGesture((int)TouchGesture.End, lastMouseDownX, lastMouseDownY, 0);
                    break;
            }

            base.OnKeyUp(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (0 != (flags & TouchSampleDownFlag))
            {
                if (m_currentGesture != TouchGesture.NoGesture)
                {
                    m_currentGesture = TouchGesture.NoGesture;
                    touchPort.PostGesture((int)TouchGesture.End, lastMouseDownX, lastMouseDownY, 0);
                }

                MouseEventArgs mea = new MouseEventArgs(MouseButtons.Left, 1, lastMouseDownX, lastMouseDownY, 0);

                OnMouseUp(mea);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();

            flags = TouchSampleValidFlag | TouchSampleDownFlag;

            lastMouseDownX = e.X;
            lastMouseDownY = e.Y;

            touchPort.WriteTouchData(flags, 0, e.X, e.Y);

            switch (m_GestureNumKeysPressed)
            {
                case 2:
                    touchPort.WriteTouchData(flags, 1, e.X, e.Y);
                    break;
                case 3:
                    touchPort.WriteTouchData(flags, 1, e.X, e.Y);
                    touchPort.WriteTouchData(flags, 2, e.X, e.Y);
                    break;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (m_currentGesture != TouchGesture.NoGesture)
            {
                HandleGesture(e.X, e.Y, true);

                m_currentGesture = TouchGesture.NoGesture;
                touchPort.PostGesture((int)TouchGesture.End, lastMouseDownX, lastMouseDownY, 0);
            }

            flags = TouchSampleValidFlag | TouchSamplePreviousDownFlag;

            touchPort.WriteTouchData(flags, 0, e.X, e.Y);

            switch (m_GestureNumKeysPressed)
            {
                case 2:
                    touchPort.WriteTouchData(flags, 1, e.X, e.Y);
                    break;
                case 3:
                    touchPort.WriteTouchData(flags, 1, e.X, e.Y);
                    touchPort.WriteTouchData(flags, 2, e.X, e.Y);
                    break;
            }

            m_GestureNumKeysPressed = 0;
            lastAngle = 0.0;
            lastDist = 0.0;
            lastMouseDownX = 0;
            lastMouseDownY = 0;

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if ((flags & (TouchSampleValidFlag | TouchSampleDownFlag)) == (TouchSampleValidFlag | TouchSampleDownFlag))
            {
                flags = TouchSampleValidFlag | TouchSamplePreviousDownFlag | TouchSampleDownFlag;

                if ((e.X >= 0) && (e.Y >= 0))
                {
                    touchPort.WriteTouchData(flags, 0, e.X, e.Y);
                    switch (m_GestureNumKeysPressed)
                    {
                        case 2:
                            touchPort.WriteTouchData(flags, 1, e.X, e.Y);
                            break;
                        case 3:
                            touchPort.WriteTouchData(flags, 1, e.X, e.Y);
                            touchPort.WriteTouchData(flags, 2, e.X, e.Y);
                            break;
                    }
                    HandleGesture(e.X, e.Y, false);
                }
            }
        }

        private void HandleGesture(int x, int y, bool fForce)
        {
            if (m_currentGesture != TouchGesture.NoGesture)
            {
                ushort data = 0;

                int xCenter = lastMouseDownX, yCenter = lastMouseDownY;

                switch (m_currentGesture)
                {
                    case TouchGesture.Pan:
                    case TouchGesture.Zoom:
                        double dist = Math.Sqrt(Math.Pow(x - xCenter, 2.0) + Math.Pow(yCenter - y, 2.0));
                        if (Math.Abs(dist - lastDist) > 10 || fForce)
                        {
                            if (x < xCenter && yCenter < y)
                            {
                                m_currentGesture = TouchGesture.Pan;
                                data = (ushort)dist;
                                lastDist = dist;
                            }
                            else if (x > xCenter && yCenter > y)
                            {
                                m_currentGesture = TouchGesture.Zoom;
                                data = (ushort)dist;
                                lastDist = dist;
                            }
                        }
                        break;
                    case TouchGesture.Rotate:
                        xCenter -= c_GestureRotateXOffset;

                        double dx = (x - xCenter);
                        double dy = (yCenter - y);

                        double angle = 180.0 * Math.Atan(Math.Abs(dy) / Math.Abs(dx)) / Math.PI;

                        if (dx < 0)
                        {
                            if (dy < 0)
                                angle += 180;
                            else
                                angle = 180 - angle;
                        }
                        else if (dx > 0 && dy < 0)
                            angle = 360 - angle;

                        if (angle < 0)
                            angle += 360.0;

                        // switch to clockwise rotational angle
                        angle = 360.0 - angle;

                        if ((Math.Abs(angle - lastAngle) > c_GestureRotateXOffset && (360.0 - (angle - lastAngle)) > c_GestureRotateXOffset) || fForce)
                        {
                            data = (ushort)(angle);
                            lastAngle = angle;
                        }

                        break;
                }

                if (data != 0)
                    touchPort.PostGesture((int)m_currentGesture, xCenter, yCenter, data);
            }
        }
    }
}
