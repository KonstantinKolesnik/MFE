using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public class RadioButtonGroup : Container
    {
        #region Fields
        private int selectedIndex = -1;
        private bool ignore = false;
        #endregion

        #region Properties
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex != value)
                {
                    ignore = true;
                    if (selectedIndex != -1)
                        ((RadioButton)children[selectedIndex]).Checked = false;

                    selectedIndex = value;

                    ((RadioButton)children[selectedIndex]).Checked = true;
                    ignore = false;

                    OnSelectionChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler SelectionChanged;
        protected void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }
        #endregion

        #region Constructor
        public RadioButtonGroup(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }
        #endregion

        #region Public methods
        public void AddRadioButton(RadioButton rb)
        {
            if (rb != null && !Children.Contains(rb))
            {
                int idx = Children.Add(rb);
                if (rb.Checked)
                    SelectedIndex = idx;
                rb.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            }
        }
        public void RemoveRadioButton(RadioButton rb)
        {
            if (rb != null && Children.Contains(rb))
            {
                Children.Remove(rb);
                if (rb.Checked)
                    selectedIndex = -1;
            }
        }
        #endregion

        #region Event handlers
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignore)
            {
                RadioButton rb = (RadioButton)sender;
                for (int i = 0; i < Children.Count; i++)
                    if (children[i] == rb)
                    {
                        SelectedIndex = i;
                        break;
                    }
            }
        }
        #endregion
    }
}
