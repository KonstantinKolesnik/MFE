using System;

namespace Gadgeteer.Modules.LoveElectronics
{
    /// <summary>
    /// A pH and Temperature module for Microsoft .NET Gadgeteer
    /// </summary>
    public class PHTemp : Module
    {
        #region Constants
        private const double voltageReferencePh = 1.024;
        private const double voltageReferenceTemp = 2.048;
        private const float rBridge = 1000; //"01B", 1kOm
        private const MCP342xChannel channelPH = MCP342xChannel.Channel1;
        private const MCP342xChannel channelTemp = MCP342xChannel.Channel2;
        private const string connectionError = "Device has not been connected. Call 'Connect()' first or check connection and address jumper settings.";
        #endregion

        #region Fiels
        private uint socketNumber;
        private MCP342 adc;
        private bool isConnected;
        private PHTemp.PHTempSettings settings;
        #endregion

        #region Properties
        /// <summary>
        /// The offset in milliVolts to apply to the reading from the pH probe for correct calibration.
        /// </summary>
        public double phOffset
        {
            get { return settings.pHOffset; }
            set { settings.pHOffset = value; }
        }

        /// <summary>
        /// The amount of milliVolts per pH level.
        /// </summary>
        public double phSlope
        {
            get { return settings.pHSlope; }
            set { settings.pHSlope = value; }
        }

        /// <summary>
        /// The array of data points to determine the RTD temperature.
        /// </summary>
        public PHTemp.RTDDataPoint[] TemperatureCurve
        {
            get { return settings.TemperatureCurve; }
            set { settings.TemperatureCurve = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create an instance of a pH and Temperature Module.
        /// </summary>
        /// <param name="socketNumber"></param>
        public PHTemp(int socketNumber)
        {
            this.socketNumber = (uint)socketNumber;
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);
            socket.EnsureTypeIsSupported('I', this);

            settings = new PHTemp.PHTempSettings()
            {
                pHOffset = 0,
                pHSlope = 54.2,
                TemperatureCurve = GetExampleTemperatureCurveDataPoints(PHTemp.RTDType.Pt1000)
            };
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Connect to the device to set its initial settings and ensure correct address settings.
        /// Address lines are defaulted to floating.
        /// </summary>
        public bool Connect()
        {
            return Connect(MCP342xJumperState.Floating, MCP342xJumperState.Floating);
        }
        /// <summary>
        /// Connect to the device to set its initial settings and ensure correct address settings.
        /// </summary>
        /// <param name="j1">The state of J1 on the module.</param>
        /// <param name="j2">The state of J2 on the module.</param>
        /// <returns></returns>
        public bool Connect(MCP342xJumperState j1, MCP342xJumperState j2)
        {
            adc = new MCP342(j1, j2);
            if (adc.ReadConfiguration() == 0)
                return false;

            //adc.Channel = MCP342xChannel.Channel1;
            //adc.Gain = MCP342xGain.x1;
            //adc.Resolution = MCP324xResolution.SixteenBits;
            //adc.ConversionMode = MCP342xConversionMode.Continuous;

            isConnected = true;

            return true;
        }

        /// <summary>
        /// Retrives the built in temperature curves.
        /// If you do not have a type provided in the enum you will need to create your own datapoint from the sensor datasheet.
        /// </summary>
        /// <param name="curve">The type of sensor you have.</param>
        /// <returns></returns>
        public PHTemp.RTDDataPoint[] GetExampleTemperatureCurveDataPoints(PHTemp.RTDType curve)
        {
            PHTemp.RTDDataPoint[] dataPoints;
            if (curve == PHTemp.RTDType.Pt1000)
            {
                dataPoints = new PHTemp.RTDDataPoint[35];
                dataPoints[0].Temperature = -50;
                dataPoints[0].Resistance = 803.1;
                dataPoints[1].Temperature = -45;
                dataPoints[1].Resistance = 822.9;
                dataPoints[2].Temperature = -40;
                dataPoints[2].Resistance = 842.7;
                dataPoints[3].Temperature = -35;
                dataPoints[3].Resistance = 862.5;
                dataPoints[4].Temperature = -30;
                dataPoints[4].Resistance = 882.2;
                dataPoints[5].Temperature = -25;
                dataPoints[5].Resistance = 901.9;
                dataPoints[6].Temperature = -20;
                dataPoints[6].Resistance = 921.6;
                dataPoints[7].Temperature = -15;
                dataPoints[7].Resistance = 941.2;
                dataPoints[8].Temperature = -10;
                dataPoints[8].Resistance = 960.9;
                dataPoints[9].Temperature = -5;
                dataPoints[9].Resistance = 980.4;
                dataPoints[10].Temperature = 0;
                dataPoints[10].Resistance = 1000;
                dataPoints[11].Temperature = 5;
                dataPoints[11].Resistance = 1019.5;
                dataPoints[12].Temperature = 10;
                dataPoints[12].Resistance = 1039;
                dataPoints[13].Temperature = 15;
                dataPoints[13].Resistance = 1058.5;
                dataPoints[14].Temperature = 20;
                dataPoints[14].Resistance = 1077.9;
                dataPoints[15].Temperature = 25;
                dataPoints[15].Resistance = 1097.3;
                dataPoints[16].Temperature = 30;
                dataPoints[16].Resistance = 1116.7;
                dataPoints[17].Temperature = 35;
                dataPoints[17].Resistance = 1136.1;
                dataPoints[18].Temperature = 40;
                dataPoints[18].Resistance = 1155.4;
                dataPoints[19].Temperature = 45;
                dataPoints[19].Resistance = 1174.7;
                dataPoints[20].Temperature = 50;
                dataPoints[20].Resistance = 1194;
                dataPoints[21].Temperature = 55;
                dataPoints[21].Resistance = 1213.2;
                dataPoints[22].Temperature = 60;
                dataPoints[22].Resistance = 1232.4;
                dataPoints[23].Temperature = 65;
                dataPoints[23].Resistance = 1251.6;
                dataPoints[24].Temperature = 70;
                dataPoints[24].Resistance = 1270.7;
                dataPoints[25].Temperature = 75;
                dataPoints[25].Resistance = 1289.8;
                dataPoints[26].Temperature = 80;
                dataPoints[26].Resistance = 1308.9;
                dataPoints[27].Temperature = 85;
                dataPoints[27].Resistance = 1328;
                dataPoints[28].Temperature = 90;
                dataPoints[28].Resistance = 1347;
                dataPoints[29].Temperature = 95;
                dataPoints[29].Resistance = 1366;
                dataPoints[30].Temperature = 100;
                dataPoints[30].Resistance = 1385;
                dataPoints[31].Temperature = 105;
                dataPoints[31].Resistance = 1403.9;
                dataPoints[32].Temperature = 110;
                dataPoints[32].Resistance = 1422.9;
                dataPoints[33].Temperature = 150;
                dataPoints[33].Resistance = 1573.1;
                dataPoints[34].Temperature = 200;
                dataPoints[34].Resistance = 1758.4;
            }
            else if (curve == PHTemp.RTDType.Pt100)
            {
                dataPoints = new PHTemp.RTDDataPoint[35];
                dataPoints[0].Temperature = -50;
                dataPoints[0].Resistance = 80.31;
                dataPoints[1].Temperature = -45;
                dataPoints[1].Resistance = 82.29;
                dataPoints[2].Temperature = -40;
                dataPoints[2].Resistance = 84.27;
                dataPoints[3].Temperature = -35;
                dataPoints[3].Resistance = 86.25;
                dataPoints[4].Temperature = -30;
                dataPoints[4].Resistance = 88.22;
                dataPoints[5].Temperature = -25;
                dataPoints[5].Resistance = 90.19;
                dataPoints[6].Temperature = -20;
                dataPoints[6].Resistance = 92.16;
                dataPoints[7].Temperature = -15;
                dataPoints[7].Resistance = 94.12;
                dataPoints[8].Temperature = -10;
                dataPoints[8].Resistance = 96.09;
                dataPoints[9].Temperature = -5;
                dataPoints[9].Resistance = 98.04;
                dataPoints[10].Temperature = 0;
                dataPoints[10].Resistance = 100;
                dataPoints[11].Temperature = 5;
                dataPoints[11].Resistance = 101.95;
                dataPoints[12].Temperature = 10;
                dataPoints[12].Resistance = 103.9;
                dataPoints[13].Temperature = 15;
                dataPoints[13].Resistance = 105.85;
                dataPoints[14].Temperature = 20;
                dataPoints[14].Resistance = 107.79;
                dataPoints[15].Temperature = 25;
                dataPoints[15].Resistance = 109.73;
                dataPoints[16].Temperature = 30;
                dataPoints[16].Resistance = 111.67;
                dataPoints[17].Temperature = 35;
                dataPoints[17].Resistance = 113.61;
                dataPoints[18].Temperature = 40;
                dataPoints[18].Resistance = 115.54;
                dataPoints[19].Temperature = 45;
                dataPoints[19].Resistance = 117.47;
                dataPoints[20].Temperature = 50;
                dataPoints[20].Resistance = 119.4;
                dataPoints[21].Temperature = 55;
                dataPoints[21].Resistance = 121.32;
                dataPoints[22].Temperature = 60;
                dataPoints[22].Resistance = 123.24;
                dataPoints[23].Temperature = 65;
                dataPoints[23].Resistance = 125.16;
                dataPoints[24].Temperature = 70;
                dataPoints[24].Resistance = 127.07;
                dataPoints[25].Temperature = 75;
                dataPoints[25].Resistance = 128.98;
                dataPoints[26].Temperature = 80;
                dataPoints[26].Resistance = 130.89;
                dataPoints[27].Temperature = 85;
                dataPoints[27].Resistance = 132.8;
                dataPoints[28].Temperature = 90;
                dataPoints[28].Resistance = 134.7;
                dataPoints[29].Temperature = 95;
                dataPoints[29].Resistance = 136.6;
                dataPoints[30].Temperature = 100;
                dataPoints[30].Resistance = 138.5;
                dataPoints[31].Temperature = 105;
                dataPoints[31].Resistance = 140.39;
                dataPoints[32].Temperature = 110;
                dataPoints[32].Resistance = 142.29;
                dataPoints[33].Temperature = 150;
                dataPoints[33].Resistance = 157.31;
                dataPoints[34].Temperature = 200;
                dataPoints[34].Resistance = 175.84;
            }
            else if (curve == PHTemp.RTDType.PTC_Type201)
            {
                dataPoints = new PHTemp.RTDDataPoint[30];
                dataPoints[0].Temperature = -50;
                dataPoints[0].Resistance = 1032;
                dataPoints[1].Temperature = -45;
                dataPoints[1].Resistance = 1084;
                dataPoints[2].Temperature = -40;
                dataPoints[2].Resistance = 1135;
                dataPoints[3].Temperature = -35;
                dataPoints[3].Resistance = 1191;
                dataPoints[4].Temperature = -30;
                dataPoints[4].Resistance = 1246;
                dataPoints[5].Temperature = -25;
                dataPoints[5].Resistance = 1306;
                dataPoints[6].Temperature = -20;
                dataPoints[6].Resistance = 1366;
                dataPoints[7].Temperature = -15;
                dataPoints[7].Resistance = 1430;
                dataPoints[8].Temperature = -10;
                dataPoints[8].Resistance = 1493;
                dataPoints[9].Temperature = -5;
                dataPoints[9].Resistance = 1561;
                dataPoints[10].Temperature = 0;
                dataPoints[10].Resistance = 1628;
                dataPoints[11].Temperature = 5;
                dataPoints[11].Resistance = 1700;
                dataPoints[12].Temperature = 10;
                dataPoints[12].Resistance = 1771;
                dataPoints[13].Temperature = 15;
                dataPoints[13].Resistance = 1847;
                dataPoints[14].Temperature = 20;
                dataPoints[14].Resistance = 1922;
                dataPoints[15].Temperature = 25;
                dataPoints[15].Resistance = 2000;
                dataPoints[16].Temperature = 30;
                dataPoints[16].Resistance = 2080;
                dataPoints[17].Temperature = 35;
                dataPoints[17].Resistance = 2162;
                dataPoints[18].Temperature = 40;
                dataPoints[18].Resistance = 2244;
                dataPoints[19].Temperature = 45;
                dataPoints[19].Resistance = 2330;
                dataPoints[20].Temperature = 50;
                dataPoints[20].Resistance = 2415;
                dataPoints[21].Temperature = 55;
                dataPoints[21].Resistance = 2505;
                dataPoints[22].Temperature = 60;
                dataPoints[22].Resistance = 2595;
                dataPoints[23].Temperature = 65;
                dataPoints[23].Resistance = 2689;
                dataPoints[24].Temperature = 70;
                dataPoints[24].Resistance = 2782;
                dataPoints[25].Temperature = 75;
                dataPoints[25].Resistance = 2880;
                dataPoints[26].Temperature = 80;
                dataPoints[26].Resistance = 2977;
                dataPoints[27].Temperature = 85;
                dataPoints[27].Resistance = 3079;
                dataPoints[28].Temperature = 90;
                dataPoints[28].Resistance = 3180;
                dataPoints[29].Temperature = 95;
                dataPoints[29].Resistance = 3285;
                dataPoints[30].Temperature = 100;
                dataPoints[30].Resistance = 3390;
            }
            else if (curve == PHTemp.RTDType.NTC_Type101)
            {
                dataPoints = new PHTemp.RTDDataPoint[10];
                dataPoints[0].Temperature = -5;
                dataPoints[0].Resistance = 31389;
                dataPoints[1].Temperature = 0;
                dataPoints[1].Resistance = 23868;
                dataPoints[2].Temperature = 5;
                dataPoints[2].Resistance = 18299;
                dataPoints[3].Temperature = 10;
                dataPoints[3].Resistance = 14130;
                dataPoints[4].Temperature = 15;
                dataPoints[4].Resistance = 10998;
                dataPoints[5].Temperature = 20;
                dataPoints[5].Resistance = 8618;
                dataPoints[6].Temperature = 25;
                dataPoints[6].Resistance = 6800;
                dataPoints[7].Temperature = 30;
                dataPoints[7].Resistance = 5401;
                dataPoints[8].Temperature = 35;
                dataPoints[8].Resistance = 4317;
                dataPoints[9].Temperature = 40;
                dataPoints[9].Resistance = 3471;
            }
            else if (curve == PHTemp.RTDType.NTC_Type101)
            {
                dataPoints = new PHTemp.RTDDataPoint[10];
                dataPoints[0].Temperature = -5;
                dataPoints[0].Resistance = 31389;
                dataPoints[1].Temperature = 0;
                dataPoints[1].Resistance = 23868;
                dataPoints[2].Temperature = 5;
                dataPoints[2].Resistance = 18299;
                dataPoints[3].Temperature = 10;
                dataPoints[3].Resistance = 14130;
                dataPoints[4].Temperature = 15;
                dataPoints[4].Resistance = 10998;
                dataPoints[5].Temperature = 20;
                dataPoints[5].Resistance = 8618;
                dataPoints[6].Temperature = 25;
                dataPoints[6].Resistance = 6800;
                dataPoints[7].Temperature = 30;
                dataPoints[7].Resistance = 5401;
                dataPoints[8].Temperature = 35;
                dataPoints[8].Resistance = 4317;
                dataPoints[9].Temperature = 40;
                dataPoints[9].Resistance = 3471;
            }
            else if (curve == PHTemp.RTDType.NTC_Type102)
            {
                dataPoints = new PHTemp.RTDDataPoint[8];
                dataPoints[0].Temperature = -25;
                dataPoints[0].Resistance = 26083;
                dataPoints[1].Temperature = 20;
                dataPoints[1].Resistance = 19414;
                dataPoints[2].Temperature = 15;
                dataPoints[2].Resistance = 14596;
                dataPoints[3].Temperature = 10;
                dataPoints[3].Resistance = 11066;
                dataPoints[4].Temperature = 5;
                dataPoints[4].Resistance = 8466;
                dataPoints[5].Temperature = 0;
                dataPoints[5].Resistance = 6536;
                dataPoints[6].Temperature = 5;
                dataPoints[6].Resistance = 5078;
                dataPoints[7].Temperature = 10;
                dataPoints[7].Resistance = 3986;
            }
            else if (curve == PHTemp.RTDType.NTC_Type103)
            {
                dataPoints = new PHTemp.RTDDataPoint[7];
                dataPoints[0].Temperature = -40;
                dataPoints[0].Resistance = 50475;
                dataPoints[1].Temperature = -35;
                dataPoints[1].Resistance = 36405;
                dataPoints[2].Temperature = -30;
                dataPoints[2].Resistance = 26550;
                dataPoints[3].Temperature = -25;
                dataPoints[3].Resistance = 19560;
                dataPoints[4].Temperature = -20;
                dataPoints[4].Resistance = 14560;
                dataPoints[5].Temperature = -15;
                dataPoints[5].Resistance = 10943;
                dataPoints[6].Temperature = -10;
                dataPoints[6].Resistance = 8299;
            }
            else if (curve != PHTemp.RTDType.NTC_Type104)
            {
                if (curve != PHTemp.RTDType.NTC_Type105)
                {
                    throw new Exception("Curve type not recognised");
                }
                dataPoints = new PHTemp.RTDDataPoint[9];
                dataPoints[0].Temperature = 55;
                dataPoints[0].Resistance = 27475;
                dataPoints[1].Temperature = 60;
                dataPoints[1].Resistance = 22590;
                dataPoints[2].Temperature = 65;
                dataPoints[2].Resistance = 18668;
                dataPoints[3].Temperature = 70;
                dataPoints[3].Resistance = 15052;
                dataPoints[4].Temperature = 75;
                dataPoints[4].Resistance = 12932;
                dataPoints[5].Temperature = 80;
                dataPoints[5].Resistance = 10837;
                dataPoints[6].Temperature = 85;
                dataPoints[6].Resistance = 9121;
                dataPoints[7].Temperature = 90;
                dataPoints[7].Resistance = 7708;
                dataPoints[8].Temperature = 95;
                dataPoints[8].Resistance = 6539;
            }
            else
            {
                dataPoints = new PHTemp.RTDDataPoint[8];
                dataPoints[0].Temperature = 25;
                dataPoints[0].Resistance = 15000;
                dataPoints[1].Temperature = 30;
                dataPoints[1].Resistance = 11933;
                dataPoints[2].Temperature = 35;
                dataPoints[2].Resistance = 9522;
                dataPoints[3].Temperature = 40;
                dataPoints[3].Resistance = 7657;
                dataPoints[4].Temperature = 45;
                dataPoints[4].Resistance = 6194;
                dataPoints[5].Temperature = 50;
                dataPoints[5].Resistance = 5039;
                dataPoints[6].Temperature = 55;
                dataPoints[6].Resistance = 4299;
                dataPoints[7].Temperature = 60;
                dataPoints[7].Resistance = 3756;
            }
            return dataPoints;
        }

        /// <summary>
        /// Returns the raw value of the RTD returned by the ADC in milliVolts;
        /// </summary>
        /// <returns></returns>
        public double ReadRawTemperature()
        {
            if (!isConnected)
                throw new Exception(connectionError);

            adc.Channel = channelTemp;
            return adc.ReadVolts();
        }
        /// <summary>
        /// Read the temperature from the RTD sensor.
        /// </summary>
        /// <returns>The temperature read by the RTD in degrees Celcius.</returns>
        public double ReadTemperature()
        {
            double volts = ReadRawTemperature() / 1000;
            double r1 = rBridge;
            double r2 = rBridge;
            double r3 = rBridge;
            double t1 = (r1 + r2) * (volts / voltageReferenceTemp);
            double rx = (r1 * r3 + r3 * t1) / (r2 - t1);

            //double k = 2.048 / volts;
            //double rx = (r3 * (r1 + r2) + k * r2) / (k * r1 - (r1 + r2));

            if (rx <= settings.TemperatureCurve[0].Resistance)
                throw new Exception(string.Concat("The resistance (", rx, " Ohms) was out of range of the supplied temperature curve."));

            var tempCurve = settings.TemperatureCurve;
            double deltaTemp = 0;
            double temp = (double)tempCurve[0].Temperature;

            int i = 1;
            while (i < tempCurve.Length)
            {
                deltaTemp = (double)(tempCurve[i].Temperature - tempCurve[i - 1].Temperature);
                if (rx >= tempCurve[i].Resistance)
                {
                    temp += deltaTemp;
                    i++;
                }
                else
                    return temp + (rx - tempCurve[i - 1].Resistance) * deltaTemp / (tempCurve[i].Resistance - tempCurve[i - 1].Resistance);
            }

            return temp;
        }

        /// <summary>
        /// Returns the raw value of the ph Probe returned by the ADC in milliVolts.
        /// </summary>
        /// <returns></returns>
        public double ReadRawPH()
        {
            if (!isConnected)
                throw new Exception(connectionError);

            adc.Channel = channelPH;
            return adc.ReadVolts();
        }
        /// <summary>
        /// Reads the pH from the pH probe with no temperature compensation.
        /// </summary>
        /// <returns></returns>
        public double ReadPH()
        {
            double value = ReadRawPH();
            value = value - voltageReferencePh + phOffset;
            return 7 - value / phSlope;
        }
        /// <summary>
        /// Reads the pH from the pH probe with temperature compensation.
        /// </summary>
        /// <returns></returns>
        public double ReadTempCompensatedPH()
        {
            double temperatureSlopeModification = 0.1984 * ReadTemperature();

            double value = ReadRawPH();
            value = value - voltageReferencePh + phOffset;
            return 7 - value / (phSlope + temperatureSlopeModification);
        }

        /// <summary>
        /// Set the resolution of the analog to digital converter.
        /// Larger resolutions increase the conversion time.
        /// </summary>
        /// <param name="resolution"></param>
        public void SetResolution(MCP324xResolution resolution)
        {
            if (!isConnected)
                throw new Exception(connectionError);

            adc.Resolution = resolution;
        }
        #endregion

        /// <summary>
        /// Data points of the RTD sensor curve to calculate the temperature.
        /// </summary>
        [Serializable]
        public struct RTDDataPoint
        {
            public int Temperature;
            public double Resistance;
        }

        public enum RTDType : byte
        {
            Pt1000,
            Pt100,
            PTC_Type201,
            NTC_Type101,
            NTC_Type102,
            NTC_Type103,
            NTC_Type104,
            NTC_Type105
        }

        private class PHTempSettings
        {
            public double pHOffset;
            public double pHSlope;
            public PHTemp.RTDDataPoint[] TemperatureCurve;
        }
    }





    ///// <summary>
    ///// A pH and Temperature module for Microsoft .NET Gadgeteer
    ///// </summary>
    //public class PHTemp : Module
    //{
    //    private const double m_VoltageReferencePh = 1.024;
    //    private const double m_VoltageReferenceTemp = 2.048;
    //    private const float rBridge = 1000; //"01B", 1kOm
    //    private const MCP342xChannel channelPH = MCP342xChannel.Channel1;
    //    private const MCP342xChannel channelTemp = MCP342xChannel.Channel2;
    //    private const string connectionError = "Device has not been connected. Call 'Connect()' first or check connection and address jumper settings.";

    //    private uint socketNumber;
    //    private MCP342X adc; // MCP342x device
    //    private int clockRate = 400; //kHz
    //    private bool isConnected;
    //    private PHTemp.PHTempSettings settings;

    //    #region Properties
    //    /// <summary>
    //    /// The offset in milliVolts to apply to the reading from the pH probe for correct calibration.
    //    /// </summary>
    //    public double phOffset
    //    {
    //        get { return settings.pHOffset; }
    //        set { settings.pHOffset = value; }
    //    }

    //    /// <summary>
    //    /// The amount of milliVolts per pH level.
    //    /// </summary>
    //    public double phSlope
    //    {
    //        get { return settings.pHSlope; }
    //        set { settings.pHSlope = value; }
    //    }

    //    /// <summary>
    //    /// The array of data points to determine the RTD temperature.
    //    /// </summary>
    //    public PHTemp.RTDDataPoint[] TemperatureCurve
    //    {
    //        get { return settings.TemperatureCurve; }
    //        set { settings.TemperatureCurve = value; }
    //    }
    //    #endregion

    //    #region Constructor
    //    /// <summary>
    //    /// Create an instance of a pH and Temperature Module.
    //    /// </summary>
    //    /// <param name="socketNumber"></param>
    //    public PHTemp(int socketNumber)
    //    {
    //        this.socketNumber = (uint)socketNumber;
    //        Socket socket = Socket.GetSocket(socketNumber, true, this, null);
    //        socket.EnsureTypeIsSupported('I', this);

    //        settings = new PHTemp.PHTempSettings()
    //        {
    //            pHOffset = 0,
    //            pHSlope = 54.2,
    //            TemperatureCurve = GetExampleTemperatureCurveDataPoints(PHTemp.RTDType.Pt1000)
    //        };
    //    }
    //    #endregion

    //    /// <summary>
    //    /// Connect to the device to set its initial settings and ensure correct address settings.
    //    /// Address lines are defaulted to floating.
    //    /// </summary>
    //    public bool Connect()
    //    {
    //        return Connect(PHTemp.JumperState.Floating, PHTemp.JumperState.Floating);
    //    }
    //    /// <summary>
    //    /// Connect to the device to set its initial settings and ensure correct address settings.
    //    /// </summary>
    //    /// <param name="j1">The state of J1 on the module.</param>
    //    /// <param name="j2">The state of J2 on the module.</param>
    //    /// <returns></returns>
    //    public bool Connect(PHTemp.JumperState j1, PHTemp.JumperState j2)
    //    {
    //        ushort userAddress = 0;

    //        if (j1 == PHTemp.JumperState.Floating && j2 == PHTemp.JumperState.Floating)
    //            userAddress = 0;
    //        else if (j1 == PHTemp.JumperState.Low && j2 == PHTemp.JumperState.Low)
    //            userAddress = 0;
    //        else if (j1 == PHTemp.JumperState.Low && j2 == PHTemp.JumperState.Floating)
    //            userAddress = 1;
    //        else if (j1 == PHTemp.JumperState.Low && j2 == PHTemp.JumperState.High)
    //            userAddress = 2;
    //        else if (j1 == PHTemp.JumperState.High && j2 == PHTemp.JumperState.Low)
    //            userAddress = 4;
    //        else if (j1 == PHTemp.JumperState.High && j2 == PHTemp.JumperState.Floating)
    //            userAddress = 5;
    //        else if (j1 == PHTemp.JumperState.High && j2 == PHTemp.JumperState.High)
    //            userAddress = 6;
    //        else if (j1 == PHTemp.JumperState.Floating && j2 == PHTemp.JumperState.Low)
    //            userAddress = 3;
    //        else if (j1 == PHTemp.JumperState.Floating && j2 == PHTemp.JumperState.High)
    //            userAddress = 7;

    //        ushort address = (ushort)(userAddress | 104); // 1101xxx, xxx = user address
    //        adc = new MCP342X(address, clockRate);
    //        if (adc.ReadConfiguration() == 0)
    //            return false;

    //        adc.Channel = MCP342xChannel.Channel1;
    //        adc.Gain = MCP342xGain.x1;
    //        adc.Resolution = MCP324xResolution.SixteenBits;
    //        adc.ConversionMode = MCP342xConversionMode.Continuous;
            
    //        isConnected = true;

    //        return true;
    //    }

    //    /// <summary>
    //    /// Retrives the built in temperature curves.
    //    /// If you do not have a type provided in the enum you will need to create your own datapoint from the sensor datasheet.
    //    /// </summary>
    //    /// <param name="curve">The type of sensor you have.</param>
    //    /// <returns></returns>
    //    public PHTemp.RTDDataPoint[] GetExampleTemperatureCurveDataPoints(PHTemp.RTDType curve)
    //    {
    //        PHTemp.RTDDataPoint[] dataPoints;
    //        if (curve == PHTemp.RTDType.Pt1000)
    //        {
    //            dataPoints = new PHTemp.RTDDataPoint[35];
    //            dataPoints[0].Temperature = -50;
    //            dataPoints[0].Resistance = 803.1;
    //            dataPoints[1].Temperature = -45;
    //            dataPoints[1].Resistance = 822.9;
    //            dataPoints[2].Temperature = -40;
    //            dataPoints[2].Resistance = 842.7;
    //            dataPoints[3].Temperature = -35;
    //            dataPoints[3].Resistance = 862.5;
    //            dataPoints[4].Temperature = -30;
    //            dataPoints[4].Resistance = 882.2;
    //            dataPoints[5].Temperature = -25;
    //            dataPoints[5].Resistance = 901.9;
    //            dataPoints[6].Temperature = -20;
    //            dataPoints[6].Resistance = 921.6;
    //            dataPoints[7].Temperature = -15;
    //            dataPoints[7].Resistance = 941.2;
    //            dataPoints[8].Temperature = -10;
    //            dataPoints[8].Resistance = 960.9;
    //            dataPoints[9].Temperature = -5;
    //            dataPoints[9].Resistance = 980.4;
    //            dataPoints[10].Temperature = 0;
    //            dataPoints[10].Resistance = 1000;
    //            dataPoints[11].Temperature = 5;
    //            dataPoints[11].Resistance = 1019.5;
    //            dataPoints[12].Temperature = 10;
    //            dataPoints[12].Resistance = 1039;
    //            dataPoints[13].Temperature = 15;
    //            dataPoints[13].Resistance = 1058.5;
    //            dataPoints[14].Temperature = 20;
    //            dataPoints[14].Resistance = 1077.9;
    //            dataPoints[15].Temperature = 25;
    //            dataPoints[15].Resistance = 1097.3;
    //            dataPoints[16].Temperature = 30;
    //            dataPoints[16].Resistance = 1116.7;
    //            dataPoints[17].Temperature = 35;
    //            dataPoints[17].Resistance = 1136.1;
    //            dataPoints[18].Temperature = 40;
    //            dataPoints[18].Resistance = 1155.4;
    //            dataPoints[19].Temperature = 45;
    //            dataPoints[19].Resistance = 1174.7;
    //            dataPoints[20].Temperature = 50;
    //            dataPoints[20].Resistance = 1194;
    //            dataPoints[21].Temperature = 55;
    //            dataPoints[21].Resistance = 1213.2;
    //            dataPoints[22].Temperature = 60;
    //            dataPoints[22].Resistance = 1232.4;
    //            dataPoints[23].Temperature = 65;
    //            dataPoints[23].Resistance = 1251.6;
    //            dataPoints[24].Temperature = 70;
    //            dataPoints[24].Resistance = 1270.7;
    //            dataPoints[25].Temperature = 75;
    //            dataPoints[25].Resistance = 1289.8;
    //            dataPoints[26].Temperature = 80;
    //            dataPoints[26].Resistance = 1308.9;
    //            dataPoints[27].Temperature = 85;
    //            dataPoints[27].Resistance = 1328;
    //            dataPoints[28].Temperature = 90;
    //            dataPoints[28].Resistance = 1347;
    //            dataPoints[29].Temperature = 95;
    //            dataPoints[29].Resistance = 1366;
    //            dataPoints[30].Temperature = 100;
    //            dataPoints[30].Resistance = 1385;
    //            dataPoints[31].Temperature = 105;
    //            dataPoints[31].Resistance = 1403.9;
    //            dataPoints[32].Temperature = 110;
    //            dataPoints[32].Resistance = 1422.9;
    //            dataPoints[33].Temperature = 150;
    //            dataPoints[33].Resistance = 1573.1;
    //            dataPoints[34].Temperature = 200;
    //            dataPoints[34].Resistance = 1758.4;
    //        }
    //        else if (curve == PHTemp.RTDType.Pt100)
    //        {
    //            dataPoints = new PHTemp.RTDDataPoint[35];
    //            dataPoints[0].Temperature = -50;
    //            dataPoints[0].Resistance = 80.31;
    //            dataPoints[1].Temperature = -45;
    //            dataPoints[1].Resistance = 82.29;
    //            dataPoints[2].Temperature = -40;
    //            dataPoints[2].Resistance = 84.27;
    //            dataPoints[3].Temperature = -35;
    //            dataPoints[3].Resistance = 86.25;
    //            dataPoints[4].Temperature = -30;
    //            dataPoints[4].Resistance = 88.22;
    //            dataPoints[5].Temperature = -25;
    //            dataPoints[5].Resistance = 90.19;
    //            dataPoints[6].Temperature = -20;
    //            dataPoints[6].Resistance = 92.16;
    //            dataPoints[7].Temperature = -15;
    //            dataPoints[7].Resistance = 94.12;
    //            dataPoints[8].Temperature = -10;
    //            dataPoints[8].Resistance = 96.09;
    //            dataPoints[9].Temperature = -5;
    //            dataPoints[9].Resistance = 98.04;
    //            dataPoints[10].Temperature = 0;
    //            dataPoints[10].Resistance = 100;
    //            dataPoints[11].Temperature = 5;
    //            dataPoints[11].Resistance = 101.95;
    //            dataPoints[12].Temperature = 10;
    //            dataPoints[12].Resistance = 103.9;
    //            dataPoints[13].Temperature = 15;
    //            dataPoints[13].Resistance = 105.85;
    //            dataPoints[14].Temperature = 20;
    //            dataPoints[14].Resistance = 107.79;
    //            dataPoints[15].Temperature = 25;
    //            dataPoints[15].Resistance = 109.73;
    //            dataPoints[16].Temperature = 30;
    //            dataPoints[16].Resistance = 111.67;
    //            dataPoints[17].Temperature = 35;
    //            dataPoints[17].Resistance = 113.61;
    //            dataPoints[18].Temperature = 40;
    //            dataPoints[18].Resistance = 115.54;
    //            dataPoints[19].Temperature = 45;
    //            dataPoints[19].Resistance = 117.47;
    //            dataPoints[20].Temperature = 50;
    //            dataPoints[20].Resistance = 119.4;
    //            dataPoints[21].Temperature = 55;
    //            dataPoints[21].Resistance = 121.32;
    //            dataPoints[22].Temperature = 60;
    //            dataPoints[22].Resistance = 123.24;
    //            dataPoints[23].Temperature = 65;
    //            dataPoints[23].Resistance = 125.16;
    //            dataPoints[24].Temperature = 70;
    //            dataPoints[24].Resistance = 127.07;
    //            dataPoints[25].Temperature = 75;
    //            dataPoints[25].Resistance = 128.98;
    //            dataPoints[26].Temperature = 80;
    //            dataPoints[26].Resistance = 130.89;
    //            dataPoints[27].Temperature = 85;
    //            dataPoints[27].Resistance = 132.8;
    //            dataPoints[28].Temperature = 90;
    //            dataPoints[28].Resistance = 134.7;
    //            dataPoints[29].Temperature = 95;
    //            dataPoints[29].Resistance = 136.6;
    //            dataPoints[30].Temperature = 100;
    //            dataPoints[30].Resistance = 138.5;
    //            dataPoints[31].Temperature = 105;
    //            dataPoints[31].Resistance = 140.39;
    //            dataPoints[32].Temperature = 110;
    //            dataPoints[32].Resistance = 142.29;
    //            dataPoints[33].Temperature = 150;
    //            dataPoints[33].Resistance = 157.31;
    //            dataPoints[34].Temperature = 200;
    //            dataPoints[34].Resistance = 175.84;
    //        }
    //        else if (curve == PHTemp.RTDType.PTC_Type201)
    //        {
    //            dataPoints = new PHTemp.RTDDataPoint[30];
    //            dataPoints[0].Temperature = -50;
    //            dataPoints[0].Resistance = 1032;
    //            dataPoints[1].Temperature = -45;
    //            dataPoints[1].Resistance = 1084;
    //            dataPoints[2].Temperature = -40;
    //            dataPoints[2].Resistance = 1135;
    //            dataPoints[3].Temperature = -35;
    //            dataPoints[3].Resistance = 1191;
    //            dataPoints[4].Temperature = -30;
    //            dataPoints[4].Resistance = 1246;
    //            dataPoints[5].Temperature = -25;
    //            dataPoints[5].Resistance = 1306;
    //            dataPoints[6].Temperature = -20;
    //            dataPoints[6].Resistance = 1366;
    //            dataPoints[7].Temperature = -15;
    //            dataPoints[7].Resistance = 1430;
    //            dataPoints[8].Temperature = -10;
    //            dataPoints[8].Resistance = 1493;
    //            dataPoints[9].Temperature = -5;
    //            dataPoints[9].Resistance = 1561;
    //            dataPoints[10].Temperature = 0;
    //            dataPoints[10].Resistance = 1628;
    //            dataPoints[11].Temperature = 5;
    //            dataPoints[11].Resistance = 1700;
    //            dataPoints[12].Temperature = 10;
    //            dataPoints[12].Resistance = 1771;
    //            dataPoints[13].Temperature = 15;
    //            dataPoints[13].Resistance = 1847;
    //            dataPoints[14].Temperature = 20;
    //            dataPoints[14].Resistance = 1922;
    //            dataPoints[15].Temperature = 25;
    //            dataPoints[15].Resistance = 2000;
    //            dataPoints[16].Temperature = 30;
    //            dataPoints[16].Resistance = 2080;
    //            dataPoints[17].Temperature = 35;
    //            dataPoints[17].Resistance = 2162;
    //            dataPoints[18].Temperature = 40;
    //            dataPoints[18].Resistance = 2244;
    //            dataPoints[19].Temperature = 45;
    //            dataPoints[19].Resistance = 2330;
    //            dataPoints[20].Temperature = 50;
    //            dataPoints[20].Resistance = 2415;
    //            dataPoints[21].Temperature = 55;
    //            dataPoints[21].Resistance = 2505;
    //            dataPoints[22].Temperature = 60;
    //            dataPoints[22].Resistance = 2595;
    //            dataPoints[23].Temperature = 65;
    //            dataPoints[23].Resistance = 2689;
    //            dataPoints[24].Temperature = 70;
    //            dataPoints[24].Resistance = 2782;
    //            dataPoints[25].Temperature = 75;
    //            dataPoints[25].Resistance = 2880;
    //            dataPoints[26].Temperature = 80;
    //            dataPoints[26].Resistance = 2977;
    //            dataPoints[27].Temperature = 85;
    //            dataPoints[27].Resistance = 3079;
    //            dataPoints[28].Temperature = 90;
    //            dataPoints[28].Resistance = 3180;
    //            dataPoints[29].Temperature = 95;
    //            dataPoints[29].Resistance = 3285;
    //            dataPoints[30].Temperature = 100;
    //            dataPoints[30].Resistance = 3390;
    //        }
    //        else if (curve == PHTemp.RTDType.NTC_Type101)
    //        {
    //            dataPoints = new PHTemp.RTDDataPoint[10];
    //            dataPoints[0].Temperature = -5;
    //            dataPoints[0].Resistance = 31389;
    //            dataPoints[1].Temperature = 0;
    //            dataPoints[1].Resistance = 23868;
    //            dataPoints[2].Temperature = 5;
    //            dataPoints[2].Resistance = 18299;
    //            dataPoints[3].Temperature = 10;
    //            dataPoints[3].Resistance = 14130;
    //            dataPoints[4].Temperature = 15;
    //            dataPoints[4].Resistance = 10998;
    //            dataPoints[5].Temperature = 20;
    //            dataPoints[5].Resistance = 8618;
    //            dataPoints[6].Temperature = 25;
    //            dataPoints[6].Resistance = 6800;
    //            dataPoints[7].Temperature = 30;
    //            dataPoints[7].Resistance = 5401;
    //            dataPoints[8].Temperature = 35;
    //            dataPoints[8].Resistance = 4317;
    //            dataPoints[9].Temperature = 40;
    //            dataPoints[9].Resistance = 3471;
    //        }
    //        else if (curve == PHTemp.RTDType.NTC_Type101)
    //        {
    //            dataPoints = new PHTemp.RTDDataPoint[10];
    //            dataPoints[0].Temperature = -5;
    //            dataPoints[0].Resistance = 31389;
    //            dataPoints[1].Temperature = 0;
    //            dataPoints[1].Resistance = 23868;
    //            dataPoints[2].Temperature = 5;
    //            dataPoints[2].Resistance = 18299;
    //            dataPoints[3].Temperature = 10;
    //            dataPoints[3].Resistance = 14130;
    //            dataPoints[4].Temperature = 15;
    //            dataPoints[4].Resistance = 10998;
    //            dataPoints[5].Temperature = 20;
    //            dataPoints[5].Resistance = 8618;
    //            dataPoints[6].Temperature = 25;
    //            dataPoints[6].Resistance = 6800;
    //            dataPoints[7].Temperature = 30;
    //            dataPoints[7].Resistance = 5401;
    //            dataPoints[8].Temperature = 35;
    //            dataPoints[8].Resistance = 4317;
    //            dataPoints[9].Temperature = 40;
    //            dataPoints[9].Resistance = 3471;
    //        }
    //        else if (curve == PHTemp.RTDType.NTC_Type102)
    //        {
    //            dataPoints = new PHTemp.RTDDataPoint[8];
    //            dataPoints[0].Temperature = -25;
    //            dataPoints[0].Resistance = 26083;
    //            dataPoints[1].Temperature = 20;
    //            dataPoints[1].Resistance = 19414;
    //            dataPoints[2].Temperature = 15;
    //            dataPoints[2].Resistance = 14596;
    //            dataPoints[3].Temperature = 10;
    //            dataPoints[3].Resistance = 11066;
    //            dataPoints[4].Temperature = 5;
    //            dataPoints[4].Resistance = 8466;
    //            dataPoints[5].Temperature = 0;
    //            dataPoints[5].Resistance = 6536;
    //            dataPoints[6].Temperature = 5;
    //            dataPoints[6].Resistance = 5078;
    //            dataPoints[7].Temperature = 10;
    //            dataPoints[7].Resistance = 3986;
    //        }
    //        else if (curve == PHTemp.RTDType.NTC_Type103)
    //        {
    //            dataPoints = new PHTemp.RTDDataPoint[7];
    //            dataPoints[0].Temperature = -40;
    //            dataPoints[0].Resistance = 50475;
    //            dataPoints[1].Temperature = -35;
    //            dataPoints[1].Resistance = 36405;
    //            dataPoints[2].Temperature = -30;
    //            dataPoints[2].Resistance = 26550;
    //            dataPoints[3].Temperature = -25;
    //            dataPoints[3].Resistance = 19560;
    //            dataPoints[4].Temperature = -20;
    //            dataPoints[4].Resistance = 14560;
    //            dataPoints[5].Temperature = -15;
    //            dataPoints[5].Resistance = 10943;
    //            dataPoints[6].Temperature = -10;
    //            dataPoints[6].Resistance = 8299;
    //        }
    //        else if (curve != PHTemp.RTDType.NTC_Type104)
    //        {
    //            if (curve != PHTemp.RTDType.NTC_Type105)
    //            {
    //                throw new Exception("Curve type not recognised");
    //            }
    //            dataPoints = new PHTemp.RTDDataPoint[9];
    //            dataPoints[0].Temperature = 55;
    //            dataPoints[0].Resistance = 27475;
    //            dataPoints[1].Temperature = 60;
    //            dataPoints[1].Resistance = 22590;
    //            dataPoints[2].Temperature = 65;
    //            dataPoints[2].Resistance = 18668;
    //            dataPoints[3].Temperature = 70;
    //            dataPoints[3].Resistance = 15052;
    //            dataPoints[4].Temperature = 75;
    //            dataPoints[4].Resistance = 12932;
    //            dataPoints[5].Temperature = 80;
    //            dataPoints[5].Resistance = 10837;
    //            dataPoints[6].Temperature = 85;
    //            dataPoints[6].Resistance = 9121;
    //            dataPoints[7].Temperature = 90;
    //            dataPoints[7].Resistance = 7708;
    //            dataPoints[8].Temperature = 95;
    //            dataPoints[8].Resistance = 6539;
    //        }
    //        else
    //        {
    //            dataPoints = new PHTemp.RTDDataPoint[8];
    //            dataPoints[0].Temperature = 25;
    //            dataPoints[0].Resistance = 15000;
    //            dataPoints[1].Temperature = 30;
    //            dataPoints[1].Resistance = 11933;
    //            dataPoints[2].Temperature = 35;
    //            dataPoints[2].Resistance = 9522;
    //            dataPoints[3].Temperature = 40;
    //            dataPoints[3].Resistance = 7657;
    //            dataPoints[4].Temperature = 45;
    //            dataPoints[4].Resistance = 6194;
    //            dataPoints[5].Temperature = 50;
    //            dataPoints[5].Resistance = 5039;
    //            dataPoints[6].Temperature = 55;
    //            dataPoints[6].Resistance = 4299;
    //            dataPoints[7].Temperature = 60;
    //            dataPoints[7].Resistance = 3756;
    //        }
    //        return dataPoints;
    //    }

    //    /// <summary>
    //    /// Returns the raw value of the RTD returned by the ADC in milliVolts;
    //    /// </summary>
    //    /// <returns></returns>
    //    public double ReadRawTemperature()
    //    {
    //        if (!isConnected)
    //            throw new Exception(connectionError);

    //        return adc.ReadChannel(channelTemp);
    //    }
    //    /// <summary>
    //    /// Read the temperature from the RTD sensor.
    //    /// </summary>
    //    /// <returns>The temperature read by the RTD in degrees Celcius.</returns>
    //    public double ReadTemperature()
    //    {
    //        double volts = ReadRawTemperature() / 1000;
    //        double r1 = rBridge;
    //        double r2 = rBridge;
    //        double r3 = rBridge;
    //        double t1 = (r1 + r2) * (volts / m_VoltageReferenceTemp);
    //        double rx = (r1 * r3 + r3 * t1) / (r2 - t1);

    //        //double k = 2.048 / volts;
    //        //double rx = (r3 * (r1 + r2) + k * r2) / (k * r1 - (r1 + r2));


    //        if (rx <= settings.TemperatureCurve[0].Resistance)
    //            throw new Exception(string.Concat("The resistance (", rx, " Ohms) was out of range of the supplied temperature curve."));

    //        var tempCurve = settings.TemperatureCurve;
    //        double deltaTemp = 0;
    //        double temp = (double)tempCurve[0].Temperature;

    //        int i = 1;
    //        while (i < tempCurve.Length)
    //        {
    //            deltaTemp = (double)(tempCurve[i].Temperature - tempCurve[i - 1].Temperature);
    //            if (rx >= tempCurve[i].Resistance)
    //            {
    //                temp += deltaTemp;
    //                i++;
    //            }
    //            else
    //                return temp + (rx - tempCurve[i - 1].Resistance) * deltaTemp / (tempCurve[i].Resistance - tempCurve[i - 1].Resistance);
    //        }

    //        return temp;
    //    }

    //    /// <summary>
    //    /// Returns the raw value of the ph Probe returned by the ADC in milliVolts.
    //    /// </summary>
    //    /// <returns></returns>
    //    public double ReadRawPH()
    //    {
    //        if (!isConnected)
    //            throw new Exception(connectionError);

    //        return adc.ReadChannel(channelPH);
    //    }
    //    /// <summary>
    //    /// Reads the pH from the pH probe with no temperature compensation.
    //    /// </summary>
    //    /// <returns></returns>
    //    public double ReadPH()
    //    {
    //        double value = ReadRawPH();
    //        value = value - m_VoltageReferencePh + phOffset;
    //        return 7 - value / phSlope;
    //    }
    //    /// <summary>
    //    /// Reads the pH from the pH probe with temperature compensation.
    //    /// </summary>
    //    /// <returns></returns>
    //    public double ReadTempCompensatedPH()
    //    {
    //        double temperatureSlopeModification = 0.1984 * ReadTemperature();

    //        double value = ReadRawPH();
    //        value = value - 1024 + phOffset;
    //        return 7 - value / (phSlope + temperatureSlopeModification);
    //    }

    //    /// <summary>
    //    /// Set the resolution of the analog to digital converter.
    //    /// Larger resolutions increase the conversion time.
    //    /// </summary>
    //    /// <param name="resolution"></param>
    //    public void SetResolution(MCP324xResolution resolution)
    //    {
    //        if (!isConnected)
    //            throw new Exception(connectionError);

    //        adc.Resolution = resolution;
    //    }

    //    /// <summary>
    //    /// The address jumper states.
    //    /// </summary>
    //    public enum JumperState : byte
    //    {
    //        Low,
    //        Floating,
    //        High
    //    }

    //    /// <summary>
    //    /// Data points of the RTD sensor curve to calculate the temperature.
    //    /// </summary>
    //    [Serializable]
    //    public struct RTDDataPoint
    //    {
    //        public int Temperature;
    //        public double Resistance;
    //    }

    //    public enum RTDType : byte
    //    {
    //        Pt1000,
    //        Pt100,
    //        PTC_Type201,
    //        NTC_Type101,
    //        NTC_Type102,
    //        NTC_Type103,
    //        NTC_Type104,
    //        NTC_Type105
    //    }

    //    private class PHTempSettings
    //    {
    //        public double pHOffset;
    //        public double pHSlope;
    //        public PHTemp.RTDDataPoint[] TemperatureCurve;
    //    }
    //}
}