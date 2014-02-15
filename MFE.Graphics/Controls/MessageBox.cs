using System;
using Microsoft.SPOT;

namespace MFE.Graphics.Controls
{
    public enum MessageBoxButton
    {
        OK = 0,
        OKCancel = 1,
        YesNoCancel = 3,
        YesNo = 4,
    }

    public enum MessageBoxImage
    {
        None = 0,
        Error = 16,
        Hand = 16,
        Stop = 16,
        Question = 32,
        Exclamation = 48,
        Warning = 48,
        Information = 64,
        Asterisk = 64,
    }

    public enum MessageBoxResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7,
    }


    public sealed class MessageBox
    {



        //public static MessageBoxResult Show(string messageBoxText);
        //public static MessageBoxResult Show(string messageBoxText, string caption);
        //public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button);
        //public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);

        //public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult);
    }
}
