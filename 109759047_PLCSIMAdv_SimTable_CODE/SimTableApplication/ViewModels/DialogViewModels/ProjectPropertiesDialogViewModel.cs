using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using SimTableApplication.Models;
using System;
using System.Windows.Input;

namespace SimTableApplication.ViewModels.DialogViewModels
{
    class ProjectPropertiesDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        #region Fields
        private readonly IDialogService _dialogService;
        private bool? _dialogResult;
        private ProjectModel _model;
        public event EventHandler Closed;


        #endregion

        #region Properties

        /// <summary>
        /// result of the dialag
        /// </summary>
        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }

            set
            {
                if (_dialogResult == value)
                {
                    return;
                }
                _dialogResult = value;
                RaisePropertyChanged(nameof(DialogResult));
            }
        }
        
        /// <summary>
        /// Project model to be displayed in dialog
        /// </summary>
        public ProjectModel Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                {
                    return;
                }
                _model = value;
                RaisePropertyChanged(nameof(Model));
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Construct view model for Project peroperties dialog
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dialogService">dialogService</param>
        public ProjectPropertiesDialogViewModel(ProjectModel model, IDialogService dialogService)
        {
            Model = model;
            _dialogService = dialogService;
        }

        #endregion

        #region Commands

        /// <summary>
        /// close dialog via OK Button
        /// </summary>
        public ICommand OkCommand => new RelayCommand(OnOk, CanOk);

        /// <summary>
        /// cancel and close dialog
        /// </summary>
        public ICommand CancelCommand => new RelayCommand(OnCancelCommand, CanCancelProject);



        #endregion

        #region Private Methods

        private bool CanOk()
        {
            return true;
        }

        private void OnOk()
        {
            DialogResult = true;
            Close();
        }

        private bool CanCancelProject()
        {
            return true;
        }

        private void OnCancelCommand()
        {
           
            Close();
        }

        private void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        #endregion


    }
}
