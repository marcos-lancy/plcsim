using Siemens.Simatic.Simulation.Runtime;
using SimTableApplication.PLCSIM_Advanced.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using SimTableApplication.PLCSIM_Advanced.Interfaces;
using System.Diagnostics;
using SimTableApplication.Core.Utils;
using System.Globalization;
using SimTableApplication.PLCSIM_Advanced.Models;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Reflection;
using System.Collections.Generic;
using SimTableApplication.Core.Exceptions;

namespace SimTableApplication.PLCSIM_Advanced
{
    public class VirtualController : NotifyPropertyChanged, IVirtualController
    {
        #region Fields        

        public event EventHandler OnEndOfCycle;
        readonly object _objectLock = new object();

        private string _virtualControllerName;
        private string _storagePath;
        private List<string> _ipAddresses;
        private bool _isActiv;
        private bool _isUpToDate;
        private bool _isOff;

        private VirtualControllerType? _virtualControllerType;
        private IInstance _virtualControllerInstance;

        private ControllerLedMode _ledStop = ControllerLedMode.Invalid;
        private ControllerLedMode _ledRun = ControllerLedMode.Invalid;
        private ControllerLedMode _ledError = ControllerLedMode.Invalid;
        private ControllerLedMode _ledMaint = ControllerLedMode.Invalid;

        private ObservableCollection<SimTag> _allTags;

        #endregion

        #region Properties

        /// <summary>
        /// name of the virtual controller
        /// </summary>
        public string VirtualControllerName
        {
            get { return _virtualControllerName; }
            set
            {
                _virtualControllerName = value;
                OnPropertyChanged(nameof(VirtualControllerName));
            }
        }

        /// <summary>
        /// indicates if the virtual controller is activ
        /// </summary>
        public bool IsActiv
        {
            get
            {
                return _isActiv;
            }

            set
            {
                _isActiv = value;
                OnPropertyChanged(nameof(IsActiv));
            }
        }

        /// <summary>
        /// indicates if the virtual controller is up to date
        /// </summary>
        public bool IsUpToDate
        {
            get
            {
                return _isUpToDate;
            }
            set
            {
                _isUpToDate = value;
                OnPropertyChanged(nameof(IsUpToDate));
            }
        }

        /// <summary>
        /// indicates if the virtual controller is off
        /// </summary>
        public bool IsOff
        {
            get { return _isOff; }
            set
            {
                _isOff = value;
                OnPropertyChanged(nameof(IsOff));
            }
        }

        public VirtualControllerType? VirtualControllerType
        {
            get
            {
                return _virtualControllerType;
            }

            set
            {
                _virtualControllerType = value;
                OnPropertyChanged(nameof(VirtualControllerType));
            }
        }

        public IInstance VirtualControllerInstance
        {
            get
            {
                return _virtualControllerInstance;
            }

            set
            {
                _virtualControllerInstance = value;
                OnPropertyChanged(nameof(VirtualControllerInstance));
            }
        }

        public ControllerLedMode LedStop
        {
            get
            {
                return _ledStop;
            }

            set
            {
                _ledStop = value;
                OnPropertyChanged(nameof(LedStop));
            }
        }

        public ControllerLedMode LedRun
        {
            get
            {
                return _ledRun;
            }

            set
            {
                _ledRun = value;
                OnPropertyChanged(nameof(LedRun));
            }
        }

        public ControllerLedMode LedError
        {
            get
            {
                return _ledError;
            }

            set
            {
                _ledError = value;
                OnPropertyChanged(nameof(LedError));
            }
        }

        public ControllerLedMode LedMaint
        {
            get
            {
                return _ledMaint;
            }

            set
            {
                _ledMaint = value;
                OnPropertyChanged(nameof(LedMaint));
            }
        }

        public string StoragePath
        {
            get
            {
                return _storagePath;
            }

            set
            {
                _storagePath = value;
                OnPropertyChanged(nameof(StoragePath));
            }
        }

        public ObservableCollection<SimTag> AllTags
        {
            get
            {
                return _allTags;
            }
            set
            {
                _allTags = value;
                OnPropertyChanged(nameof(AllTags));
            }
        }


        event EventHandler IVirtualController.OnEndOfCycle
        {
            add
            {
                lock (_objectLock)
                {
                    OnEndOfCycle += value;
                }
            }

            remove
            {
                lock (_objectLock)
                {
                    OnEndOfCycle -= value;
                }

            }
        }

        public List<String> IpAddresses
        {
            get { return _ipAddresses; }
            set
            {
                _ipAddresses = value;
                OnPropertyChanged(nameof(IpAddresses));
            }
        }



        #endregion

        #region Ctor

        /// <summary>
        /// construct virtual controller object
        /// </summary>
        /// <param name="controllerName">name of the virtual controller</param>
        /// <param name="controllerType">type of virtual controller </param>
        /// <param name="storagePath">path where the virtual controller data should be stored</param>
        public VirtualController(string controllerName, VirtualControllerType? controllerType, string storagePath)
        {
            if (String.IsNullOrEmpty(controllerName))
                throw new ArgumentException("Name of the Virtual Controller is null or empty", nameof(controllerName));

            if (controllerType == null)
                throw new ArgumentNullException(nameof(controllerType), "virtual controller type is null");

            VirtualControllerInstance = null;
            AllTags = new ObservableCollection<SimTag>();

            VirtualControllerName = controllerName;
            VirtualControllerType = controllerType;
            StoragePath = storagePath;

            if (SimulationRuntimeManager.IsInitialized)
            {
                var info = SimulationRuntimeManager.RegisteredInstanceInfo.SingleOrDefault(x => x.Name.Equals(VirtualControllerName));

                try
                {
                    //attach if Instance exists
                    if (info.Name != null)
                    {

                        VirtualControllerInstance = SimulationRuntimeManager.CreateInterface(info.ID);
                        VirtualControllerInstance.IsSendSyncEventInDefaultModeEnabled = true;
                        VirtualControllerInstance.OperatingMode = EOperatingMode.Default;
                        VirtualControllerInstance.OnLedChanged += OnLedChanged;
                        VirtualControllerInstance.OnSyncPointReached += OnSyncPointReached;
                        VirtualControllerInstance.OnConfigurationChanged += OnConfigurationChanged;


                        if (!VirtualControllerInstance.OperatingState.Equals(EOperatingState.Off))
                        {
                            VirtualControllerInstance.UpdateTagList(ETagListDetails.IOCTDB);
                            ConvertSTagInfo(VirtualControllerInstance.TagInfos);
                            IsActiv = true;
                        }
                    }
                    //create new instance if not existing
                    else
                    {
                        VirtualControllerInstance = SimulationRuntimeManager.RegisterInstance(VirtualControllerName);
                        VirtualControllerInstance.CPUType = (ECPUType)VirtualControllerType;
                        VirtualControllerInstance.StoragePath = storagePath;
                        VirtualControllerInstance.OperatingMode = EOperatingMode.Default;
                        VirtualControllerInstance.IsSendSyncEventInDefaultModeEnabled = true;
                        VirtualControllerInstance.OnLedChanged += OnLedChanged;
                        VirtualControllerInstance.OnSyncPointReached += OnSyncPointReached;
                        VirtualControllerInstance.OnConfigurationChanged += OnConfigurationChanged;

                    }

                }
                catch (SimulationRuntimeException simEx)
                {
                    if (simEx.RuntimeErrorCode.Equals(ERuntimeErrorCode.LimitReached))
                    {
                        throw new VirtualControllerException(string.Format(CultureInfo.InvariantCulture, "Cannot activate PLCSIM Advanced Instance {0} because Limit of 16 Instances was reached", VirtualControllerName), simEx);

                    }
                    if (simEx.RuntimeErrorCode.Equals(ERuntimeErrorCode.AlreadyExists))
                    {
                        throw new VirtualControllerException(string.Format(CultureInfo.InvariantCulture, "Cannot activate PLCSIM Advanced Instance {0} because a instance with same name already exists", VirtualControllerName), simEx);

                    }

                }
            }

            else
            {
                throw new VirtualControllerException("SimulationRuntimeManager is not initialized!");
            }

        }


        #endregion

        #region Methods

        /// <summary>
        /// power on virtual controller
        /// </summary>
        public void PowerOn()
        {
            VirtualControllerInstance.PowerOn(10 * 1000);

            IsOff = false;
            IsActiv = true;

        }

        /// <summary>
        /// power off virtual controller
        /// </summary>
        public void PowerOff()
        {
            if (IsActiv)
            {
                IsOff = true;
                VirtualControllerInstance.PowerOff(6000);
                IsUpToDate = false;
            }

            IsActiv = false;

        }

        /// <summary>
        /// set virtual controller in RUN mode
        /// </summary>
        public void Run()
        {
            try
            {
                VirtualControllerInstance.Run(10 * 1000);

                while (VirtualControllerInstance.OperatingState != EOperatingState.Run)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (SimulationRuntimeException simEx)
            {
                if (simEx.RuntimeErrorCode.Equals(ERuntimeErrorCode.IsEmpty))
                {
                    throw new VirtualControllerException("Virtual Controller is empty. Please Download first!");

                }


            }
        }

        /// <summary>
        /// set virtual controller in STOP mode
        /// </summary>
        public void Stop()
        {
            if (VirtualControllerInstance.OperatingState == EOperatingState.Stop || VirtualControllerInstance.OperatingState == EOperatingState.Off)
                return;

            VirtualControllerInstance.Stop(10 * 1000);

            while (VirtualControllerInstance.OperatingState != EOperatingState.Stop)
            {
                Thread.Sleep(1000);
            }


        }

        /// <summary>
        /// open the storage location of the virtual memory card
        /// </summary>
        /// <param name="path"></param>
        public void ShowVirtualMemoryCard(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path of Virtual Memory Card is null or empty", nameof(path));
            }

            Process.Start(NativeMethods.WindowsExplorer, string.Format(CultureInfo.InvariantCulture, "/select,\"{0}\"", path));
        }

        /// <summary>
        /// reset of memory
        /// </summary>
        public void MemoryReset()
        {
            VirtualControllerInstance.MemoryReset(60000);
        }

        /// <summary>
        /// reads all tags from the internal image of the virtual controller
        /// </summary>
        /// <param name="tagsToRead">observable collection containing tags to read</param>
        public void ReadTags(ObservableCollection<SimTag> tagsToRead)
        {
            if (tagsToRead == null)
            {
                throw new ArgumentNullException(nameof(tagsToRead), "Collection of Type <SimTag> is null");
            }

            int length = tagsToRead.Count(x => x.IsValid);
            if (length < 1) return;
            SDataValueByName[] tags = new SDataValueByName[length];

            for (int i = 0, j = 0; i < tagsToRead.Count && j < tags.Length; i++)
            {
                if (tagsToRead[i].IsValid)
                {
                    tags[j++].Name = tagsToRead[i].TagName;
                }
            }

            if (IsUpToDate)
            {
                if (!IsOff)
                {
                    try
                    {
                        VirtualControllerInstance.ReadSignals(ref tags);

                        foreach (var item in tags)
                        {
                            var tag = tagsToRead.First(x => x.TagName == item.Name);

                            tag.Value = typeof(SDataValue).GetProperty(item.DataValue.Type.ToString())?.GetValue(item.DataValue, null);
                        }
                    }
                    catch (SimulationRuntimeException simEx)
                    {
                        if (simEx.RuntimeErrorCode.Equals(ERuntimeErrorCode.NotUpToDate))
                        {
                            IsUpToDate = false;
                        }
                    }


                }
            }


        }

        /// <summary>
        /// writes all tags into internal image of the virtual controller
        /// </summary>
        /// <param name="tagsToWrite">observable collection containing tags to write</param>
        /// <exception cref="VirtualControllerException"></exception>
        /// <exception cref="FormatException"></exception>
        public void WriteTags(IEnumerable<SimTag> tagsToWrite)
        {
            if (tagsToWrite == null)
            {
                throw new ArgumentNullException(nameof(tagsToWrite), "Collection of Type <SimTag> is null");
            }

            foreach (var tag in tagsToWrite)
            {
                if (tag.ModifyValue != null)
                {
                    try
                    {
                        //get copy of tag to avoid UI change after remove of 16#
                        SimTag simTagCopy = (SimTag)tag.Clone();

                        //read simTag current value to get the primitiveDataType
                        SDataValue currentValue = _virtualControllerInstance.Read(simTagCopy.TagName);

                        EPrimitiveDataType dataType = currentValue.Type;

                        SDataValue newValue = new SDataValue
                        {
                            Type = dataType
                        };

                        PropertyInfo propertyInfo = typeof(SDataValue).GetProperty(dataType.ToString());

                        object clipboardNewValue = newValue;
                        try
                        {
                            //convert value specific UI presentations
                            simTagCopy.ModifyValue = FormatHelper.ConvertValue(simTagCopy.DataType, (string)simTagCopy.ModifyValue);

                            if (propertyInfo != null)
                            {
                                //object value = Convert.ChangeType(simTagCopy.ModifyValue, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
                                propertyInfo.SetValue(clipboardNewValue, simTagCopy.ModifyValue, null);
                            }

                            newValue = (SDataValue)clipboardNewValue;

                            _virtualControllerInstance.Write(simTagCopy.TagName, newValue);
                        }
                        catch (FormatException)
                        {
                            throw;
                        }


                    }
                    catch (SimulationRuntimeException s)
                    {
                        throw new VirtualControllerException(string.Format(CultureInfo.InvariantCulture, "You tried to write a new value to a variable, that is not supporting this yet."), s);
                    }

                }
            }
        }

        /// <summary>
        /// unregister virtual controller from simulation runtime manager
        /// </summary>
        public void UnRegisterVirtualController()
        {
            LedStop = ControllerLedMode.Invalid;
            LedRun = ControllerLedMode.Invalid;
            LedError = ControllerLedMode.Invalid;
            LedMaint = ControllerLedMode.Invalid;

            if (VirtualControllerInstance == null) return;

            VirtualControllerInstance.OnLedChanged -= OnLedChanged;
            VirtualControllerInstance.UnregisterInstance();
            VirtualControllerInstance.Dispose();

            VirtualControllerInstance = null;

        }

        #endregion

        #region Event executions

        private void OnLedChanged(IInstance inSender, ERuntimeErrorCode inErrorCode, DateTime inDateTime, ELEDType inLedType, ELEDMode inLedMode)
        {
            var ledMode = ControllerLedMode.Invalid;

            switch (inLedMode)
            {
                case ELEDMode.Off:
                    ledMode = ControllerLedMode.Off;
                    break;
                case ELEDMode.On:
                    ledMode = ControllerLedMode.On;
                    break;
                case ELEDMode.FlashFast:
                    ledMode = ControllerLedMode.FlashFast;
                    break;
                case ELEDMode.FlashSlow:
                    ledMode = ControllerLedMode.FlashSlow;
                    break;
                case ELEDMode.Invalid:
                    ledMode = ControllerLedMode.Invalid;
                    break;
            }

            switch (inLedType)
            {
                case ELEDType.Stop:
                    LedStop = ledMode;
                    break;
                case ELEDType.Run:
                    LedRun = ledMode;
                    break;
                case ELEDType.Error:
                    LedError = ledMode;
                    break;
                case ELEDType.Maint:
                    LedMaint = ledMode;
                    break;
            }
        }

        private void OnConfigurationChanged(IInstance inSender, ERuntimeErrorCode inErrorCode, DateTime inDateTime, EInstanceConfigChanged inInstanceConfigChanged, uint inParam1, uint inParam2, uint inParam3, uint inParam4)
        {
            IsUpToDate = false;            

            if (inInstanceConfigChanged == EInstanceConfigChanged.HardwareSoftwareChanged)
            {
                try
                {
                    int i = 0;
                    //If HW or SW changed and PLC is not in RUN and STOP mode --> wait 
                    while (!inSender.OperatingState.Equals(EOperatingState.Run) && !inSender.OperatingState.Equals(EOperatingState.Stop) || i <= 10)
                    {
                        i++;
                        Thread.Sleep(100);
                    }

                    //update Taglist
                    inSender.UpdateTagList(ETagListDetails.IOMCTDB, false);                   

                    //get new Tags
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate { ConvertSTagInfo(inSender.TagInfos); });

                    IsUpToDate = true;

                }
                catch (SimulationRuntimeException simEx)
                {
                    throw new VirtualControllerException(simEx.Message);
                }


            }

            if (inInstanceConfigChanged == EInstanceConfigChanged.IPChanged)
            {
                IpAddresses = inSender.ControllerIP.ToList();
            }

            IsUpToDate = true;
        }

        private void OnSyncPointReached(IInstance inSender, ERuntimeErrorCode inErrorCode, DateTime inDateTime, uint inPipId, long inTimeSinceSameSyncPointNs, long inTimeSinceAnySyncPointNs, uint inSyncPointCount)
        {
            OnEndOfCycle?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Private


        /// <summary>
        /// Convert STagInfo into SimTag Object
        /// </summary>
        /// <param name="tagInfos"></param>
        private void ConvertSTagInfo(STagInfo[] tagInfos)
        {
            if (tagInfos == null)
            {
                throw new ArgumentNullException(nameof(tagInfos), "STagInfo[] Array is null.");
            }
            AllTags.Clear();

            foreach (STagInfo info in tagInfos)
            {
                if (!(info.PrimitiveDataType == EPrimitiveDataType.Struct || info.PrimitiveDataType == EPrimitiveDataType.Unspecific ))
                {
                    var dimension = info.Dimension;  
                                      
                    //add tag when it is not the root of array
                    if (dimension.Length == 0)
                    {
                        SimTag tag = new SimTag
                        {
                            TagName = info.Name,
                            DataType = (SimDataType)info.DataType,
                            Area = info.Area.ToString(),
                            IsValid = true,
                            Value = null
                        };

                        if (!AllTags.Contains(tag))
                        {
                            AllTags.Add(tag);
                        }
                    }

                    
                }
            }
        }


        #endregion

    }
}
