using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

namespace Typhoon.MF.Presentation.Controls
{
    public class RadioButton : StackPanel
    {
        #region Fields
        private Text text;
        private Image image;
        private bool isChecked = false;
        private int textMargin = 3;
        #endregion

        #region Properties
        public string Text
        {
            get { return text.TextContent; }
            set
            {
                CheckAccess();
                if (text.TextContent != value)
                    text.TextContent = value;
            }
        }
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                VerifyAccess();
                if (isChecked != value)
                {
                    isChecked = value;
                    SetImage();

                    if (IsCheckedChanged != null)
                        IsCheckedChanged(this, new PropertyChangedEventArgs("IsChecked", !value, value));
                }
            }
        }
        public Color ForeColor
        {
            get { return text.ForeColor; }
            set
            {
                CheckAccess();
                if (text.ForeColor != value)
                    text.ForeColor = value;
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler IsCheckedChanged;
        #endregion

        #region Constructors
        public RadioButton(Font font)
            : this(font, "")
        {
        }
        public RadioButton(Font font, string txt)
            : this(font, txt, false)
        {
        }
        public RadioButton(Font font, string txt, bool isChecked)
        {
            Orientation = Orientation.Horizontal;

            this.isChecked = isChecked;

            image = new Image();
            image.VerticalAlignment = VerticalAlignment.Center;
            SetImage();
            Children.Add(image);

            text = new Text(font, txt);
            text.ForeColor = Color.White;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.TextWrap = false;
            text.SetMargin(textMargin, 0, 0, 0);
            Children.Add(text);
        }
        #endregion

        #region Event handlers
        protected override void OnTouchDown(TouchEventArgs e)
        {
            IsChecked = true;
        }
        #endregion

        #region Private methods
        private void SetImage()
        {
            Bitmap bmp = isChecked ? ResourceManager.BmpRadioButtonChecked : ResourceManager.BmpRadioButtonClear;
            bmp.MakeTransparent(bmp.GetPixel(0, 0));
            image.Bitmap = bmp;
        }
        #endregion
    }
}
