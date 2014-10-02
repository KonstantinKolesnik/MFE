using Microsoft.SPOT;

namespace MFE.Presentation
{
    class ResourceManager
    {
        public static Bitmap BmpCheckboxClear;
        public static Bitmap BmpCheckboxChecked;
        public static Bitmap BmpRadioButtonClear;
        public static Bitmap BmpRadioButtonChecked;

        static ResourceManager()
        {
            BmpCheckboxClear = Resources.GetBitmap(Resources.BitmapResources.CheckboxClear);
            BmpCheckboxChecked = Resources.GetBitmap(Resources.BitmapResources.CheckboxChecked);
            BmpRadioButtonClear = Resources.GetBitmap(Resources.BitmapResources.RadiobuttonClear);
            BmpRadioButtonChecked = Resources.GetBitmap(Resources.BitmapResources.RadiobuttonChecked);
        }
    }
}
