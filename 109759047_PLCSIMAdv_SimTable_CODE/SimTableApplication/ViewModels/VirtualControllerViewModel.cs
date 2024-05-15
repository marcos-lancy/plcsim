using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SimTableApplication.PLCSIM_Advanced.Interfaces;
using SimTableApplication.PLCSIM_Advanced.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using SimTableApplication.Models;
using GalaSoft.MvvmLight.Messaging;
using SimTableApplication.PLCSIM_Advanced;
using System.Linq;
using MvvmDialogs;
using System.Windows;
using System.Collections.Specialized;
using SimTableApplication.Services.Interfaces;
using System.Collections.Generic;
using SimTableApplication.Core.Exceptions;

namespace SimTableApplication.ViewModels
{
    /// <summary>
    /// ViewModel for Controller in UI
    /// </summary>
    public class VirtualControllerViewModel : ViewModelBase
    {
        #region Fields

        private readonly IVirtualController _controller;
        private readonly IDialogService _dialogService;
        private readonly IBackgroundTaskService _backgroundTaskService;

        private VirtualControllerModel _vcModel;
        private ObservableCollection<SimTableDirectoryViewModel> _simTableDirectory;        
        private string _name;
        private string _controllerType;
        private bool _isBusy;      
        private bool _isModified;
        private bool _isSelected;

        #endregion

        #region Properties

        /// <summary>
        /// model of the controller
        /// </summary>
        public VirtualControllerModel VcModel
        {
            get { return _vcModel; }
            set
            {
                _vcModel = value;
                RaisePropertyChanged(nameof(VcModel));
            }
        }

        /// <summary>
        /// collection of simulation tables
        /// </summary>
        public ObservableCollection<SimTableDirectoryViewModel> SimTableDirectory
        {
            get { return _simTableDirectory; }
            set
            {
                _simTableDirectory = value;
                RaisePropertyChanged(nameof(SimTableDirectory));
            }
        }

        /// <summary>
        /// Ip Addresses provided by the virtual controller
        /// </summary>
        public List<string> IpAddresses
        {
            get { return _controller.IpAddresses; }
         
        }

        /// <summary>
        /// name of the controller for UI 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }        

        /// <summary>
        /// type of the controller
        /// </summary>
        public string ControllerType
        {
            get { return _controllerType; }
            set
            {
                _controllerType = value;
                RaisePropertyChanged(nameof(ControllerType));
            }
        }
       
        /// <summary>
        /// indicates if backgroundworker is busy
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));
            }
        }        

        /// <summary>
        /// indicates modifications done by user 
        /// </summary>
        public bool IsModified
        {
            get { return _isModified; }
            set
            {

                var oldValue = _isModified;
                _isModified = value;

                RaisePropertyChanged(nameof(IsModified), oldValue, value, true);
            }
        }
       
        /// <summary>
        /// indicates if the item is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }               


        /// <summary>
        /// LED Stop
        /// </summary>
        public ControllerLedMode LedStop
        {
            get
            {
                return _controller?.LedStop ?? ControllerLedMode.Invalid;
            }
        }

        /// <summary>
        /// LED Run
        /// </summary>
        public ControllerLedMode LedRun
        {
            get
            {
                return _controller?.LedRun ?? ControllerLedMode.Invalid;
            }
        }

        /// <summary>
        /// LED Error
        /// </summary>
        public ControllerLedMode LedError
        {
            get
            {
                return _controller?.LedError ?? ControllerLedMode.Invalid;
            }
        }

        /// <summary>
        /// LED maintenance
        /// </summary>
        public ControllerLedMode LedMaint
        {
            get
            {
                return _controller?.LedMaint ?? ControllerLedMode.Invalid;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Construct viewmodel for controller
        /// </summary>
        /// <param name="model">model for controller</param>
        /// <param name="dialogService">dialog service for dialoghandling</param>
        /// <param name="backgroundTaskService">background Task Service</param>
        public VirtualControllerViewModel(VirtualControllerModel model, IDialogService dialogService, IBackgroundTaskService backgroundTaskService)
        {
            VcModel = model;
            _dialogService = dialogService;
            _backgroundTaskService = backgroundTaskService;

            SimTableDirectory = new ObservableCollection<SimTableDirectoryViewModel>();            

            Name = VcModel.VirtualControllerName;
            ControllerType = VcModel.VirtualControllerType.ToString();

            try
            {
                _controller = new VirtualController(VcModel.VirtualControllerName, VcModel.VirtualControllerType, VcModel.VirtualControllerPath);
            }
            catch (VirtualControllerException vcException)
            {

                _dialogService.ShowMessageBox(this, string.Format("Message: {0}, InnerException: {1} ", vcException.Message, vcException.InnerException.ToString()),"Exception", MessageBoxButton.OKCancel,MessageBoxImage.Error,MessageBoxResult.OK);
                
            }
                     
            
            foreach (var simModel in VcModel.SimTableDirectoryModels)
            {
                SimTableDirectory.Add(new SimTableDirectoryViewModel(simModel, _controller, _dialogService, _backgroundTaskService));
            }

            //Add initial SimTable Directory if not existing
            if (SimTableDirectory.Count<1)
            {
                SimTableDirectoryModel simTableDirModel = VcModel.AddNewSimTableDirectory();
                SimTableDirectory.Add(new SimTableDirectoryViewModel(simTableDirModel, _controller, _dialogService, _backgroundTaskService));
            }           

            _controller.PropertyChanged += (sender, args) =>
            {
                RaisePropertyChanged(args.PropertyName);                
            };        

        }

        #endregion

        #region Commands

        /// <summary>
        /// power on controller
        /// </summary>
        public ICommand PowerOnCommand => new RelayCommand(OnPowerOn, CanPowerOn);

        /// <summary>
        /// power off controller
        /// </summary>
        public ICommand PowerOffCommand => new RelayCommand(OnPowerOff, CanPowerOff);

        /// <summary>
        /// set controller in run mode
        /// </summary>
        public ICommand RunCommand => new RelayCommand(OnRun, CanRun);

        /// <summary>
        /// set controller in stop mode
        /// </summary>
        public ICommand StopCommand => new RelayCommand(OnStop, CanStop);

        /// <summary>
        /// reset memory card of controller
        /// </summary>
        public ICommand ResetMemoryCardCommand => new RelayCommand(OnResetMemoryCard, CanResetMemoryCard);

        /// <summary>
        /// open filepath of virtual memory card
        /// </summary>
        public ICommand ShowMemoryCardCommand => new RelayCommand(OnShowMemoryCard, CanShowMemoryCard);


        #endregion

        #region Private Methods

        private bool CanPowerOn()
        {
            return !_controller.IsActiv;
        }
        private void OnPowerOn()
        {
            if (!CanPowerOn())
            {
                return;
            }

            _backgroundTaskService.PerformInBackground(PowerOn, PowerOnFinished, null);
           
        }       
        private bool PowerOn(IBackgroundTask backgroundTask, object taskData)
        {
            backgroundTask.ReportProgress(0);

            _controller.PowerOn();
            ((RelayCommand)RunCommand).RaiseCanExecuteChanged();
            backgroundTask.ReportProgress(100);
            return true;
        }
        private void PowerOnFinished(IFinishedBackgroundTask<bool> result)
        {
            if (result.Exception != null)
                _dialogService.ShowMessageBox(this,result.Exception.Message, result.Exception.GetType().ToString(), MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
        private bool CanPowerOff()
        {
            return _controller.IsActiv;
        }
        private void OnPowerOff()
        {
            if (!CanPowerOff())
            {
                return;
            }

            _backgroundTaskService.PerformInBackground(PowerOff, PowerOffFinished, null);

        }
        private bool PowerOff(IBackgroundTask backgroundTask, object taskData)
        {
            backgroundTask.ReportProgress(0);

            _controller.PowerOff();            

            backgroundTask.ReportProgress(100);
            return true;
        }
        private void PowerOffFinished(IFinishedBackgroundTask<bool> result)
        {
            if (result.Exception != null)
                _dialogService.ShowMessageBox(this, result.Exception.Message, result.Exception.GetType().ToString(), MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
        private void OnRun()
        {
            if (CanRun())
            {
                try
                {
                    _controller.Run();
                }
                catch (VirtualControllerException vcEx)
                {
                    _dialogService.ShowMessageBox(this, vcEx.Message, "Initial Download required!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                }
                
            }

        }
        private bool CanRun()
        {
            return _controller.IsActiv;
        }
        private void OnStop()
        {
            if (CanStop())
            {
                _controller.Stop();
            }


        }
        private bool CanStop()
        {
            return _controller.IsActiv;
        }
        private bool CanResetMemoryCard()
        {
            return _controller.IsActiv;
        }
        private void OnResetMemoryCard()
        {
            if (!CanResetMemoryCard())
            {
                return;
            }

            _backgroundTaskService.PerformInBackground(ResetMemoryCard, ResetMemoryCardFinished, null);
            
        }
        private bool ResetMemoryCard(IBackgroundTask backgroundTask, object taskData)
        {
            backgroundTask.ReportProgress(0);

            _controller.MemoryReset();

            backgroundTask.ReportProgress(100);
            return true;

        }
        private void ResetMemoryCardFinished(IFinishedBackgroundTask<bool> result)
        {
            if (result.Exception != null)
                _dialogService.ShowMessageBox(this, result.Exception.Message, result.Exception.GetType().ToString(), MessageBoxButton.OK,
                    MessageBoxImage.Error);

        }
        private bool CanShowMemoryCard()
        {
            return _controller.IsActiv;
        }
        private void OnShowMemoryCard()
        {
            _controller.ShowVirtualMemoryCard(_controller.StoragePath);
        }      
        
        
        #endregion

        #region Public Methods

        /// <summary>
        /// close routine for Controller
        /// </summary>
        public void Close()
        {
            foreach (var simTableDirVm in SimTableDirectory)
            {
                simTableDirVm.Close();
            }

            _controller.PowerOff();
            _controller.UnRegisterVirtualController();

        }

        #endregion        
       
    }
}
