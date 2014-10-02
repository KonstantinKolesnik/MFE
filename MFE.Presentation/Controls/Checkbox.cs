using Microsoft.SPOT;
using Microsoft.SPOT.Input;

namespace MFE.Presentation.Controls
{
    public class Checkbox : Icon
    {
        #region Fields
        private bool isChecked;
        #endregion

        #region Properties
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                CheckAccess();
                if (isChecked != value)
                {
                    isChecked = value;
                    Bitmap = isChecked ? ResourceManager.BmpCheckboxChecked : ResourceManager.BmpCheckboxClear;

                    if (IsCheckedChanged != null)
                        IsCheckedChanged(this, new PropertyChangedEventArgs("IsChecked", !value, value));
                }
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler IsCheckedChanged;
        #endregion

        #region Constructors
        public Checkbox()
            : this(false)
        {
        }
        public Checkbox(bool isChecked)
            : base(null, 13)
        {

            IsChecked = isChecked;
        }
        #endregion

        #region Event handlers
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (IsEnabled)
                IsChecked = !IsChecked;
        }
        #endregion
    }
}
