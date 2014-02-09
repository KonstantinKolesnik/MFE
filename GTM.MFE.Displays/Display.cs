using Gadgeteer;
using Gadgeteer.Modules;
using GHI.Premium.System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;
using GT = Gadgeteer;
using GTI = Gadgeteer.Interfaces;

namespace GTM.MFE.Displays
{
    //N18 =>    ITDB18SP        ST7735
    //          TFT01_22SP      ILI9341	(Serial 5Pin)

    /// <summary>
    /// A Display module for Microsoft .NET Gadgeteer
    /// </summary>
    public class Display : Module.DisplayModule
    {
        #region Fields
        byte			fch, fcl, bch, bcl;
		byte			orient;
		long			disp_x_size, disp_y_size;
		byte			display_model, display_transfer_mode, display_serial_mode;
        //regtype			*P_RS, *P_WR, *P_CS, *P_RST, *P_SDA, *P_SCL, *P_ALE;
        //regsize			B_RS, B_WR, B_CS, B_RST, B_SDA, B_SCL, B_ALE;
        //_current_font	cfont;
		bool			_transparent;



        private GTI.SPI spi;
        private GTI.SPI.Configuration spiConfig;
        private SPI.Configuration netMFSpiConfig;
        private GT.Socket socket;

        // chip select - pin 6
        private GTI.DigitalOutput pinReset;
        private GTI.DigitalOutput pinBacklight;
        private GTI.DigitalOutput pinDc; // TFT D/C

        private ushort[] shortArray = new ushort[2];
        #endregion

        #region Properties
        /// <summary>
        /// Gets the width of the display.
        /// </summary>
        /// <remarks>
        /// This property always returns 128.
        /// </remarks>
        public override uint Width
        {
            get { return 240; }
        }

        /// <summary>
        /// Gets the height of the display.
        /// </summary>
        /// <remarks>
        /// This property always returns 160.
        /// </remarks>
        public override uint Height
        {
            get { return 320; }
        }
        #endregion

        #region Constructor
        /// <summary>Constructor</summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public Display(ModelType model, int socketNumber)
            : base(WPFRenderOptions.Intercept)
        {
            #region UTFT
            ushort[] dsx = {239, 239, 239, 239, 239, 239, 175, 175, 239, 127, 127, 239, 271, 479, 239, 239, 239, 239, 239, 239, 479, 319, 239, 175, 127, 239, 239, 319, 319};
            ushort[] dsy = {319, 399, 319, 319, 319, 319, 219, 219, 399, 159, 127, 319, 479, 799, 319, 319, 319, 319, 319, 319, 799, 479, 319, 219, 159, 319, 319, 479, 479};
            byte[] dtm = { 16, 16, 16, 8, 8, 16, 8, (byte)DisplayTransferMode.SERIAL_4PIN, 16, (byte)DisplayTransferMode.SERIAL_5PIN, (byte)DisplayTransferMode.SERIAL_5PIN, 16, 16, 16, 8, 16, (byte)DisplayTransferMode.LATCHED_16, 8, 16, 8, 16, 16, 16, 8, (byte)DisplayTransferMode.SERIAL_5PIN, (byte)DisplayTransferMode.SERIAL_5PIN, (byte)DisplayTransferMode.SERIAL_4PIN, 16, 16 };

            disp_x_size = dsx[(byte)model];
            disp_y_size = dsy[(byte)model];
            display_transfer_mode = dtm[(byte)model];
            display_model = (byte)model;

            if (display_transfer_mode == (byte)DisplayTransferMode.SERIAL_4PIN)
            {
                display_transfer_mode = 1;
                display_serial_mode = (byte)DisplayTransferMode.SERIAL_4PIN;
            }
            if (display_transfer_mode == (byte)DisplayTransferMode.SERIAL_5PIN)
            {
                display_transfer_mode = 1;
                display_serial_mode = (byte)DisplayTransferMode.SERIAL_5PIN;
            }

            if (display_transfer_mode != 1)
            {
                //_set_direction_registers(display_transfer_mode);
                //P_RS = portOutputRegister(digitalPinToPort(RS));
                //B_RS = digitalPinToBitMask(RS);
                //P_WR = portOutputRegister(digitalPinToPort(WR));
                //B_WR = digitalPinToBitMask(WR);
                //P_CS = portOutputRegister(digitalPinToPort(CS));
                //B_CS = digitalPinToBitMask(CS);
                //P_RST = portOutputRegister(digitalPinToPort(RST));
                //B_RST = digitalPinToBitMask(RST);
                //if (display_transfer_mode == LATCHED_16)
                //{
                //    P_ALE = portOutputRegister(digitalPinToPort(SER));
                //    B_ALE = digitalPinToBitMask(SER);
                //    pinMode(SER, OUTPUT);
                //    cbi(P_ALE, B_ALE);
                //    pinMode(8, OUTPUT);
                //    digitalWrite(8, LOW);
                //}
                //pinMode(RS, OUTPUT);
                //pinMode(WR, OUTPUT);
                //pinMode(CS, OUTPUT);
                //pinMode(RST, OUTPUT);
            }
            else
            {
                //P_SDA = portOutputRegister(digitalPinToPort(RS));
                //B_SDA = digitalPinToBitMask(RS);
                //P_SCL = portOutputRegister(digitalPinToPort(WR));
                //B_SCL = digitalPinToBitMask(WR);
                //P_CS = portOutputRegister(digitalPinToPort(CS));
                //B_CS = digitalPinToBitMask(CS);
                //P_RST = portOutputRegister(digitalPinToPort(RST));
                //B_RST = digitalPinToBitMask(RST);
                //if (display_serial_mode != SERIAL_4PIN)
                //{
                //    P_RS = portOutputRegister(digitalPinToPort(SER));
                //    B_RS = digitalPinToBitMask(SER);
                //    pinMode(SER, OUTPUT);
                //}
                //pinMode(RS, OUTPUT);
                //pinMode(WR, OUTPUT);
                //pinMode(CS, OUTPUT);
                //pinMode(RST, OUTPUT);
            }
            #endregion

            socket = Socket.GetSocket(socketNumber, true, this, null);
            socket.EnsureTypeIsSupported('S', this);
            /*
             * Serial peripheral interface (SPI).
             * Pin 7 is MOSI line, pin 8 is MISO line, pin 9 is SCK line.
             * In addition, pins 3, 4 and 5 are general-purpose input/outputs, with pin 3 supporting interrupt capabilities.
            */

            pinReset = new GTI.DigitalOutput(socket, Socket.Pin.Three, false, this); // pin 3
			pinBacklight = new GTI.DigitalOutput(socket, Socket.Pin.Four, false, this); // pin 4
            pinDc = new GTI.DigitalOutput(socket, Socket.Pin.Five, false, this); // pin 5

            spiConfig = new GTI.SPI.Configuration(false, 0, 0, false, true, 12000);
            netMFSpiConfig = new SPI.Configuration(socket.CpuPins[6], spiConfig.ChipSelectActiveState, spiConfig.ChipSelectSetupTime, spiConfig.ChipSelectHoldTime, spiConfig.ClockIdleState, spiConfig.ClockEdge, spiConfig.ClockRateKHz, socket.SPIModule);
            spi = new GTI.SPI(socket, spiConfig, GTI.SPI.Sharing.Shared, socket, Socket.Pin.Six, this);

            Reset();
            ConfigureDisplay();
            Clear();
            SetBacklight(true);
		}
        #endregion

        #region Public methods
        /// <summary>
        /// Enables or disables the display backlight.
        /// </summary>
        /// <param name="state">The state to set the backlight to.</param>
        public void SetBacklight(bool state)
        {
            pinBacklight.Write(state);
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void Clear()
        {
            uint w = Width / 2;
            uint h = Height / 2;

            byte[] data = new byte[w * h * 2]; //zero-init'd by default

            DrawRaw(data, w, h, 0, 0);
            DrawRaw(data, w, h, w, 0);
            DrawRaw(data, w, h, 0, h);
            DrawRaw(data, w, h, w, h);
        }

        /// <summary>
        /// Draws an image to the screen.
        /// </summary>
        /// <param name="bmp">The bitmap to be drawn to the screen</param>
        /// <param name="x">Starting X position of the image.</param>
        /// <param name="y">Starting Y position of the image.</param>
        public void Draw(Bitmap bmp, uint x = 0, uint y = 0)
        {
            if (Module.Mainboard.NativeBitmapConverter == null)
                Module.Mainboard.NativeBitmapConverter = new Mainboard.BitmapConvertBPP(BitmapConverter);

            byte[] rawData = new byte[bmp.Width * bmp.Height * 2];
            Module.Mainboard.NativeBitmapConverter(bmp.GetBitmap(), rawData, Mainboard.BPP.BPP16_BGR_BE);
            DrawRaw(rawData, (uint)bmp.Width, (uint)bmp.Height, x, y);
        }

        /// <summary>
        /// Draws an image to the specified position on the screen.
        /// </summary>
        /// <param name="rawData">Raw Bitmap data to be drawn to the screen.</param>
        /// <param name="x">Starting X position of the image.</param>
        /// <param name="y">Starting Y position of the image.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        public void DrawRaw(byte[] rawData, uint width, uint height, uint x, uint y)
        {
            if (x > Width || y > Height)
                return;

            if (x + width > Width)
                width = Width - x;
            if (y + height > Height)
                height = Height - y;

            SetClippingArea(x, y, width - 1, height - 1);
            WriteCommand(0x2C);
            WriteData(rawData);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Renders Bitmap data on the display device. 
        /// </summary>
        /// <param name="bitmap">The <see cref="T:Microsoft.SPOT.Bitmap"/> object to render on the display.</param>
        protected override void Paint(Bitmap bitmap)
        {
            try
            {
                if (Mainboard.NativeBitmapCopyToSpi != null)
                {
                    SetClippingArea(0, 0, (uint)bitmap.Width - 1, (uint)bitmap.Height - 1);
                    WriteCommand(0x2C);
                    pinDc.Write(true);
                    Mainboard.NativeBitmapCopyToSpi(bitmap, netMFSpiConfig, 0, 0, bitmap.Width, bitmap.Height, GT.Mainboard.BPP.BPP16_BGR_BE);
                }
                else
                    Draw(bitmap);
            }
            catch
            {
                this.ErrorPrint("Painting error");
            }
        }

        private void Reset()
        {
            pinReset.Write(false);
            Thread.Sleep(150);
            pinReset.Write(true);
        }

        private void ConfigureDisplay()
        {
            Mainboard.LCDConfiguration lcdConfig = new Mainboard.LCDConfiguration();
            lcdConfig.LCDControllerEnabled = false;
            lcdConfig.Width = Width;
            lcdConfig.Height = Height;
            DisplayModule.SetLCDConfig(lcdConfig);


            #region N22
            WriteCommand(0xCB);
            WriteData(0x39);
            WriteData(0x2C);
            WriteData(0x00);
            WriteData(0x34);
            WriteData(0x02);

            WriteCommand(0xCF);
            WriteData(0x00);
            WriteData(0XC1);
            WriteData(0X30);

            WriteCommand(0xE8);
            WriteData(0x85);
            WriteData(0x00);
            WriteData(0x78);

            WriteCommand(0xEA);
            WriteData(0x00);
            WriteData(0x00);

            WriteCommand(0xED);
            WriteData(0x64);
            WriteData(0x03);
            WriteData(0X12);
            WriteData(0X81);

            WriteCommand(0xF7);
            WriteData(0x20);

            WriteCommand(0xC0);    //Power control 
            WriteData(0x23);   //VRH[5:0] 

            WriteCommand(0xC1);    //Power control 
            WriteData(0x10);   //SAP[2:0];BT[3:0] 

            WriteCommand(0xC5);    //VCM control 
            WriteData(0x3e);   //Contrast
            WriteData(0x28);

            WriteCommand(0xC7);    //VCM control2 
            WriteData(0x86);   //--

            WriteCommand(0x36);    // Memory Access Control 
            WriteData(0x48);

            WriteCommand(0x3A);
            WriteData(0x55);

            WriteCommand(0xB1);
            WriteData(0x00);
            WriteData(0x18);

            WriteCommand(0xB6);    // Display Function Control 
            WriteData(0x08);
            WriteData(0x82);
            WriteData(0x27);
            /* 
                WriteCommand(0xF2);    // 3Gamma Function Disable 
                WriteData(0x00); 
 
                WriteCommand(0x26);    //Gamma curve selected 
                WriteData(0x01); 

                WriteCommand(0xE0);    //Set Gamma 
                WriteData(0x0F); 
                WriteData(0x31); 
                WriteData(0x2B); 
                WriteData(0x0C); 
                WriteData(0x0E); 
                WriteData(0x08); 
                WriteData(0x4E); 
                WriteData(0xF1); 
                WriteData(0x37); 
                WriteData(0x07); 
                WriteData(0x10); 
                WriteData(0x03); 
                WriteData(0x0E); 
                WriteData(0x09); 
                WriteData(0x00); 

                WriteCommand(0XE1);    //Set Gamma 
                WriteData(0x00); 
                WriteData(0x0E); 
                WriteData(0x14); 
                WriteData(0x03); 
                WriteData(0x11); 
                WriteData(0x07); 
                WriteData(0x31); 
                WriteData(0xC1); 
                WriteData(0x48); 
                WriteData(0x08); 
                WriteData(0x0F); 
                WriteData(0x0C); 
                WriteData(0x31); 
                WriteData(0x36); 
                WriteData(0x0F); 
            */
            WriteCommand(0x11);    //Exit Sleep 
            Thread.Sleep(120);

            WriteCommand(0x29);    //Display on 
            WriteCommand(0x2c);
            #endregion

            return;

            #region N18
            WriteCommand(0x11); //Sleep exit 
            Thread.Sleep(120);

            //ST7735R Frame Rates
            WriteCommand(0xB1);
            WriteData(0x01); WriteData(0x2C); WriteData(0x2D);
            WriteCommand(0xB2);
            WriteData(0x01); WriteData(0x2C); WriteData(0x2D);
            WriteCommand(0xB3);
            WriteData(0x01); WriteData(0x2C); WriteData(0x2D);
            WriteData(0x01); WriteData(0x2C); WriteData(0x2D);

            WriteCommand(0xB4); //Column inversion 
            WriteData(0x07);

            //ST7735R Power Sequence
            WriteCommand(0xC0);
            WriteData(0xA2); WriteData(0x02); WriteData(0x84);
            WriteCommand(0xC1); WriteData(0xC5);
            WriteCommand(0xC2);
            WriteData(0x0A); WriteData(0x00);
            WriteCommand(0xC3);
            WriteData(0x8A); WriteData(0x2A);
            WriteCommand(0xC4);
            WriteData(0x8A); WriteData(0xEE);

            WriteCommand(0xC5); //VCOM 
            WriteData(0x0E);

            WriteCommand(0x36); //MX, MY, RGB mode 
            WriteData(0xC8);

            //ST7735R Gamma Sequence
            WriteCommand(0xe0);
            WriteData(0x0f); WriteData(0x1a);
            WriteData(0x0f); WriteData(0x18);
            WriteData(0x2f); WriteData(0x28);
            WriteData(0x20); WriteData(0x22);
            WriteData(0x1f); WriteData(0x1b);
            WriteData(0x23); WriteData(0x37); WriteData(0x00);

            WriteData(0x07);
            WriteData(0x02); WriteData(0x10);
            WriteCommand(0xe1);
            WriteData(0x0f); WriteData(0x1b);
            WriteData(0x0f); WriteData(0x17);
            WriteData(0x33); WriteData(0x2c);
            WriteData(0x29); WriteData(0x2e);
            WriteData(0x30); WriteData(0x30);
            WriteData(0x39); WriteData(0x3f);
            WriteData(0x00); WriteData(0x07);
            WriteData(0x03); WriteData(0x10);

            WriteCommand(0x2a);
            WriteData(0x00); WriteData(0x00);
            WriteData(0x00); WriteData(0x7f);
            WriteCommand(0x2b);
            WriteData(0x00); WriteData(0x00);
            WriteData(0x00); WriteData(0x9f);

            WriteCommand(0xF0); //Enable test command  
            WriteData(0x01);
            WriteCommand(0xF6); //Disable ram power save mode 
            WriteData(0x00);

            WriteCommand(0x3A); //65k mode 
            WriteData(0x05);

            WriteCommand(0x29);//Display on
            #endregion
        }

        private void SetClippingArea(uint x, uint y, uint w, uint h)
        {
            shortArray[0] = (ushort)x;
            shortArray[1] = (ushort)(x + w);
            WriteCommand(0x2A);
            WriteData(shortArray);

            shortArray[0] = (ushort)y;
            shortArray[1] = (ushort)(y + h);
            WriteCommand(0x2B);
            WriteData(shortArray);
        }

        private void WriteCommand(byte command)
        {
            pinDc.Write(false);
            spi.Write(new byte[1] { command });
        }

        private void WriteData(byte data)
        {
            WriteData(new byte[1] { data });
        }
        private void WriteData(byte[] data)
        {
            pinDc.Write(true);
            spi.Write(data);
        }
        private void WriteData(ushort[] data)
        {
            pinDc.Write(true);
            spi.Write(data);
        }

        private void BitmapConverter(byte[] bitmapBytes, byte[] pixelBytes, GT.Mainboard.BPP bpp)
        {
            if (bpp != GT.Mainboard.BPP.BPP16_BGR_BE)
                throw new ArgumentOutOfRangeException("bpp", "Only BPP16_BGR_LE supported");

            Util.BitmapConvertBPP(bitmapBytes, pixelBytes, Util.BPP_Type.BPP16_BGR_BE);
        }
        #endregion
    }
}
