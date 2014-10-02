using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using MFE.Graphics.Presentation;

namespace Typhoon.MF.Presentation.Controls
{
    public class UpDownComboBox : MFE.Graphics.Presentation.StackPanel
    {
        #region Fields
        private Microsoft.SPOT.Presentation.Controls.Text text;
        private Button btnUp;
        private Button btnDown;
        private ArrayList items = new ArrayList();
        private int selectedIdx = -1;
        #endregion

        #region Properties
        public ArrayList Items
        {
            get { return items; }
        }
        public int SelectedIdx
        {
            get { return selectedIdx; }
            set
            {
                CheckAccess();

                value = System.Math.Max(value, 0);
                value = System.Math.Min(value, items.Count - 1);

                if (selectedIdx != value)
                {
                    selectedIdx = value;
                    text.TextContent = items[selectedIdx].ToString();
                }
            }
        }
        public Button ButtonDown
        {
            get { return btnDown; }
        }
        public Button ButtonUp
        {
            get { return btnUp; }
        }
        #endregion

        #region Constructor
        public UpDownComboBox(Font font, int width)
        {
            Width = width;
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            btnDown = new Button(font, " < ", null, Colors.White);
            btnDown.ShowBorder = true;
            btnDown.HorizontalAlignment = HorizontalAlignment.Left;
            btnDown.Clicked += new EventHandler(btnDown_Clicked);
            Children.Add(btnDown);

            text = new Text(font, "");
            text.ForeColor = Color.White;
            text.HorizontalAlignment = HorizontalAlignment.Stretch;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.TextAlignment = TextAlignment.Center;
            text.TextWrap = false;
            Children.Add(text);

            btnUp = new Button(font, " > ", null, Colors.White);
            btnUp.ShowBorder = true;
            btnUp.HorizontalAlignment = HorizontalAlignment.Right;
            btnUp.Clicked += new EventHandler(btnUp_Clicked);
            Children.Add(btnUp);
        }
        #endregion

        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(null, new Pen(Colors.Gray), 0, 0, ActualWidth, ActualHeight);
        }
        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            text.Width = Width - btnDown.Width - btnUp.Width;
            base.MeasureOverride(availableWidth, availableHeight, out desiredWidth, out desiredHeight);
        }

        #region Event handlers
        private void btnDown_Clicked(object sender, EventArgs e)
        {
            SelectedIdx--;
        }
        private void btnUp_Clicked(object sender, EventArgs e)
        {
            SelectedIdx++;
        }
        #endregion
    }
}
