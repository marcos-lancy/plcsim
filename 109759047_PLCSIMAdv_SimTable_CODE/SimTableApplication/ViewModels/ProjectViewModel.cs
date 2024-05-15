using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using SimTableApplication.Models;
using SimTableApplication.ViewModels.DialogViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SimTableApplication.Views.Dialogs;
using SimTableApplication.Services.Interfaces;

namespace SimTableApplication.ViewModels
{
    /// <summary>
    /// ViewModel for Project in UI
    /// </summary>
    public class ProjectViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDialogService _dialogService;
        private readonly IBackgroundTaskService _backgroundTaskService;

        private ObservableCollection<VirtualControllerViewModel> _controllers;
        private ProjectModel _projmodel;
        private bool _isModified;
        private bool _isSelected;

        #endregion

        #region Properties

        /// <summary>
        /// model of project
        /// </summary>
        public ProjectModel ProjModel
        {
            get { return _projmodel; }
            set
            {
                if (_projmodel == value)
                {
                    return;
                }
                _projmodel = value;
                RaisePropertyChanged(nameof(ProjModel));
            }
        }

        /// <summary>
        /// collection for controller viewmodels
        /// </summary>
        public ObservableCollection<VirtualControllerViewModel> Controllers
        {
            get { return _controllers; }
            set
            {
                if (_controllers == value)
                {
                    return;
                }
                _controllers = value;
                RaisePropertyChanged(nameof(Controllers));

            }
        }

        /// <summary>
        /// indicates if something modified
        /// </summary>
        public bool IsModified
        {
            get { return _isModified; }
            set
            {
                var oldValue = _isModified;
                _isModified = value;

                //raise property changed event and publish new value to all subscriber for Save routine
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


        #endregion

        #region Ctor

        /// <summary>
        /// construct viewmodel for the project
        /// </summary>
        /// <param name="model">model of project</param>
        /// <param name="dialogService">Dialog Service</param>
        /// <param name="backgroundTaskService">background Task Service</param>
        public ProjectViewModel(ProjectModel model, IDialogService dialogService, IBackgroundTaskService backgroundTaskService)
        {
            ProjModel = model;
            _dialogService = dialogService;
            _backgroundTaskService = backgroundTaskService;

            Controllers = new ObservableCollection<VirtualControllerViewModel>();
            Controllers.CollectionChanged += Controllers_CollectionChanged;

            foreach (var vcModel in ProjModel.VirtualControllerModels)
            {
                Controllers.Add(new VirtualControllerViewModel(vcModel, _dialogService, _backgroundTaskService));
            }

        }

        #endregion

        #region Commands

        /// <summary>
        /// add new controller 
        /// </summary>
        public ICommand AddNewControllerCommand => new RelayCommand(OnAddController, CanAddController);

        /// <summary>
        /// show properties of the project
        /// </summary>
        public ICommand ShowProjectPropertiesCommand => new RelayCommand(OnShowProperties, CanShowProperties);

        /// <summary>
        /// delete selected controller
        /// </summary>
        public ICommand DeleteControllerCommand => new RelayCommand<string>(OnDeleteController);


        #endregion

        #region Private Methods

        private bool CanAddController()
        {
            return true;
        }

        private void OnAddController()
        {
            var dialogViewModel = new AddNewPlcDialogViewModel(ProjModel.ProjectPath, _dialogService);

            bool? success = _dialogService.ShowCustomDialog<AddNewCustomPlcDialog>(this, dialogViewModel);
            if (success == true)
            {
                var item = Controllers.FirstOrDefault(x => x.Name == dialogViewModel.ControllerName);
                if (item == null)
                {
                    //create Path for Virtual SIMATIC Memory card
                    string controllerPath = Path.Combine(ProjModel.ProjectPath, dialogViewModel.ControllerName);

                    VirtualControllerModel model = ProjModel.AddNewController(dialogViewModel.ControllerName, dialogViewModel.ControllerType, controllerPath);

                    Controllers.Add(new VirtualControllerViewModel(model, _dialogService, _backgroundTaskService));

                }

                else
                {
                    _dialogService.ShowMessageBox(this, "PLC with same name already exists", "Error");

                }


            }
        }

        private void OnDeleteController(string name)
        {
            var result = MessageBox.Show(
               "Do you really want to delete the Controller?",
               "Delete Controller",
               MessageBoxButton.YesNoCancel,
               MessageBoxImage.Warning,
               MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                if (!String.IsNullOrEmpty(name))
                {
                    var controllerToRemove = Controllers.Where(x => x.Name == name);

                    foreach (var controller in controllerToRemove)
                    {
                        controller.Close();
                        Controllers.Remove(controller);
                        ProjModel.VirtualControllerModels.Remove(controller.VcModel);
                        break;
                    }
                }
            }
        }

        private bool CanShowProperties()
        {
            if (ProjModel != null)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private void OnShowProperties()
        {          

            var dialogViewModel = new ProjectPropertiesDialogViewModel(ProjModel,_dialogService);
            _dialogService.ShowDialog<ProjectPropertiesDialog>(this, dialogViewModel);            

        }

        #endregion

        #region Public Methods  

        /// <summary>
        /// close routine for Controller
        /// </summary>
        public void Close()
        {
            foreach (var controller in Controllers)
            {
                controller.Close();
            }
            Controllers.Clear();
        }

        #endregion

        #region Events

        private void Controllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    //Add listener for each item on PropertyChanged event
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                        case NotifyCollectionChangedAction.Reset:
                            IsModified = true;
                            break;
                        default:
                            IsModified = false;
                            break;
                    }

                }
            }

            if (e.OldItems != null)
            {
                foreach (var olditem in e.OldItems)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Remove:
                        case NotifyCollectionChangedAction.Replace:
                        case NotifyCollectionChangedAction.Move:
                        case NotifyCollectionChangedAction.Reset:
                            IsModified = true;
                            break;
                        default:
                            IsModified = false;
                            break;
                    }
                }
            }

        }


        #endregion


    }
}
