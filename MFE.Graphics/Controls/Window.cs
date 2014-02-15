using System.Threading;
using MFE.Graphics.Media;
using MFE.LCD;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class Window : Container
    {
        #region Fields
        private ManualResetEvent block;
        private Pen border;
        #endregion

        #region Properties
        //public override Control Parent
        //{
        //    get { return null; }
        //}
        public Pen Border
        {
            get { return border; }
            set
            {
                if (border != value)
                {
                    border = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler Shown;
        public event EventHandler Closed;
        #endregion

        #region Constructors
        public Window()
            : this(0, 0, LCDManager.ScreenWidth, LCDManager.ScreenHeight)
        {
        }
        public Window(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Background = new SolidColorBrush(Color.LightGray);
            block = new ManualResetEvent(false);
        }
        #endregion

        public void Show()
        {
            Visible = true;
            GraphicsManager.Desktop.Children.Add(this);

            if (Shown != null)
                Shown(this, EventArgs.Empty);
        }
        public void ShowModal()
        {
            Show();

            GraphicsManager.modalWindow = this;
            block.WaitOne();
            //block.Reset();
        }
        public void Close()
        {
            block.Set();
            
            GraphicsManager.modalWindow = null;
            GraphicsManager.Desktop.Children.Remove(this);

            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (border != null)
                dc.DrawRectangle(null, border, 0, 0, Width, Height);
        }
    }
}
