using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;

namespace MFE.Presentation.Controls
{
    public class TabControl : BackgroundStackPanel
    {
        #region Fields
        private WrapPanel pnlHeaders;
        private Panel pnlContent;
        #endregion

        #region Properties
        public Tab SelectedTab
        {
            get
            {
                foreach (Tab t in pnlHeaders.Children)
                    if (t.IsSelected)
                        return t;
                
                return null;
            }
        }
        #endregion

        #region Events
        public event EventHandler SelectedTabChanged;
        #endregion

        #region Constructor
        public TabControl()
        {
            Orientation = Orientation.Vertical;

            pnlHeaders = new WrapPanel(Orientation.Horizontal);
            Children.Add(pnlHeaders);

            pnlContent = new Panel();
            Children.Add(pnlContent);
        }
        #endregion

        //public override void OnRender(DrawingContext dc)
        //{
        //    base.OnRender(dc);

        //    dc.DrawRectangle(background, new Pen(Colors.Gray), 0, pnlHeaders.ActualHeight - 1, ActualWidth, ActualHeight - pnlHeaders.ActualHeight);
        //}

        #region Public methods
        public void AddTab(Tab tab)
        {
            tab.PropertyChanged += new PropertyChangedEventHandler(tab_PropertyChanged);
            pnlHeaders.Children.Add(tab);
            if (tab.IsSelected)
                ProcessSelectedTab(tab);
        }
        //public void RemoveTab(MfeTab tab)
        //{
        //    pnlHeaders.Children.Remove(tab);
        //    if (tab.IsSelected)
        //        ProcessSelectedTab((Tab)pnlHeaders.Children[0]);
        //}
        #endregion

        #region Event handlers
        private void tab_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.Property)
            {
                case "IsSelected":
                    if ((bool)e.NewValue)
                        ProcessSelectedTab((Tab)sender);
                    break;
            }
        }
        #endregion

        #region Private methods
        private void ProcessSelectedTab(Tab tab)
        {
            foreach (Tab t in pnlHeaders.Children)
                t.IsSelected = t == tab;

            pnlContent.Children.Clear();
            if (tab.Tag != null)
                pnlContent.Children.Add(tab.Tag);

            if (SelectedTabChanged != null)
                SelectedTabChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
