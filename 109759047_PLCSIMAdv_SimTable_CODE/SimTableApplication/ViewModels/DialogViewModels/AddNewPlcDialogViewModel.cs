using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using SimTableApplication.PLCSIM_Advanced.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace SimTableApplication.ViewModels.DialogViewModels
{
    public class AddNewPlcDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        #region Fields

        public event EventHandler Closed;
        private readonly IDialogService _dialogService;
        private readonly string _projectPath;

        #endregion

        #region Properties        

        private bool? _dialogResult;
        bool? IModalDialogViewModel.DialogResult
        {
            get
            {
                return _dialogResult;
            }
        }

        private ObservableCollection<string> _availablePlcInstances;
        public ObservableCollection<string> AvailablePlcInstances
        {
            get { return _availablePlcInstances; }
            set
            {
                _availablePlcInstances = value;
                RaisePropertyChanged(nameof(AvailablePlcInstances));
            }
        }

        private string _controllerName;
        public string ControllerName
        {
            get { return _controllerName; }
            set
            {
                _controllerName = value;
                RaisePropertyChanged(nameof(ControllerName));
            }
        }

        private VirtualControllerType _controllerType;
        public VirtualControllerType ControllerType
        {
            get { return _controllerType; }
            set
            {
                _controllerType = value;
                RaisePropertyChanged(nameof(ControllerType));
            }
        }
        
        public IList<VirtualControllerType> ControllerTypes
        {
            get
            {
                return Enum.GetValues(typeof(VirtualControllerType)).Cast<VirtualControllerType>().ToList();
            }

        }

        #endregion

        #region Ctor

        public AddNewPlcDialogViewModel(string projectPath, IDialogService dialogService)
        {
            _projectPath = projectPath;
            _dialogService = dialogService;

            AvailablePlcInstances = new ObservableCollection<string>();

            UpdateExistingInstances();
        }


        #endregion

        #region Commands

        public ICommand AddNewPlcCommand => new RelayCommand(OnAddNewPlc, CanAddNewPlc);
        public ICommand CancelCommand => new RelayCommand(OnCancelCommand, CanCancel);

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private void OnAddNewPlc()
        {
            if (!string.IsNullOrEmpty(ControllerName))
            {
                _dialogResult = true;

                Close();
            }
        }

        private bool CanAddNewPlc()
        {
            if (String.IsNullOrEmpty(ControllerName))
                return false;
            else
                return true;
        }

        private bool CanCancel()
        {
            return true;
        }

        private void OnCancelCommand()
        {
            Reset();
            Close();
        }

        private void Reset()
        {
            ControllerName = String.Empty;
        }

        private void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateExistingInstances()
        {
            AvailablePlcInstances.Clear();

            DirectoryInfo info = new DirectoryInfo(_projectPath);

            foreach (DirectoryInfo dirInfo in info.GetDirectories())
            {
                try
                {
                    dirInfo.GetAccessControl();
                    if (dirInfo.GetDirectories("SIMATIC_MC").Length == 1)
                    {
                        AvailablePlcInstances.Add(dirInfo.Name);
                    }
                }
                catch (UnauthorizedAccessException)
                {

                }
            }
        }

        #endregion


    }
}
