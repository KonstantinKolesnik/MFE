using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MFE.CustomEmulator
{
    public partial class DialogInsertMedia : Form
    {
        #region Fields
        private bool _createNewMedia = true;
        private string _filePath = null;
        private uint _bytesPerSector = 0;
        private uint _sectorsPerBlock = 0;
        private uint _numBlocks = 0;
        private uint _serialNumber = 0;
        #endregion

        #region Properties
        public bool CreateNewMedia
        {
            get { return _createNewMedia; }
        }
        public uint BytesPerSector
        {
            get { return _bytesPerSector; }
        }
        public uint SectorsPerBlock
        {
            get { return _sectorsPerBlock; }
        }
        public uint NumBlocks
        {
            get { return _numBlocks; }
        }
        public uint SerialNumber
        {
            get { return _serialNumber; }
        }
        public string FilePath
        {
            get { return _filePath; }
        }
        #endregion

        public DialogInsertMedia()
        {
            InitializeComponent();
        }

        private void DialogInsertMedia_Load(object sender, EventArgs e)
        {
            BytesPerSectorComboBox.SelectedIndex = 8;
            UpdateSize();
        }
        
        private void OpenExistingRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (openFileDialogBox.ShowDialog() == DialogResult.OK)
            {
                _createNewMedia = false;
                _filePath = openFileDialogBox.FileName;
                DialogResult = DialogResult.OK;
            }
            else
                DialogResult = DialogResult.Cancel;

            Close();
        }

        private void SectorsPerBlockTextBox_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !IsValidNumber(SectorsPerBlockTextBox.Text);
        }
        private void SectorsPerBlockTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }
        private void SectorsPerBlockTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessNumericTextBoxKeyDown(e);
        }

        private void NumberOfBlocksTextBox_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !IsValidNumber(NumberOfBlocksTextBox.Text);
        }
        private void NumberOfBlocksTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }
        private void NumberOfBlocksTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessNumericTextBoxKeyDown(e);
        }

        private void SerialNumberTextBox_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !IsValidNumber(SerialNumberTextBox.Text);
        }
        private void SerialNumberTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }
        private void SerialNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessNumericTextBoxKeyDown(e);
        }

        private void BytesPerSectorComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialogBox.ShowDialog() == DialogResult.OK)
                FilePathTextBox.Text = saveFileDialogBox.FileName;
        }
        private void OKButton_Click(object sender, EventArgs e)
        {
            _filePath = FilePathTextBox.Text;
            _bytesPerSector = Convert.ToUInt32(BytesPerSectorComboBox.Text);
            _sectorsPerBlock = Convert.ToUInt32(SectorsPerBlockTextBox.Text);
            _numBlocks = Convert.ToUInt32(NumberOfBlocksTextBox.Text);
            _serialNumber = Convert.ToUInt32(SerialNumberTextBox.Text);
        }

        /// <summary>
        /// Update total size value, use KB, MB, GB etc.
        /// </summary>
        private void UpdateSize()
        {
            try
            {
                uint bytesPerSector = Convert.ToUInt32(BytesPerSectorComboBox.Text);
                uint sectorsPerBlock = Convert.ToUInt32(SectorsPerBlockTextBox.Text);
                uint numBlocks = Convert.ToUInt32(NumberOfBlocksTextBox.Text);

                double size = ((double)bytesPerSector * sectorsPerBlock * numBlocks);

                if (size < 1024)
                    MediaSizeLabel.Text = String.Format("{0:0.} Bytes", size);
                else if (size < (1024 * 1024))
                    MediaSizeLabel.Text = String.Format("{0:0.00} KB", size / 1024);
                else if (size < (1024 * 1024 * 1024))
                    MediaSizeLabel.Text = String.Format("{0:0.00} MB", size / (1024 * 1024));
                else
                    MediaSizeLabel.Text = String.Format("{0:0.00} GB", size / (1024 * 1024 * 1024));
            }
            catch (FormatException)
            {
                MediaSizeLabel.Text = "0";
            }
            catch (OverflowException)
            {
                MediaSizeLabel.Text = "Too large.";
            }
        }

        private bool IsValidNumber(string text)
        {
            try
            {
                uint u = Convert.ToUInt32(text);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Don't let users press non-numeric keys. Keys such as DEL, BKSPACE, 
        /// LEFT, RIGHT are allowed.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessNumericTextBoxKeyDown(KeyEventArgs e)
        {
            if (((e.KeyValue >= '0') && (e.KeyValue <= '9')) ||
                (e.KeyValue == 8) || (e.KeyValue == 46) || (e.KeyValue == 37) ||
                (e.KeyValue == 39))
                e.SuppressKeyPress = false;
            else
                e.SuppressKeyPress = true;
        }
    }
}
