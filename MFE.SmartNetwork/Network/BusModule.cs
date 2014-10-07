using MFE.Core;
using System.Collections;

namespace MFE.SmartNetwork.Network
{
    public class BusModule : INotifyPropertyChanged
    {
        #region Fields
        private BusHubBase busHub;
        private ushort address = 0;
        private BusModuleType type = BusModuleType.Unknown; // unknown
        private string name = "";
        private ArrayList controlLines = new ArrayList();
        #endregion

        #region Properties
        public BusHubBase BusHub
        {
            get { return busHub; }
        }
        public ushort Address
        {
            get { return address; }
        }
        public BusModuleType Type
        {
            get { return type; }
            //private set
            //{
            //    if (type != value)
            //    {
            //        type = value;
            //        NotifyPropertyChanged("Type");
            //    }
            //}
        }
        public string TypeName
        {
            get
            {
                switch (type)
                {
                    case BusModuleType.Test: return "AE test full module";
                    case BusModuleType.AER8: return "AE-D8";


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
        public BusModule(BusHubBase busHub, ushort address, BusModuleType type)
        {
            this.busHub = busHub;
            this.address = address;
            this.type = type;
        }
        #endregion

        #region Private methods
        //internal void QueryType()
        //{
        //    if (busHub != null)
        //    {
        //        byte[] response = new byte[1];
        //        if (busHub.BusModuleWriteRead(this, new byte[] { BusModuleAPI.CmdGetType }, response))
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
