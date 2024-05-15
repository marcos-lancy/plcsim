using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using System;
using System.Windows.Input;

namespace SimTableApplication.ViewModels.DialogViewModels
{
    public class RenameDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        #region Fields

        private bool? _dialogResult;
        public event EventHandler Closed;
        private string _newName;

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
        /// new name to be set
        /// </summary>
        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
                RaisePropertyChanged(nameof(NewName));
            }
        }


        #endregion

        #region Ctor

        public RenameDialogViewModel()
        {

        }

        #endregion

        #region Commands

        /// <summary>
        /// close dialog via OK Button
        /// </summary>
        public ICommand RenameCommand => new RelayCommand(OnRename, CanRename);

        /// <summary>
        /// cancel and close dialog
        /// </summary>
        public ICommand CancelCommand => new RelayCommand(OnCancelCommand, CanCancelProject);

        #endregion

        #region Private Methods

        private bool CanRename()
        {
            return !string.IsNullOrEmpty(NewName);
        }

        private void OnRename()
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
