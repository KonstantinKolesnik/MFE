
namespace GTM.MFE.Displays
{
    public enum ModelType
    {
        ITDB32			= 0,	// HX8347-A (16bit)
        ITDB32WC		= 1,	// ILI9327  (16bit)
        TFT01_32W		= 1,	// ILI9327	(16bit)
        ITDB32S			= 2,	// SSD1289  (16bit)
        TFT01_32		= 2,	// SSD1289  (16bit)
        CTE32			= 2,	// SSD1289  (16bit)
        GEEE32			= 2,	// SSD1289  (16bit)
        ITDB24			= 3,	// ILI9325C (8bit)
        ITDB24D			= 4,	// ILI9325D (8bit)
        ITDB24DWOT		= 4,	// ILI9325D (8bit)
        ITDB28			= 4,	// ILI9325D (8bit)
        TFT01_24_8		= 4,	// ILI9325D (8bit)
        TFT01_24_16		= 5,	// ILI9325D (16bit)
        ITDB22			= 6,	// HX8340-B (8bit)
        GEEE22			= 6,	// HX8340-B (8bit)
        ITDB22SP		= 7,	// HX8340-B (Serial)
        ITDB32WD		= 8,	// HX8352-A (16bit)
        TFT01_32WD		= 8,	// HX8352-A	(16bit)
        CTE32W			= 8,	// HX8352-A	(16bit)
        ITDB18SP		= 9,	// ST7735   (Serial)
        LPH9135			= 10,	// PCF8833	(Serial)
        ITDB25H			= 11,	// S1D19122	(16bit)
        ITDB43			= 12,	// SSD1963	(16bit) 480x272
        TFT01_43		= 12,	// SSD1963	(16bit) 480x272
        ITDB50			= 13,	// SSD1963	(16bit) 800x480
        TFT01_50		= 13,	// SSD1963	(16bit) 800x480
        CTE50			= 13,	// SSD1963	(16bit) 800x480
        ITDB24E_8		= 14,	// S6D1121	(8bit)
        TFT01_24R2		= 14,	// S6D1121	(8bit)
        ITDB24E_16		= 15,	// S6D1121	(16bit)
        INFINIT32		= 16,	// SSD1289	(Latched 16bit) -- Legacy, will be removed later
        ELEE32_REVA		= 16,	// SSD1289	(Latched 16bit)
        GEEE24			= 17,	// ILI9320	(8bit)
        GEEE28			= 18,	// ILI9320	(16bit)
        ELEE32_REVB		= 19,	// SSD1289	(8bit)
        TFT01_70		= 20,	// SSD1963	(16bit) 800x480 Alternative Init
        CTE70			= 20,	// SSD1963	(16bit) 800x480 Alternative Init
        CTE32HR			= 21,	// ILI9481	(16bit)
        CTE28			= 22,	// ILI9325D (16bit) Alternative Init
        TFT01_28		= 22,	// ILI9325D (16bit) Alternative Init
        CTE22			= 23,	// S6D0164	(8bit)
        TFT01_22		= 23,	// S6D0164	(8bit)
        TFT01_18SP		= 24,	// ST7735S  (Serial)
        TFT01_22SP		= 25,	// ILI9341	(Serial 5Pin)
        MI0283QT9		= 26,   // ILI9341	(Serial 4Pin)
        CTE35IPS		= 27,	// R61581	(16bit)
        CTE40           = 28	// ILI9486	(16bit)
    }
}
