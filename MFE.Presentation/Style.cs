using Microsoft.SPOT.Presentation.Media;

namespace MFE.Presentation
{
    public enum Styles
    {
        MonoBlack
    }

    public class Style
    {
        #region Fields
        public static Style Current;
        #endregion

        #region Properties
        public Color TextLightColor { get; set; }
        public Color TextDarkColor { get; set; }

        public Color ButtonBorderColor { get; set; }
        public Brush ButtonLightBrush { get; set; }
        public Brush ButtonDarkBrush { get; set; }






        #endregion

        #region Constructor
        public Style(Styles style)
        {
            switch (style)
            {
                case Styles.MonoBlack:
                    CreateMonoBlack();
                    break;
            }
        }
        #endregion

        #region Private methods
        private void CreateMonoBlack()
        {
            TextLightColor = Colors.White;
            TextDarkColor = Colors.DarkGray;

            ButtonBorderColor = Colors.LightGray;
            ButtonLightBrush = new LinearGradientBrush(ColorUtility.ColorFromRGB(169, 169, 169), ColorUtility.ColorFromRGB(126, 126, 126), 500, 0, 500, 1000);
            ButtonDarkBrush = new LinearGradientBrush(ColorUtility.ColorFromRGB(105, 105, 105), ColorUtility.ColorFromRGB(10, 10, 10), 500, 0, 500, 1000);








        }
        #endregion
    }
}
