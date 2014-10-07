using MFE.Core;
using System;

namespace MFE.SmartNetwork.Network
{
    public class ControlLine : INotifyPropertyChanged
    {
        #region Fields
        private short[] state = new short[2];
        private string name = "";
        #endregion

        #region Properties
        public BusHubBase BusHub
        {
            get;
            private set;
        }
        public BusModule BusModule
        {
            get;
            private set;
        }
        public ControlLineType Type
        {
            get;
            private set;
        }
        public byte Address // up to 256 lines in a single module
        {
            get;
            private set;
        }
        public string TypeName
        {
            get
            {
                string type;

                switch (Type)
                {
                    case ControlLineType.PWM: type = "PWM"; break;
                    case ControlLineType.Relay: type = "Relay"; break;
                    case ControlLineType.Liquid: type = "Liquid"; break;
                    case ControlLineType.Ph: type = "Ph"; break;
                    default: type = "[Unknown]"; break;
                }

                return "[" + (BusHub != null ? BusHub.Address.ToString() : "-") + "][" + (BusModule != null ? BusModule.Address.ToString() : "-") + "] " + type + " #" + Address;
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
        public short[] State
        {
            get
            {
                short[] result = new short[state.Length];
                Array.Copy(state, result, state.Length);
                return result;
            }
            private set
            {
                if (value.Length == state.Length)
                    for (ushort i = 0; i < state.Length; i++)
                        if (state[i] != value[i])
                        {
                            Array.Copy(value, state, state.Length);
                            NotifyPropertyChanged("State");
                            break;
                        }
            }
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
        public ControlLine(BusHubBase busHub, BusModule busModule, ControlLineType type, byte address)
        {
            BusHub = busHub;
            BusModule = busModule;
            Type = type;
            Address = address;

            for (byte i = 0; i < state.Length; i++)
                state[i] = 0;
        }
        #endregion

        #region Public methods
        public void QueryState()
        {
            if (BusHub != null && BusModule != null)
            {
                short[] response = new short[state.Length];
                if (BusHub.BusModuleWriteRead(BusModule, new byte[] { BusModuleAPI.CmdGetControlLineState, (byte)Type, Address }, response))
                    state = response;
            }
        }
        public void SetState(byte[] state)
        {
            if (BusHub != null && BusModule != null)
            {
                byte[] data = new byte[3 + state.Length];
                data[0] = BusModuleAPI.CmdSetControlLineState;
                data[1] = (byte)Type;
                data[2] = Address;
                Array.Copy(state, 0, data, 3, state.Length);

                byte[] response = new byte[state.Length];
                if (BusHub.BusModuleWriteRead(BusModule, data, response))
                    State = response;
            }
        }
        #endregion
    }
}
