using MFE.Core;
using System.Collections;
using System.Threading;

namespace MFE.SmartNetwork.Network
{
    public abstract class BusHubBase : INotifyPropertyChanged
    {
        #region Fields
        private const int updateInterval = 3000;

        private uint address = 0;
        private string name = "";
        private ArrayList busModules = new ArrayList();
        private ArrayList busControlLines = new ArrayList();
        private Timer timerUpdate = null;
        #endregion

        #region Properties
        public uint Address
        {
            get { return address; }
        }
        public string ProductName
        {
            get { return (this is BusHubI2C) ?  "Integrated Hub" : "RF Hub"; }
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

        public ArrayList BusModules
        {
            get { return busModules; }
        }
        public BusModule this[ushort busModuleAddress]
        {
            get
            {
                foreach (BusModule busModule in busModules)
                    if (busModule.Address == busModuleAddress)
                        return busModule;

                return null;
            }
        }
        
        public ArrayList BusControlLines // control lines of all bus modules
        {
            get { return busControlLines; }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event CollectionChangedEventHandler BusModulesCollectionChanged;
        public event CollectionChangedEventHandler BusControlLinesCollectionChanged;
        #endregion

        #region Constructors
        public BusHubBase(uint address)
        {
            this.address = address;
            StartTimer();
        }
        #endregion

        #region Protected methods
        protected abstract void ScanModules(out ArrayList modulesAdded, out ArrayList modulesRemoved);
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, propertyName);
        }
        
        internal abstract bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response);
        internal abstract bool BusModuleWrite(BusModule busModule, byte[] request);
        #endregion

        #region Private methods
        private void StartTimer()
        {
            timerUpdate = new Timer((state) => {
                StopTimer();
                Update();
                StartTimer();
            }, null, updateInterval, updateInterval);
        }
        private void StopTimer()
        {
            timerUpdate.Change(Timeout.Infinite, Timeout.Infinite);
        }
        private void Update()
        {
            ArrayList modulesAdded, modulesRemoved;
            ScanModules(out modulesAdded, out modulesRemoved);

            ArrayList controlLinesAdded = new ArrayList();
            ArrayList controlLinesRemoved = new ArrayList();

            // removed control lines:
            foreach (BusModule moduleRemoved in modulesRemoved)
                foreach (ControlLine controlLine in busControlLines)
                {
                    if (controlLine.BusModule.Address == moduleRemoved.Address)
                    {
                        busControlLines.Remove(controlLine);
                        controlLinesRemoved.Add(controlLine);
                    }
                }

            // added control lines:
            foreach (BusModule moduleAdded in modulesAdded)
            {
                BusModule busModule = this[moduleAdded.Address];
                foreach (ControlLine controlLine in busModule.ControlLines)
                {
                    busControlLines.Add(controlLine);
                    controlLinesAdded.Add(controlLine);
                }
            }

            // BusModulesCollectionChanged:
            if (BusModulesCollectionChanged != null && (modulesAdded.Count != 0 || modulesRemoved.Count != 0))
                BusModulesCollectionChanged(modulesAdded, modulesRemoved);

            // BusControlLinesCollectionChanged:
            if (BusControlLinesCollectionChanged != null && (controlLinesAdded.Count != 0 || controlLinesRemoved.Count != 0))
                BusControlLinesCollectionChanged(controlLinesAdded, controlLinesRemoved);
        }
        #endregion
    }
}
