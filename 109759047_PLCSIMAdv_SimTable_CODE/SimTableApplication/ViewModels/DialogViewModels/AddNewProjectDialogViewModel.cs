using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using System;
using System.Reflection;
using System.Windows.Input;

namespace SimTableApplication.ViewModels.DialogViewModels
{
    public class AddNewProjectDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        #region Fields

        public event EventHandler Closed;
        private readonly IDialogService _dialogService;


        #endregion

        #region Properties      

        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                _projectName = value;
                RaisePropertyChanged(nameof(ProjectName));
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(nameof(Path));
            }
        }

        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                RaisePropertyChanged(nameof(Version));
            }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                RaisePropertyChanged(nameof(Author));
            }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                RaisePropertyChanged(nameof(Comment));
            }
        }

        private bool? _dialogResult;
        bool? IModalDialogViewModel.DialogResult
        {
            get
            {
                return _dialogResult;
            }
        }





        #endregion

        #region Ctor
        public AddNewProjectDialogViewModel(IDialogService dialogService)
        {
            this._dialogService = dialogService;
        }

        #endregion

        #region Commands

        public ICommand CreateCommand => new RelayCommand(OnCreateCommand, CanCreateProject);
        public ICommand CancelCommand => new RelayCommand(OnCancelCommand, CanCancelProject);
        public ICommand OpenPathCommand => new RelayCommand(OnOpenPathCommand, CanOpenPath);


        #endregion

        #region private Methods

        private bool CanOpenPath()
        {
            if (!String.IsNullOrEmpty(ProjectName))
                return true;
            else
                return false;

        }

        private void OnOpenPathCommand()
        {
            var settings = new FolderBrowserDialogSettings
            {
                Description = "Select Path",
                SelectedPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            bool? success = _dialogService.ShowFolderBrowserDialog(this, settings);
            if (success == true)
            {
                Path = settings.SelectedPath;
            }
        }



        private bool CanCancelProject()
        {
            return true;
        }

        private void OnCancelCommand()
        {
            Reset();
            Close();
        }

        private void OnCreateCommand()
        {
            _dialogResult = true;

            Close();
        }

        private bool CanCreateProject()
        {
            if (string.IsNullOrEmpty(ProjectName) || string.IsNullOrEmpty(Path))
            {
                return false;
            }
            else
                return true;
        }

        private void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void Reset()
        {
            ProjectName = String.Empty;
            Path = String.Empty;
            Version = String.Empty;
            Author = String.Empty;
            Comment = String.Empty;
        }

        #endregion

        #region public Methods






        #endregion

    }
}

