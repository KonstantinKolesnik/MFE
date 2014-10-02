using MF.Engine.Utilities;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using MFE.Graphics.Presentation;
//using Microsoft.SPOT.Touch;

namespace Typhoon.MF.Presentation.Controls
{
    public enum MessageBoxButton
    {
        // Summary:
        //     The message box displays an OK button.
        OK = 0,
        //
        // Summary:
        //     The message box displays OK and Cancel buttons.
        OKCancel = 1,
        //
        // Summary:
        //     The message box displays Yes, No, and Cancel buttons.
        YesNoCancel = 3,
        //
        // Summary:
        //     The message box displays Yes and No buttons.
        YesNo = 4,
    }
    public enum MessageBoxResult
    {
        // Summary:
        //     The message box returns no result.
        None = 0,
        //
        // Summary:
        //     The result value of the message box is OK.
        OK = 1,
        //
        // Summary:
        //     The result value of the message box is Cancel.
        Cancel = 2,
        //
        // Summary:
        //     The result value of the message box is Yes.
        Yes = 6,
        //
        // Summary:
        //     The result value of the message box is No.
        No = 7,
    }
    public enum MessageBoxImage
    {
        // Summary:
        //     No icon is displayed.
        None = 0,
        //
        // Summary:
        //     The message box displays an error icon.
        Error = 16,
        //
        // Summary:
        //     The message box displays a hand icon.
        Hand = 16,
        //
        // Summary:
        //     The message box displays a stop icon.
        Stop = 16,
        //
        // Summary:
        //     The message box displays a question mark icon.
        Question = 32,
        //
        // Summary:
        //     The message box displays an exclamation mark icon.
        Exclamation = 48,
        //
        // Summary:
        //     The message box displays a warning icon.
        Warning = 48,
        //
        // Summary:
        //     The message box displays an information icon.
        Information = 64,
        //
        // Summary:
        //     The message box displays an asterisk icon.
        Asterisk = 64,
    }

    public class MessageBox : Window
    {
        #region Fields
        private MessageBoxButton buttonsSet;
        private Text txtMessage;
        private Button btn1;
        private Button btn2;
        private Button btn3;
        private MessageBoxResult result;

        private DispatcherFrame modalBlock;
        private Window main;
        #endregion

        #region Constructor
        public MessageBox(Font font, int width, string message, MessageBoxButton buttonsSet, Bitmap buttonBackground, Color buttonTextColor, Color textColor)
        {
            Background = new SolidColorBrush(Colors.Black);
            Width = width;
            Visibility = Visibility.Hidden;
            SizeToContent = SizeToContent.Height;

            this.buttonsSet = buttonsSet;

            main = Application.Current.MainWindow;
            modalBlock = new DispatcherFrame();

            StackPanel pnl = new StackPanel(Orientation.Vertical);
            Child = pnl;

            txtMessage = new Text(font, message);
            txtMessage.ForeColor = textColor;
            txtMessage.HorizontalAlignment = HorizontalAlignment.Center;
            txtMessage.TextAlignment = TextAlignment.Center;
            txtMessage.TextWrap = true;
            txtMessage.SetMargin(3);
            pnl.Children.Add(txtMessage);

            StackPanel pnlButtons = new StackPanel(Orientation.Horizontal);
            pnlButtons.HorizontalAlignment = HorizontalAlignment.Center;
            pnlButtons.VerticalAlignment = VerticalAlignment.Bottom;
            pnl.Children.Add(pnlButtons);

            btn1 = new Button(font, "", null, buttonTextColor);
            btn1.Background = buttonBackground;
            btn1.ImageSize = 16;
            btn1.HorizontalAlignment = HorizontalAlignment.Center;
            btn1.SetMargin(3);
            btn1.Clicked += new EventHandler(btn1_Clicked);
            pnlButtons.Children.Add(btn1);

            btn2 = new Button(font, "", null, buttonTextColor);
            btn2.Background = buttonBackground;
            btn2.ImageSize = 16;
            btn2.HorizontalAlignment = HorizontalAlignment.Center;
            btn2.SetMargin(3);
            btn2.Clicked += new EventHandler(btn2_Clicked);
            pnlButtons.Children.Add(btn2);

            btn3 = new Button(font, "", null, buttonTextColor);
            btn3.Background = buttonBackground;
            btn3.ImageSize = 16;
            btn3.HorizontalAlignment = HorizontalAlignment.Center;
            btn3.SetMargin(3);
            btn3.Clicked += new EventHandler(btn3_Clicked);
            pnlButtons.Children.Add(btn3);

            switch (buttonsSet)
            {
                case MessageBoxButton.OK:
                    btn1.Text = "OK";
                    btn2.Visibility = btn3.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    btn1.Text = "OK";
                    btn2.Text = "Отмена";
                    btn3.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    btn1.Text = "Да";
                    btn2.Text = "Нет";
                    btn3.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btn1.Text = "Да";
                    btn2.Text = "Нет";
                    btn3.Text = "Отмена";
                    break;
            }

            //Top = (SystemMetrics.ScreenHeight - Height) / 2;
        }
        #endregion

        #region Public methods
        public MessageBoxResult ShowDialog()
        {
            InputManager.CurrentInputManager.InputDeviceEvents[(int)InputManager.InputDeviceType.Touch].PreProcessInput += new PreProcessInputEventHandler(InputDeviceEvents_PreProcessInput);
            Visibility = Visibility.Visible;
            Topmost = true;
            Application.Current.MainWindow = this;

            //TouchCapture.Capture(this, CaptureMode.SubTree);
            Buttons.Focus(this);

            Dispatcher.PushFrame(modalBlock);

            Close();
            Application.Current.MainWindow = main;
            InputManager.CurrentInputManager.InputDeviceEvents[(int)InputManager.InputDeviceType.Touch].PreProcessInput -= new PreProcessInputEventHandler(InputDeviceEvents_PreProcessInput);
            return result;
        }
        #endregion

        #region Event handlers
        private void btn1_Clicked(object sender, EventArgs e)
        {
            switch (buttonsSet)
            {
                case MessageBoxButton.OK:
                    result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.OKCancel:
                    result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.YesNo:
                    result = MessageBoxResult.Yes;
                    break;
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.Yes;
                    break;
            }
            modalBlock.Continue = false;
        }
        private void btn2_Clicked(object sender, EventArgs e)
        {
            switch (buttonsSet)
            {
                case MessageBoxButton.OKCancel:
                    result = MessageBoxResult.Cancel;
                    modalBlock.Continue = false;
                    break;
                case MessageBoxButton.YesNo:
                    result = MessageBoxResult.No;
                    modalBlock.Continue = false;
                    break;
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.No;
                    modalBlock.Continue = false;
                    break;
            }
            //TouchCapture.Capture(this, CaptureMode.None);
        }
        private void btn3_Clicked(object sender, EventArgs e)
        {
            switch (buttonsSet)
            {
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.Cancel;
                    modalBlock.Continue = false;
                    break;
            }
        }
        private void InputDeviceEvents_PreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.StagingItem.Input.Device is TouchDevice)
            {
                InputReportEventArgs irea = e.StagingItem.Input as InputReportEventArgs;
                if (irea != null)
                {
                    RawTouchInputReport rtir = irea.Report as RawTouchInputReport;
                    if (rtir != null)
                    {
                        // cancel any touch event that does not start within the modal window
                        //if (!this.ContainsPoint(rtir.Touches[0].X, rtir.Touches[0].Y))
                        //    e.Cancel();

                        int x = rtir.Touches[0].X;
                        int y = rtir.Touches[0].Y;
                        PointToClient(ref x, ref y);
                        if (!Utils.IsWithinRectangle(x, y, ActualWidth, ActualWidth))
                            e.Cancel();
                    }
                }
            }
        }
        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    base.OnTouchDown(e);
        //}
        #endregion
    }
    public class QuaziMessageBox : StackPanel
    {
        #region Fields
        private MessageBoxButton buttons;
        private Text txtMessage;
        private Button btn1;
        private Button btn2;
        private Button btn3;
        private MessageBoxResult result;
        private EventHandler onClose;
        #endregion

        public MessageBoxResult Result
        {
            get { return result; }
        }

        #region Constructor
        public QuaziMessageBox(Font font, int width, string message, MessageBoxButton buttons, Bitmap buttonBackground, Color buttonTextColor, Color textColor, EventHandler Closed)
        {
            Width = width;
            Visibility = Visibility.Hidden;

            this.buttons = buttons;
            onClose = Closed;

            Orientation = Orientation.Vertical;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            txtMessage = new Text(font, message);
            txtMessage.ForeColor = Color.Black;
            txtMessage.HorizontalAlignment = HorizontalAlignment.Center;
            txtMessage.TextAlignment = TextAlignment.Center;
            txtMessage.TextWrap = true;
            txtMessage.SetMargin(3);
            Children.Add(txtMessage);

            StackPanel pnlButtons = new StackPanel(Orientation.Horizontal);
            pnlButtons.HorizontalAlignment = HorizontalAlignment.Center;
            pnlButtons.VerticalAlignment = VerticalAlignment.Bottom;
            Children.Add(pnlButtons);

            btn1 = new Button(font, "", null, buttonTextColor);
            btn1.Background = buttonBackground;
            btn1.ImageSize = 16;
            btn1.HorizontalAlignment = HorizontalAlignment.Center;
            btn1.SetMargin(3);
            btn1.Clicked += new EventHandler(btn1_Clicked);
            pnlButtons.Children.Add(btn1);

            btn2 = new Button(font, "", null, buttonTextColor);
            btn2.Background = buttonBackground;
            btn2.ImageSize = 16;
            btn2.HorizontalAlignment = HorizontalAlignment.Center;
            btn2.SetMargin(3);
            btn2.Clicked += new EventHandler(btn2_Clicked);
            pnlButtons.Children.Add(btn2);

            btn3 = new Button(font, "", null, buttonTextColor);
            btn3.Background = buttonBackground;
            btn3.ImageSize = 16;
            btn3.HorizontalAlignment = HorizontalAlignment.Center;
            btn3.SetMargin(3);
            btn3.Clicked += new EventHandler(btn3_Clicked);
            pnlButtons.Children.Add(btn3);

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    btn1.Text = "OK";
                    btn2.Visibility = btn3.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    btn1.Text = "OK";
                    btn2.Text = "Отмена";
                    btn3.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    btn1.Text = "Да";
                    btn2.Text = "Нет";
                    btn3.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btn1.Text = "Да";
                    btn2.Text = "Нет";
                    btn3.Text = "Отмена";
                    break;
            }
        }
        #endregion

        #region Public methods
        public void ShowDialog()
        {
            InputManager.CurrentInputManager.InputDeviceEvents[(int)InputManager.InputDeviceType.Touch].PreProcessInput += new PreProcessInputEventHandler(InputDeviceEvents_PreProcessInput);
            Visibility = Visibility.Visible;
            Buttons.Focus(this);
            TouchCapture.Capture(this, CaptureMode.SubTree);
        }
        #endregion

        #region Event handlers
        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(new SolidColorBrush(Colors.White), new Pen(Colors.Gray), 0, 0, ActualWidth, ActualHeight);
        }

        private void btn1_Clicked(object sender, EventArgs e)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.OKCancel:
                    result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.YesNo:
                    result = MessageBoxResult.Yes;
                    break;
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.Yes;
                    break;
            }
            Exit();
        }
        private void btn2_Clicked(object sender, EventArgs e)
        {
            switch (buttons)
            {
                case MessageBoxButton.OKCancel:
                    result = MessageBoxResult.Cancel;
                    break;
                case MessageBoxButton.YesNo:
                    result = MessageBoxResult.No;
                    break;
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.No;
                    break;
            }
            Exit();
        }
        private void btn3_Clicked(object sender, EventArgs e)
        {
            switch (buttons)
            {
                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.Cancel;
                    break;
            }
            Exit();
        }
        private void InputDeviceEvents_PreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.StagingItem.Input.Device is TouchDevice)
            {
                InputReportEventArgs irea = e.StagingItem.Input as InputReportEventArgs;
                if (irea != null)
                {
                    RawTouchInputReport rtir = irea.Report as RawTouchInputReport;
                    if (rtir != null)
                    {
                        int x = rtir.Touches[0].X;
                        int y = rtir.Touches[0].Y;
                        PointToClient(ref x, ref y);
                        if (!Utils.IsWithinRectangle(x, y, ActualWidth, ActualWidth))
                            e.Cancel();
                    }
                }
            }
        }
        #endregion

        #region Private methods
        private void Exit()
        {
            TouchCapture.Capture(this, CaptureMode.None);
            Visibility = Visibility.Collapsed;
            InputManager.CurrentInputManager.InputDeviceEvents[(int)InputManager.InputDeviceType.Touch].PreProcessInput -= new PreProcessInputEventHandler(InputDeviceEvents_PreProcessInput);

            if (onClose != null)
                onClose(this, EventArgs.Empty);
        }
        #endregion
    }
}