using MFE.Core;
using System.Collections;

namespace MFE.SmartNetwork.Network
{
    public class BusModule : INotifyPropertyChanged
    {
        #region Fields
        private BusMasterBase busMaster;
        private byte[] address;
        private BusModuleType type = BusModuleType.Unknown;
        private string name = "";
        private ArrayList controlLines = new ArrayList();
        #endregion

        #region Properties
        public BusMasterBase BusMaster
        {
            get { return busMaster; }
        }
        public byte[] Address
        {
            get { return address; }
        }
        public BusModuleType Type
        {
            get { return type; }
        }
        public string TypeName
        {
            get
            {
                switch (type)
                {
                    case BusModuleType.Unknown: return "AE test module";
                    case BusModuleType.D5: return "AE-D5";
                    case BusModuleType.D6: return "AE-D6";
                    case BusModuleType.D8: return "AE-D8";


                    default: return type.ToString() + " [Unknown]";
                }
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public ArrayList ControlLines
        {
            get { return controlLines; }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, propertyName);
        }
        #endregion

        #region Constructor
        public BusModule(BusMasterBase busMaster, byte[] address, BusModuleType type)
        {
            this.busMaster = busMaster;
            this.address = address;
            this.type = type;
        }
        #endregion

        #region Private methods
        //internal void QueryType()
        //{
        //    if (busMaster != null)
        //    {
        //        byte[] response = new byte[1];
        //        if (busMaster.BusModuleWriteRead(this, new byte[] { BusModuleAPI.CmdGetType }, response))
        //            Type = (BusModuleType)response[0];
        //    }
        //}
        //internal void QueryControlLines(bool updateState = false)
        //{
        //    if (busHub != null)
        //    {
        //        for (byte type = 0; type < BusModuleAPI.ControlLineTypesToRequest; type++)
        //        {
        //            byte[] response = new byte[1]; // up to 256 numbers for one type
        //            if (busHub.BusModuleWriteRead(this, new byte[] { BusModuleAPI.CmdGetControlLineCount, type }, response))
        //            {
        //                for (byte number = 0; number < response[0]; number++)
        //                {
        //                    ControlLine controlLine = new ControlLine(busHub, this, (ControlLineType)type, number);
        //                    ControlLines.Add(controlLine);

        //                    // query control line state:
        //                    if (updateState)
        //                        controlLine.QueryState();
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
