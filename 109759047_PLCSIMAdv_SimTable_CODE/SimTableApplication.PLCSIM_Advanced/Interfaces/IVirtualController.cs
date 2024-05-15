using SimTableApplication.PLCSIM_Advanced.Models;
using SimTableApplication.PLCSIM_Advanced.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SimTableApplication.PLCSIM_Advanced.Interfaces
{
    public interface IVirtualController : INotifyPropertyChanged
    {
        #region Properties      

        /// <summary>
        /// Name of the PLCSIM Adv. instance
        /// </summary>
        string VirtualControllerName { get; set; }

        /// <summary>
        /// Type of virtual controller (S7-1500 or ET200SP)
        /// </summary>
        VirtualControllerType? VirtualControllerType { get; set; }

        /// <summary>
        /// IP Addresses of controller
        /// </summary>
        List<string> IpAddresses { get; set; }

        /// <summary>
        /// Specifies if controller is activ or not
        /// </summary>
        bool IsActiv { get; set; }    
        
        /// <summary>
        /// Storage Path of the virtual controller instance
        /// </summary>
        string StoragePath { get; set; }

        /// <summary>
        /// all available tags within instance storage
        /// </summary>
        ObservableCollection<SimTag> AllTags { get; set; }

        /// <summary>
        /// specifies if symbolic taglist of controller is uptodate
        /// </summary>
        bool IsUpToDate { get; set; }

        ControllerLedMode LedStop { get; set; }
        ControllerLedMode LedRun { get; set; }
        ControllerLedMode LedError { get; set; }
        ControllerLedMode LedMaint { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Power On Virtual Controller
        /// </summary>
        void PowerOn();

        /// <summary>
        /// Power Off Virtual Controller
        /// </summary>
        void PowerOff();

        /// <summary>
        /// set virtual controller into run mode
        /// </summary>
        void Run();

        /// <summary>
        /// set virtual controller into stop mode
        /// </summary>
        void Stop();

        /// <summary>
        /// shows the folder of the virtual memory card
        /// </summary>
        /// <param name="path"></param>
        void ShowVirtualMemoryCard(string path);

        /// <summary>
        /// executes the MRES of the virtual controller
        /// </summary>
        void MemoryReset();

        /// <summary>
        /// Reads all Tags given within collection
        /// </summary>
        /// <param name="tagsToRead">collection containing all tags to read</param>
        void ReadTags(ObservableCollection<SimTag> tagsToRead);

        /// <summary>
        /// Writes all Tags given within collection
        /// </summary>
        /// <param name="tagsToWrite">collection containing all tags to write</param>
        void WriteTags(IEnumerable<SimTag> tagsToWrite);

        /// <summary>
        /// unregister the virtual Controller from the Simulation Runtime Manager
        /// </summary>
        void UnRegisterVirtualController();  

        #endregion

        #region Events

        event EventHandler OnEndOfCycle;

        #endregion

    }
}
