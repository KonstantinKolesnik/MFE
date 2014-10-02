using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation.Controls
{
    public class Tab : ToolButton
    {
        #region Fields
        private bool isSelected = false;
        private UIElement tag;
        #endregion

        #region Properties
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                VerifyAccess();
                if (isSelected != value)
                {
                    isSelected = value;
                    Invalidate();
                    OnPropertyChanged(new PropertyChangedEventArgs("IsSelected", !isSelected, isSelected));
                }
            }
        }
        public UIElement Tag
        {
            get { return tag; }
            set
            {
                VerifyAccess();
                tag = value;
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructors
        public Tab()
            : this("")
        {
        }
        public Tab(string name)
            : base(name)
        {
            Click += new EventHandler(Tab_Clicked);
        }
        public Tab(string name, UIElement content)
            : this(name)
        {
            Content = content;
        }
        #endregion

        #region Event handlers
        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (isSelected)
            {
                Pen pen = new Pen(Colors.DarkGray, 1);

                // top border
                dc.DrawLine(pen, 0, 0, ActualWidth - 1, 0);

                // left border
                dc.DrawLine(pen, 0, 0, 0, ActualHeight - 1);

                // right border
                dc.DrawLine(pen, ActualWidth - 1, 0, ActualWidth - 1, ActualHeight - 1);

                // bottom border
                dc.DrawLine(pen, 0, ActualHeight - 1, ActualWidth - 1, ActualHeight - 1);
            }
        }
        private void Tab_Clicked(object sender, EventArgs e)
        {
            IsSelected = true;
        }
        #endregion
    }
}
