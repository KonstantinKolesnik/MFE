using MFE.CustomEmulator.Components;
using Microsoft.SPOT.Emulator;
using Microsoft.SPOT.Emulator.BlockStorage;
using Microsoft.SPOT.Emulator.Com;
using Microsoft.SPOT.Emulator.Gpio;
using Microsoft.SPOT.Emulator.TouchPanel;
using System;
using System.Windows.Forms;

namespace MFE.CustomEmulator
{
    public partial class FormMain : Form
    {
        private Emulator emulator;

        public FormMain(Emulator emulator)
        {
            this.emulator = emulator;
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            PopulateSD();
            PopulateSerialPorts();
        }

        #region SD cards
        private void PopulateSD()
        {
            foreach (BlockStorageDevice bsd in emulator.BlockStorageDevices)
            {
                if (bsd is EmulatorRemovableBlockStorageDevice)
                {
                    EmulatorRemovableBlockStorageDevice rbsd = bsd as EmulatorRemovableBlockStorageDevice;
                    if (rbsd != null)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = rbsd.Namespace;
                        lvi.Tag = rbsd;
                        lvSD.Items.Add(lvi);

                        PopulateSDInfo(lvi);
                    }
                }
            }
            btnSDInsert.Enabled = btnSDRemove.Enabled = false;
        }
        private void PopulateSDInfo(ListViewItem lvi)
        {
            btnSDInsert.Enabled = btnSDRemove.Enabled = false;
            if (lvi != null)
            {
                EmulatorRemovableBlockStorageDevice rbsd = lvi.Tag as EmulatorRemovableBlockStorageDevice;
                if (rbsd != null)
                {
                    btnSDInsert.Enabled = !rbsd.Inserted;
                    btnSDRemove.Enabled = rbsd.Inserted;


                    //lvi.SubItems.Add();
                }
            }
        }
        private void lvSD_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSDInfo(lvSD.SelectedItems.Count != 0 ? lvSD.SelectedItems[0] : null);
        }
        private void btnSDInsertRemove_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = lvSD.SelectedItems.Count != 0 ? lvSD.SelectedItems[0] : null;
            if (lvi != null)
            {
                EmulatorRemovableBlockStorageDevice rbsd = lvi.Tag as EmulatorRemovableBlockStorageDevice;
                try
                {
                    if (rbsd.Inserted)
                        rbsd.Eject();
                    else
                    {
                        DialogInsertMedia dlg = new DialogInsertMedia();
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            if (dlg.CreateNewMedia)
                                rbsd.Insert(dlg.FilePath, dlg.SectorsPerBlock, dlg.BytesPerSector, dlg.NumBlocks, dlg.SerialNumber);
                            else
                                rbsd.Insert(dlg.FilePath);
                        }
                    }
                    PopulateSDInfo(lvi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        #region Serial ports
        private void PopulateSerialPorts()
        {
            //List<VirtualSerialPort> espList = new List<VirtualSerialPort>();
            //foreach (ComPort serialPort in emulator.SerialPorts)
            //{
            //    if (serialPort is VirtualSerialPort)
            //        espList.Add(serialPort as VirtualSerialPort);
            //}
            //emulatorSPs = new Dictionary<String, VirtualSerialPort>();
            //if (espList.Count > 0)
            //{
            //    // Create a menu item for each EmulatorSerialPort
            //    foreach (VirtualSerialPort serialPort in espList)
            //    {
            //        String itemText = "Write to " + serialPort.ComPortHandle + " (" + serialPort.ComponentId + ")";
            //        ToolStripItem item = new ToolStripMenuItem(itemText, null, WriteToEmulatorSerialPortOnClick);
            //        item.Name = serialPort.ComPortHandle.ToString();
            //        espToolStripMenuItem.DropDownItems.Add(item);
            //        emulatorSPs.Add(item.Name, serialPort);
            //    }
            //}



            foreach (ComPort serialPort in emulator.SerialPorts)
            {
                if (serialPort is VirtualSerialPort)
                {
                    VirtualSerialPort vsp = serialPort as VirtualSerialPort;

                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = vsp.ComPortHandle.ToString();
                    lvi.SubItems.Add(vsp.ComponentId);
                    lvi.Tag = vsp;

                    lvSerialPorts.Items.Add(lvi);
                }
            }
        }
        /// <summary>
        /// OnClick handler for "Write To Emulator Serial Port" menu item. It launches a 
        /// SendToSerialPortDialogBox to get the string, and send the string to the emulator
        /// serial port using its StreamOut stream (from the inherited ComPortToMemoryStream).
        /// </summary>
        /// <param name="sender">the menu item that was clicked</param>
        /// <param name="e">Not used</param>
        private void WriteToEmulatorSerialPortOnClick(Object sender, EventArgs e)
        {
            //ToolStripItem item = sender as ToolStripItem;
            //if (item != null)
            //{
            //    EmulatorSerialPort esp = emulatorSPs[item.Name];

            //    try
            //    {
            //        SendToSerialPortDialogBox sendToSerialPortDialogBox = new SendToSerialPortDialogBox();

            //        if (sendToSerialPortDialogBox.ShowDialog() == DialogResult.OK)
            //        {
            //            byte[] data = Encoding.UTF8.GetBytes(sendToSerialPortDialogBox.TextToSend);
            //            esp.StreamOut.Write(data, 0, data.Length);
            //            esp.StreamOut.Flush();
            //        }
            //    }
            //    catch (Exception exception)
            //    {
            //        MessageBox.Show(exception.ToString());
            //        return;
            //    }
            //}
        }
        #endregion

        public void OnInitializeComponent()
        {
            // Initialize the LCD control with the LCD emulator component.
            lcdDisplay.LcdDisplay = emulator.LcdDisplay;
            lcdDisplay.Width = emulator.LcdDisplay.Width;
            lcdDisplay.Height = emulator.LcdDisplay.Height;
            lcdDisplay.TouchPort = (TouchGpioPort)emulator.GpioPorts[TouchGpioPort.DefaultTouchPin];

            Invalidate();

            // Read the GPIO pins from the Emulator.config file. This allows 
            // overriding the specific GPIO pin numbers, for example, without 
            // having to change code.
            BindButton(hwButtonLeft, "Pin_Left");
            BindButton(hwButtonRight, "Pin_Right");
            BindButton(hwButtonUp, "Pin_Up");
            BindButton(hwButtonDown, "Pin_Down");
            BindButton(hwButtonSelect, "Pin_Select");
        }

        private void BindButton(HWButton button, string componentId)
        {
            button.Port = emulator.FindComponentById(componentId) as GpioPort;
        }
    }
}
