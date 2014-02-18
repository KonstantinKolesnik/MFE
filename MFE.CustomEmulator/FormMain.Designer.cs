namespace MFE.CustomEmulator
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSD = new System.Windows.Forms.TabPage();
            this.lvSD = new System.Windows.Forms.ListView();
            this.colSlot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tlbRBSD = new System.Windows.Forms.ToolStrip();
            this.btnSDInsert = new System.Windows.Forms.ToolStripButton();
            this.btnSDRemove = new System.Windows.Forms.ToolStripButton();
            this.tabSerialPorts = new System.Windows.Forms.TabPage();
            this.lvSerialPorts = new System.Windows.Forms.ListView();
            this.colHandle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tlbSerialPorts = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lcdDisplay = new CustomEmulator.Components.LcdControl();
            this.hwButton9 = new CustomEmulator.Components.HWButton();
            this.hwButton8 = new CustomEmulator.Components.HWButton();
            this.hwButton7 = new CustomEmulator.Components.HWButton();
            this.hwButton6 = new CustomEmulator.Components.HWButton();
            this.hwButton5 = new CustomEmulator.Components.HWButton();
            this.hwButton4 = new CustomEmulator.Components.HWButton();
            this.hwButton3 = new CustomEmulator.Components.HWButton();
            this.hwButton2 = new CustomEmulator.Components.HWButton();
            this.hwButton1 = new CustomEmulator.Components.HWButton();
            this.hwButtonRight = new CustomEmulator.Components.HWButton();
            this.hwButtonSelect = new CustomEmulator.Components.HWButton();
            this.hwButtonUp = new CustomEmulator.Components.HWButton();
            this.hwButtonLeft = new CustomEmulator.Components.HWButton();
            this.hwButtonDown = new CustomEmulator.Components.HWButton();
            this.tabControl1.SuspendLayout();
            this.tabSD.SuspendLayout();
            this.tlbRBSD.SuspendLayout();
            this.tabSerialPorts.SuspendLayout();
            this.tlbSerialPorts.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabSD);
            this.tabControl1.Controls.Add(this.tabSerialPorts);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(260, 480);
            this.tabControl1.TabIndex = 1;
            // 
            // tabSD
            // 
            this.tabSD.Controls.Add(this.lvSD);
            this.tabSD.Controls.Add(this.tlbRBSD);
            this.tabSD.Location = new System.Drawing.Point(4, 22);
            this.tabSD.Name = "tabSD";
            this.tabSD.Padding = new System.Windows.Forms.Padding(3);
            this.tabSD.Size = new System.Drawing.Size(252, 454);
            this.tabSD.TabIndex = 0;
            this.tabSD.Text = "Removable Media";
            this.tabSD.UseVisualStyleBackColor = true;
            // 
            // lvSD
            // 
            this.lvSD.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSlot});
            this.lvSD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSD.FullRowSelect = true;
            this.lvSD.HideSelection = false;
            this.lvSD.Location = new System.Drawing.Point(3, 28);
            this.lvSD.MultiSelect = false;
            this.lvSD.Name = "lvSD";
            this.lvSD.Size = new System.Drawing.Size(246, 423);
            this.lvSD.TabIndex = 2;
            this.lvSD.UseCompatibleStateImageBehavior = false;
            this.lvSD.View = System.Windows.Forms.View.Details;
            this.lvSD.SelectedIndexChanged += new System.EventHandler(this.lvSD_SelectedIndexChanged);
            // 
            // colSlot
            // 
            this.colSlot.Text = "Slot";
            this.colSlot.Width = 118;
            // 
            // tlbRBSD
            // 
            this.tlbRBSD.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSDInsert,
            this.btnSDRemove});
            this.tlbRBSD.Location = new System.Drawing.Point(3, 3);
            this.tlbRBSD.Name = "tlbRBSD";
            this.tlbRBSD.Size = new System.Drawing.Size(246, 25);
            this.tlbRBSD.TabIndex = 1;
            this.tlbRBSD.Text = "toolStrip2";
            // 
            // btnSDInsert
            // 
            this.btnSDInsert.Enabled = false;
            this.btnSDInsert.Image = ((System.Drawing.Image)(resources.GetObject("btnSDInsert.Image")));
            this.btnSDInsert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSDInsert.Name = "btnSDInsert";
            this.btnSDInsert.Size = new System.Drawing.Size(56, 22);
            this.btnSDInsert.Text = "Insert";
            this.btnSDInsert.Click += new System.EventHandler(this.btnSDInsertRemove_Click);
            // 
            // btnSDRemove
            // 
            this.btnSDRemove.Enabled = false;
            this.btnSDRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnSDRemove.Image")));
            this.btnSDRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSDRemove.Name = "btnSDRemove";
            this.btnSDRemove.Size = new System.Drawing.Size(70, 22);
            this.btnSDRemove.Text = "Remove";
            this.btnSDRemove.Click += new System.EventHandler(this.btnSDInsertRemove_Click);
            // 
            // tabSerialPorts
            // 
            this.tabSerialPorts.Controls.Add(this.lvSerialPorts);
            this.tabSerialPorts.Controls.Add(this.tlbSerialPorts);
            this.tabSerialPorts.Location = new System.Drawing.Point(4, 22);
            this.tabSerialPorts.Name = "tabSerialPorts";
            this.tabSerialPorts.Padding = new System.Windows.Forms.Padding(3);
            this.tabSerialPorts.Size = new System.Drawing.Size(252, 454);
            this.tabSerialPorts.TabIndex = 1;
            this.tabSerialPorts.Text = "Serial Ports";
            this.tabSerialPorts.UseVisualStyleBackColor = true;
            // 
            // lvSerialPorts
            // 
            this.lvSerialPorts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colHandle,
            this.colID});
            this.lvSerialPorts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSerialPorts.FullRowSelect = true;
            this.lvSerialPorts.HideSelection = false;
            this.lvSerialPorts.Location = new System.Drawing.Point(3, 28);
            this.lvSerialPorts.MultiSelect = false;
            this.lvSerialPorts.Name = "lvSerialPorts";
            this.lvSerialPorts.Size = new System.Drawing.Size(246, 423);
            this.lvSerialPorts.TabIndex = 2;
            this.lvSerialPorts.UseCompatibleStateImageBehavior = false;
            this.lvSerialPorts.View = System.Windows.Forms.View.Details;
            // 
            // colHandle
            // 
            this.colHandle.Text = "Handle";
            this.colHandle.Width = 107;
            // 
            // colID
            // 
            this.colID.Text = "ID";
            this.colID.Width = 90;
            // 
            // tlbSerialPorts
            // 
            this.tlbSerialPorts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
            this.tlbSerialPorts.Location = new System.Drawing.Point(3, 3);
            this.tlbSerialPorts.Name = "tlbSerialPorts";
            this.tlbSerialPorts.Size = new System.Drawing.Size(246, 25);
            this.tlbSerialPorts.TabIndex = 1;
            this.tlbSerialPorts.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.hwButton9);
            this.tabPage1.Controls.Add(this.hwButton8);
            this.tabPage1.Controls.Add(this.hwButton7);
            this.tabPage1.Controls.Add(this.hwButton6);
            this.tabPage1.Controls.Add(this.hwButton5);
            this.tabPage1.Controls.Add(this.hwButton4);
            this.tabPage1.Controls.Add(this.hwButton3);
            this.tabPage1.Controls.Add(this.hwButton2);
            this.tabPage1.Controls.Add(this.hwButton1);
            this.tabPage1.Controls.Add(this.hwButtonRight);
            this.tabPage1.Controls.Add(this.hwButtonSelect);
            this.tabPage1.Controls.Add(this.hwButtonUp);
            this.tabPage1.Controls.Add(this.hwButtonLeft);
            this.tabPage1.Controls.Add(this.hwButtonDown);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(252, 454);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Buttons";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lcdDisplay
            // 
            this.lcdDisplay.BackColor = System.Drawing.Color.White;
            this.lcdDisplay.LcdDisplay = null;
            this.lcdDisplay.Location = new System.Drawing.Point(278, 12);
            this.lcdDisplay.Name = "lcdDisplay";
            this.lcdDisplay.Size = new System.Drawing.Size(800, 480);
            this.lcdDisplay.TabIndex = 2;
            this.lcdDisplay.TouchPort = null;
            // 
            // hwButton9
            // 
            this.hwButton9.Location = new System.Drawing.Point(168, 151);
            this.hwButton9.Name = "hwButton9";
            this.hwButton9.Port = null;
            this.hwButton9.Size = new System.Drawing.Size(75, 23);
            this.hwButton9.TabIndex = 14;
            this.hwButton9.Text = "9";
            this.hwButton9.UseVisualStyleBackColor = true;
            // 
            // hwButton8
            // 
            this.hwButton8.Location = new System.Drawing.Point(87, 151);
            this.hwButton8.Name = "hwButton8";
            this.hwButton8.Port = null;
            this.hwButton8.Size = new System.Drawing.Size(75, 23);
            this.hwButton8.TabIndex = 13;
            this.hwButton8.Text = "8";
            this.hwButton8.UseVisualStyleBackColor = true;
            // 
            // hwButton7
            // 
            this.hwButton7.Location = new System.Drawing.Point(6, 151);
            this.hwButton7.Name = "hwButton7";
            this.hwButton7.Port = null;
            this.hwButton7.Size = new System.Drawing.Size(75, 23);
            this.hwButton7.TabIndex = 12;
            this.hwButton7.Text = "7";
            this.hwButton7.UseVisualStyleBackColor = true;
            // 
            // hwButton6
            // 
            this.hwButton6.Location = new System.Drawing.Point(168, 122);
            this.hwButton6.Name = "hwButton6";
            this.hwButton6.Port = null;
            this.hwButton6.Size = new System.Drawing.Size(75, 23);
            this.hwButton6.TabIndex = 11;
            this.hwButton6.Text = "6";
            this.hwButton6.UseVisualStyleBackColor = true;
            // 
            // hwButton5
            // 
            this.hwButton5.Location = new System.Drawing.Point(87, 122);
            this.hwButton5.Name = "hwButton5";
            this.hwButton5.Port = null;
            this.hwButton5.Size = new System.Drawing.Size(75, 23);
            this.hwButton5.TabIndex = 10;
            this.hwButton5.Text = "5";
            this.hwButton5.UseVisualStyleBackColor = true;
            // 
            // hwButton4
            // 
            this.hwButton4.Location = new System.Drawing.Point(6, 122);
            this.hwButton4.Name = "hwButton4";
            this.hwButton4.Port = null;
            this.hwButton4.Size = new System.Drawing.Size(75, 23);
            this.hwButton4.TabIndex = 9;
            this.hwButton4.Text = "4";
            this.hwButton4.UseVisualStyleBackColor = true;
            // 
            // hwButton3
            // 
            this.hwButton3.Location = new System.Drawing.Point(168, 93);
            this.hwButton3.Name = "hwButton3";
            this.hwButton3.Port = null;
            this.hwButton3.Size = new System.Drawing.Size(75, 23);
            this.hwButton3.TabIndex = 8;
            this.hwButton3.Text = "3";
            this.hwButton3.UseVisualStyleBackColor = true;
            // 
            // hwButton2
            // 
            this.hwButton2.Location = new System.Drawing.Point(87, 93);
            this.hwButton2.Name = "hwButton2";
            this.hwButton2.Port = null;
            this.hwButton2.Size = new System.Drawing.Size(75, 23);
            this.hwButton2.TabIndex = 7;
            this.hwButton2.Text = "2";
            this.hwButton2.UseVisualStyleBackColor = true;
            // 
            // hwButton1
            // 
            this.hwButton1.Location = new System.Drawing.Point(6, 93);
            this.hwButton1.Name = "hwButton1";
            this.hwButton1.Port = null;
            this.hwButton1.Size = new System.Drawing.Size(75, 23);
            this.hwButton1.TabIndex = 6;
            this.hwButton1.Text = "1";
            this.hwButton1.UseVisualStyleBackColor = true;
            // 
            // hwButtonRight
            // 
            this.hwButtonRight.Location = new System.Drawing.Point(168, 35);
            this.hwButtonRight.Name = "hwButtonRight";
            this.hwButtonRight.Port = null;
            this.hwButtonRight.Size = new System.Drawing.Size(75, 23);
            this.hwButtonRight.TabIndex = 5;
            this.hwButtonRight.Text = "Right";
            this.hwButtonRight.UseVisualStyleBackColor = true;
            // 
            // hwButtonSelect
            // 
            this.hwButtonSelect.Location = new System.Drawing.Point(87, 35);
            this.hwButtonSelect.Name = "hwButtonSelect";
            this.hwButtonSelect.Port = null;
            this.hwButtonSelect.Size = new System.Drawing.Size(75, 23);
            this.hwButtonSelect.TabIndex = 4;
            this.hwButtonSelect.Text = "Select";
            this.hwButtonSelect.UseVisualStyleBackColor = true;
            // 
            // hwButtonUp
            // 
            this.hwButtonUp.Location = new System.Drawing.Point(87, 6);
            this.hwButtonUp.Name = "hwButtonUp";
            this.hwButtonUp.Port = null;
            this.hwButtonUp.Size = new System.Drawing.Size(75, 23);
            this.hwButtonUp.TabIndex = 3;
            this.hwButtonUp.Text = "Up";
            this.hwButtonUp.UseVisualStyleBackColor = true;
            // 
            // hwButtonLeft
            // 
            this.hwButtonLeft.Location = new System.Drawing.Point(6, 35);
            this.hwButtonLeft.Name = "hwButtonLeft";
            this.hwButtonLeft.Port = null;
            this.hwButtonLeft.Size = new System.Drawing.Size(75, 23);
            this.hwButtonLeft.TabIndex = 2;
            this.hwButtonLeft.Text = "Left";
            this.hwButtonLeft.UseVisualStyleBackColor = true;
            // 
            // hwButtonDown
            // 
            this.hwButtonDown.Location = new System.Drawing.Point(87, 64);
            this.hwButtonDown.Name = "hwButtonDown";
            this.hwButtonDown.Port = null;
            this.hwButtonDown.Size = new System.Drawing.Size(75, 23);
            this.hwButtonDown.TabIndex = 1;
            this.hwButtonDown.Text = "Down";
            this.hwButtonDown.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1089, 498);
            this.Controls.Add(this.lcdDisplay);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Emulator";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabSD.ResumeLayout(false);
            this.tabSD.PerformLayout();
            this.tlbRBSD.ResumeLayout(false);
            this.tlbRBSD.PerformLayout();
            this.tabSerialPorts.ResumeLayout(false);
            this.tabSerialPorts.PerformLayout();
            this.tlbSerialPorts.ResumeLayout(false);
            this.tlbSerialPorts.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSD;
        private System.Windows.Forms.TabPage tabSerialPorts;
        private System.Windows.Forms.ToolStrip tlbSerialPorts;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ListView lvSerialPorts;
        private System.Windows.Forms.ColumnHeader colHandle;
        private System.Windows.Forms.ColumnHeader colID;
        private System.Windows.Forms.ToolStrip tlbRBSD;
        private System.Windows.Forms.ToolStripButton btnSDInsert;
        private System.Windows.Forms.ToolStripButton btnSDRemove;
        private System.Windows.Forms.ListView lvSD;
        private System.Windows.Forms.ColumnHeader colSlot;
        private Components.LcdControl lcdDisplay;
        private System.Windows.Forms.TabPage tabPage1;
        private Components.HWButton hwButtonSelect;
        private Components.HWButton hwButtonUp;
        private Components.HWButton hwButtonLeft;
        private Components.HWButton hwButtonDown;
        private Components.HWButton hwButtonRight;
        private Components.HWButton hwButton1;
        private Components.HWButton hwButton3;
        private Components.HWButton hwButton2;
        private Components.HWButton hwButton6;
        private Components.HWButton hwButton5;
        private Components.HWButton hwButton4;
        private Components.HWButton hwButton9;
        private Components.HWButton hwButton8;
        private Components.HWButton hwButton7;
    }
}

