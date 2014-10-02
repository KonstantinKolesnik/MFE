using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;

namespace Typhoon.MF.Presentation.Controls
{
    public class RadioButtonGroup : StackPanel
    {
        #region Fields
        private int selectedIndex = -1;
        private int margin = 3;
        private bool ignore = false;
        #endregion

        #region Properties
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                CheckAccess();
                if (selectedIndex != value)
                {
                    selectedIndex = value;

                    // check
                    ((RadioButton)Children[selectedIndex]).IsChecked = true;

                    // uncheck others
                    ignore = true;
                    for (int i = 0; i < Children.Count; i++)
                        if (i != selectedIndex)
                            ((RadioButton)Children[i]).IsChecked = false;
                    ignore = false;

                    if (SelectionChanged != null)
                        SelectionChanged(this, null);
                }
            }
        }
        #endregion

        #region Events
        public PropertyChangedEventHandler SelectionChanged;
        #endregion

        #region Constructor
        public RadioButtonGroup(Orientation orientation)
        {
            Orientation = orientation;
        }
        #endregion

        #region Public methods
        public void AddButton(RadioButton rb)
        {
            if (rb != null && !Children.Contains(rb))
            {
                Children.Add(rb);
                rb.IsCheckedChanged += new PropertyChangedEventHandler(RadioButton_CheckedChanged);
                rb.SetMargin(margin);
            }
        }
        public void RemoveButton(RadioButton rb)
        {
            if (rb != null && Children.Contains(rb))
                Children.Remove(rb);
        }
        #endregion

        #region Event handlers
        private void RadioButton_CheckedChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ignore)
                return;

            RadioButton rb = (RadioButton)sender;
            for (int i = 0; i < Children.Count; i++)
                if (Children[i] == rb)
                {
                    SelectedIndex = i;
                    break;
                }
        }
        #endregion
    }
}
