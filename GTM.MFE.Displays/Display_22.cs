using Gadgeteer;
using Gadgeteer.Modules;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using GT = Gadgeteer;
using GTI = Gadgeteer.Interfaces;

namespace GTM.MFE.Displays
{
    /// <summary>
    /// A Display 2.2" module for Microsoft .NET Gadgeteer
    /// </summary>
    public class Display_22 : Module.DisplayModule // ILI9341 LCD controller
    {
        private GTI.SPI spi;
        private GTI.SPI.Configuration spiConfig;
        private SPI.Configuration netMFSpiConfig;
        private GT.Socket socket;
        private GTI.DigitalOutput resetPin;
        private GTI.DigitalOutput backlightPin;
        private GTI.DigitalOutput rs; // TFT D/C

        private byte[] byteArray;
        private ushort[] shortArray;

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

        /// <summary>Constructor</summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public Display_22(int socketNumber)
            : base(WPFRenderOptions.Intercept)
        {
			byteArray = new byte[1];
			shortArray = new ushort[2];

            socket = Socket.GetSocket(socketNumber, true, this, null);
            socket.EnsureTypeIsSupported('S', this);

			resetPin = new GTI.DigitalOutput(socket, Socket.Pin.Three, false, this);
			backlightPin = new GTI.DigitalOutput(socket, Socket.Pin.Four, false, this);
			rs = new GTI.DigitalOutput(socket, Socket.Pin.Five, false, this);

			spiConfig = new GTI.SPI.Configuration(false, 0, 0, false, true, 12000);
			netMFSpiConfig = new SPI.Configuration(socket.CpuPins[6], spiConfig.ChipSelectActiveState, spiConfig.ChipSelectSetupTime, spiConfig.ChipSelectHoldTime, spiConfig.ClockIdleState, spiConfig.ClockEdge, spiConfig.ClockRateKHz, socket.SPIModule);
			spi = new GTI.SPI(socket, spiConfig, GTI.SPI.Sharing.Shared, socket, Socket.Pin.Six, this);

			Reset();
			ConfigureDisplay();
			Clear();
			SetBacklight(true);
		}

        /// <summary>
        /// Enables or disables the display backlight.
        /// </summary>
        /// <param name="state">The state to set the backlight to.</param>
        public void SetBacklight(bool state)
        {
            backlightPin.Write(state);
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void Clear()
        {
            uint w = Width / 2;//64
            uint h = Height / 2;//80

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
            byte[] vram = new byte[bmp.Width * bmp.Height * 2];
            Module.Mainboard.NativeBitmapConverter(bmp.GetBitmap(), vram, Mainboard.BPP.BPP16_BGR_BE);
            DrawRaw(vram, (uint)bmp.Width, (uint)bmp.Height, x, y);
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
                    rs.Write(true);
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
            resetPin.Write(false);
            Thread.Sleep(150);
            resetPin.Write(true);
        }

        private void ConfigureDisplay()
        {
            Mainboard.LCDConfiguration lcdConfig = new Mainboard.LCDConfiguration();
            lcdConfig.LCDControllerEnabled = false;
            lcdConfig.Width = Width;
            lcdConfig.Height = Height;

            DisplayModule.SetLCDConfig(lcdConfig);

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
            byteArray[0] = command;

            rs.Write(false);
            spi.Write(byteArray);
        }

        private void WriteData(byte data)
        {
            byteArray[0] = data;
            WriteData(byteArray);
        }
        private void WriteData(byte[] data)
        {
            rs.Write(true);
            spi.Write(data);
        }
        private void WriteData(ushort[] data)
        {
            rs.Write(true);
            spi.Write(data);
        }
    }
}
