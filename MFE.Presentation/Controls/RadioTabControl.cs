using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace Typhoon.MF.Presentation.Controls
{
    public class RadioTabControl : WrapPanel
    {
        #region Fields
        private Brush background;
        #endregion

        #region Properties
        public Brush Background
        {
            get { return background; }
            set
            {
                CheckAccess();
                if (background != value)
                {
                    background = value;
                    Invalidate();
                }
            }
        }
        public Tab SelectedTab
        {
            get
            {
                foreach (Tab t in Children)
                    if (t.IsSelected)
                        return t;
                
                return null;
            }
            set
            {
                CheckAccess();
                if (SelectedTab != value && value != null)
                    value.IsSelected = true;
            }
        }
        #endregion

        #region Events
        public event EventHandler SelectedTabChanged;
        #endregion

        #region Constructor
        public RadioTabControl()
        {
            Orientation = Orientation.Horizontal;
        }
        #endregion

        #region Public methods
        public void AddTab(Tab tab)
        {
            tab.PropertyChanged += new PropertyChangedEventHandler(tab_PropertyChanged);
            Children.Add(tab);
            if (tab.IsSelected)
                ProcessSelectedTab(tab);
        }
        //public void RemoveTab(Tab tab)
        //{
        //    Children.Remove(tab);
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
            foreach (Tab t in Children)
                t.IsSelected = t == tab;

            if (SelectedTabChanged != null)
                SelectedTabChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
