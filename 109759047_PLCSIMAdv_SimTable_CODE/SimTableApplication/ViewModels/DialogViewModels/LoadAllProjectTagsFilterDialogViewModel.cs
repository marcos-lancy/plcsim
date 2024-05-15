using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using System;
using System.Windows.Input;

namespace SimTableApplication.ViewModels.DialogViewModels
{
    public class LoadAllProjectTagsFilterDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        #region Fields

        public event EventHandler Closed;
        private bool? _dialogResult;
        private bool _isInputsSelected;
        private bool _isOutputsSelected;
        private bool _isMemorySelected;
        private bool _isDbSelected;

        #endregion

        #region Properties


        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }
        }        
        public bool IsInputsSelected
        {
            get { return _isInputsSelected; }
            set
            {
                _isInputsSelected = value;
                RaisePropertyChanged(nameof(IsInputsSelected));
            }
        }       
        public bool IsOutputsSelected
        {
            get { return _isOutputsSelected; }
            set
            {
                _isOutputsSelected = value;
                RaisePropertyChanged(nameof(IsOutputsSelected));
            }
        }        
        public bool IsMemorySelected
        {
            get { return _isMemorySelected; }
            set
            {
                _isMemorySelected = value;
                RaisePropertyChanged(nameof(IsMemorySelected));
            }
        }        
        public bool IsDbSelected
        {
            get { return _isDbSelected; }
            set
            {
                _isDbSelected = value;
                RaisePropertyChanged(nameof(IsDbSelected));
            }
        }

        #endregion

        #region Ctor

        public LoadAllProjectTagsFilterDialogViewModel()
        {

        }

        #endregion

        #region Commands

        public ICommand CancelCommand => new RelayCommand(OnCancelCommand, CanCancel);
        public ICommand LoadCommand => new RelayCommand(OnLoadCommand, CanLoad);

        private bool CanLoad()
        {
            return true;
        }

        private void OnLoadCommand()
        {
            _dialogResult = true;
            Close();
        }

        #endregion

        #region Private Methods

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
            IsInputsSelected = false;
            IsOutputsSelected = false;
            IsMemorySelected = false;
            IsDbSelected = false;
        }

        private void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
