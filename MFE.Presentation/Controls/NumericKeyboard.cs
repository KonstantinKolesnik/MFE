using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using MF.Engine.Utilities;

namespace Typhoon.MF.Presentation.Controls
{
    //public class NumericKeyboard : BackgroundStackPanel
    //{
    //    private Text txtNumber;

    //    public int Number
    //    {
    //        get { return Utils.IsStringEmpty(txtNumber.TextContent) ? 0 : int.Parse(txtNumber.TextContent); }
    //    }

    //    public NumericKeyboard(Font font, Color textColor, Color buttonTextColor)
    //    {
    //        Orientation = Orientation.Vertical;

    //        txtNumber = new Text(font, "0");
    //        txtNumber.ForeColor = textColor;
    //        txtNumber.TextAlignment = TextAlignment.Right;
    //        txtNumber.TextWrap = true;
    //        txtNumber.HorizontalAlignment = HorizontalAlignment.Stretch;
    //        Children.Add(txtNumber);

    //        StackPanel p;
    //        Button btn;

    //        p = new StackPanel(Orientation.Horizontal);
    //        p.HorizontalAlignment = HorizontalAlignment.Center;
    //        Children.Add(p);

    //        btn = new Button(font, " 8 ", null, buttonTextColor);
    //        //btn.SetMargin(1);
    //        btn.HorizontalAlignment = HorizontalAlignment.Center;
    //        //btn.BackgroundImage = Program.ButtonBackground;
    //        btn.Clicked += new EventHandler(NumericButton_Clicked);
    //        p.Children.Add(btn);

    //    }

    //    private void NumericButton_Clicked(object sender, EventArgs e)
    //    {
    //        txtNumber.TextContent += (sender as Button).Text;
    //    }

    //}
}
