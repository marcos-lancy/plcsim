using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SimTableApplication.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SimTableApplication.ViewModels.DialogViewModels;
using MvvmDialogs;
using System.IO;
using System;
using GalaSoft.MvvmLight.Messaging;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using SimTableApplication.Models;
using SimTableApplication.Core.Utils;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using SimTableApplication.Services.Interfaces;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Win32;

namespace SimTableApplication.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDialogService _dialogService;
        private readonly IBackgroundTaskService _backgroundTaskService;

        private ProjectModel _projModel;
        private ObservableCollection<ProjectViewModel> _projects;
        private object _selectedTreeViewItem;
        private bool _isModified;
        private bool _isPsaInstalled;
        private string _psaInstalledVersion;
        private bool _shutDown;

        #endregion

        #region Properties        

        /// <summary>
        /// model of the project
        /// </summary>
        public ProjectModel ProjModel
        {
            get { return _projModel; }
            set
            {
                _projModel = value;
                RaisePropertyChanged(nameof(ProjModel));
            }
        }

        /// <summary>
        /// collection for project viewmodel
        /// </summary>
        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                RaisePropertyChanged(nameof(Projects));
            }
        }

        /// <summary>
        /// selected treeview item in UI
        /// </summary>
        public object SelectedTreeViewItem
        {
            get { return _selectedTreeViewItem; }
            set
            {
                _selectedTreeViewItem = value;
                RaisePropertyChanged(nameof(SelectedTreeViewItem));
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
                if (_isModified == value)
                {
                    return;
                }
                _isModified = value;
                RaisePropertyChanged(nameof(IsModified));
            }
        }

        /// <summary>
        /// indicates if PLCSIM Advanced is installed or not
        /// </summary>
        public bool IsPsaInstalled
        {
            get { return _isPsaInstalled; }
            set
            {
                if (_isPsaInstalled == value)
                {
                    return;
                }
                _isPsaInstalled = value;
                RaisePropertyChanged(nameof(IsPsaInstalled));
            }
        }

        /// <summary>
        /// indicates the installed PLCSIM Advanced Version
        /// </summary>
        public string PsaInstalledVersion
        {
            get { return _psaInstalledVersion; }
            set
            {
                if (_psaInstalledVersion == value)
                {
                    return;
                }
                _psaInstalledVersion = value;
                RaisePropertyChanged(nameof(PsaInstalledVersion));

            }
        }
        
        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="dialogService">Dialog Service</param>
        /// <param name="backgroundTaskService">background Task Service</param>
        public MainViewModel(IDialogService dialogService, IBackgroundTaskService backgroundTaskService)
        {
            _dialogService = dialogService;
            _backgroundTaskService = backgroundTaskService;
            Projects = new ObservableCollection<ProjectViewModel>();

            //get IsModified property from all viewmodels
            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, (action) => DispatcherHelper.CheckBeginInvokeOnUI(() => IsModified = action.NewValue));

            //check if PLCSIM Adv. is installed or not
            CheckPlcSimAdvInstallation();
        }

        #endregion

        #region Commands

        /// <summary>
        /// add a new project
        /// </summary>
        public ICommand AddNewProjectCommand => new RelayCommand(OnAddProject, CanAddProject);

        /// <summary>
        /// open stored project
        /// </summary>
        public ICommand OpenProjectCommand => new RelayCommand(OnOpenProject, CanOpenProject);

        /// <summary>
        /// close current project
        /// </summary>
        public ICommand CloseProjectCommand => new RelayCommand(OnCloseProject, CanCloseProject);

        /// <summary>
        /// save current project
        /// </summary>
        public ICommand SaveProjectCommand => new RelayCommand(OnSaveProject, CanSaveProject);

        /// <summary>
        /// save current project as...
        /// </summary>
        public ICommand SaveAsProjectCommand => new RelayCommand(OnSaveAsProject, CanSaveAsProject);

        /// <summary>
        /// show information message about the application
        /// </summary>
        public ICommand ShowAboutCommand => new RelayCommand(OnShowAbout, CanShowAbout);

        /// <summary>
        /// close app via X Button
        /// </summary>
        public ICommand CloseAppCommand => new RelayCommand<CancelEventArgs>(OnCloseAppCommand);


        #endregion

        #region Private Methods      

        private bool CanOpenProject()
        {
            if (IsPsaInstalled)
            {
                return true;
            }
            else
                return false;


        }
        private void OnOpenProject()
        {
            if (!CanOpenProject())
            {
                return;
            }

            OpenFileDialogSettings openFileSettings = new OpenFileDialogSettings
            {
                Title = "Open Project",
                Filter = "Simulation project (*.sim)|*.sim"

            };
            //show open project dialog
            bool? success = _dialogService.ShowOpenFileDialog(this, openFileSettings);
            if (success == true)
            {
                //open new selected Project
                string path = openFileSettings.FileName;

                // A project is already open
                if (Projects.Count != 0)
                {
                    //Ask what to do if it was modified
                    if (IsModified)
                    {
                        //show project modified message box                   
                        var result = _dialogService.ShowMessageBox(this, "Do you want to save the changes to the current project?", "Project has been modified.", MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);

                        //Yes --> save project, close current project & open new project
                        if (result == MessageBoxResult.Yes)
                        {
                            OnSaveProject();
                            IsModified = false;
                            OnCloseProject();
                            _backgroundTaskService.PerformInBackground(DoOpenProject, DoOpenProjectFinished, path);
                        }
                        //No --> close project & open new project
                        if (result == MessageBoxResult.No)
                        {
                            //close Project
                            IsModified = false;
                            OnCloseProject();
                            _backgroundTaskService.PerformInBackground(DoOpenProject, DoOpenProjectFinished, path);

                        }
                        //cancel --> return/ cancel operation
                        if (result == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                    //if no modification was done close project and open new selected project
                    else
                    {
                        IsModified = false;
                        OnCloseProject();
                        _backgroundTaskService.PerformInBackground(DoOpenProject, DoOpenProjectFinished, path);
                    }
                }
                //no project is open
                else
                {
                    _backgroundTaskService.PerformInBackground(DoOpenProject, DoOpenProjectFinished, path);
                }

            }
        }
        private ProjectModel DoOpenProject(IBackgroundTask backgroundTask, object taskData)
        {
            string path = taskData as string;
            try
            {
                return Serializer.Deserialize<ProjectModel>(path);
            }
            catch (SerializationException ex)
            {
                _dialogService.ShowMessageBox(this, string.Format("Error occured during deserialization! Message: {0}", ex.Message), "Exception", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return null;
            }


        }
        private void DoOpenProjectFinished(IFinishedBackgroundTask<ProjectModel> result)
        {
            if (result.Exception != null)
                _dialogService.ShowMessageBox(this, result.Exception.Message, result.Exception.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);

            ProjModel = result.Result;
            Projects.Add(new ProjectViewModel(ProjModel, _dialogService, _backgroundTaskService));
            IsModified = false;
        }
        private void OnAddProject()
        {
            if (Projects.Count == 0)
            {
                var dialogViewModel = new AddNewProjectDialogViewModel(_dialogService);

                bool? success = _dialogService.ShowCustomDialog<AddNewCustomProjectDialog>(this, dialogViewModel);
                if (success == true)
                {
                    _projModel = new ProjectModel
                    {
                        ProjectName = dialogViewModel.ProjectName,
                        ProjectPath = Path.Combine(dialogViewModel.Path, dialogViewModel.ProjectName),
                        Version = dialogViewModel.Version,
                        Author = dialogViewModel.Author,
                        Comment = dialogViewModel.Comment
                    };

                    //Create Project directory if not existing
                    if (!Directory.Exists(_projModel.ProjectPath))
                    {
                        Directory.CreateDirectory(_projModel.ProjectPath);
                    }

                    Projects.Add(new ProjectViewModel(_projModel, _dialogService, _backgroundTaskService));
                }
            }

        }
        private bool CanAddProject()
        {
            if (Projects.Count == 0 && IsPsaInstalled)
                return true;
            else
                return false;
        }
        private void OnCloseProject()
        {
            if (IsModified)
            {
                var result = _dialogService.ShowMessageBox(this, "Do you want to save the changes to the current project?", "Project has been modified.", MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);

                if (result == MessageBoxResult.Yes)
                {
                    OnSaveProject();
                }
            }

            foreach (var item in Projects)
            {
                item.Close();
            }

            Projects.Clear();
        }
        private bool CanCloseProject()
        {
            if (Projects.Count > 0)
                return true;
            else
                return false;
        }
        private bool CanCloseAppCommand()
        {
            return true;
        }
        private void OnCloseAppCommand(CancelEventArgs args)
        {
            if (IsModified)
            {
                var result = _dialogService.ShowMessageBox(this, "Do you want to save the changes to the current project?", "Project has been modified.", MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:

                        //set _shutdown if prject shout be saved.
                        if (CanSaveProject())
                        {
                            _shutDown = true;
                            OnSaveProject();
                        }
                        else
                        {
                            Application.Current.Shutdown();
                        }            

                        break;
                    case MessageBoxResult.Cancel:
                        args.Cancel = true;
                        break;
                    case MessageBoxResult.None:
                    case MessageBoxResult.No:
                        Application.Current.Shutdown();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Application.Current.Shutdown();
            }



        }
        private bool CanSaveProject()
        {
            if (IsModified)
                return true;
            else
                return false;
        }
        private void OnSaveProject()
        {
            if (!CanSaveProject())
            {
                return;
            }

            _backgroundTaskService.PerformInBackground(DoSaveProject, DoSaveProjectFinished, ProjModel);
        }
        private bool DoSaveProject(IBackgroundTask backgroundTask, object taskData)
        {
            backgroundTask.ReportProgress(0);

            ProjectModel model = taskData as ProjectModel;

            if (model != null)
            {
                try
                {
                    Serializer.Serialize(model, Path.Combine(model.ProjectPath, String.Format(CultureInfo.InvariantCulture, "{0}{1}", model.ProjectName, ".sim")));
                    IsModified = false;
                }
                catch (SerializationException ex)
                {
                    _dialogService.ShowMessageBox(this, string.Format("Error occured during serialization! Message: {0}", ex.Message), "Exception", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }

            }

            backgroundTask.ReportProgress(100);
            return true;
        }
        private void DoSaveProjectFinished(IFinishedBackgroundTask<bool> result)
        {
            if (result.Exception != null)
                _dialogService.ShowMessageBox(this, result.Exception.Message, result.Exception.GetType().ToString(), MessageBoxButton.OK,
                    MessageBoxImage.Error);
            
            if (_shutDown)
            {
                Application.Current.Shutdown();
            }

        }
        private bool CanSaveAsProject()
        {
            if (Projects.Count != 0)
            {
                return true;
            }

            return false;
        }
        private void OnSaveAsProject()
        {
            var settings = new SaveFileDialogSettings
            {
                Title = "Save Project As...",
                InitialDirectory = _projModel.ProjectPath,
                Filter = "Simulation project (*.sim)|*.sim",
                CheckFileExists = false,
                AddExtension = true,
                DefaultExt = ".sim"
            };

            bool? success = _dialogService.ShowSaveFileDialog(this, settings);
            if (success == true)
            {
                string filePath = settings.FileName;

                string newDirName = Path.GetFileNameWithoutExtension(filePath);
                if (string.IsNullOrEmpty(newDirName)) return;

                string selectedDirectory = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(selectedDirectory)) return;

                string newPath = Path.Combine(selectedDirectory, newDirName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                //copy all Virtual Memorycards of Instances into new Directory
                Helper.Copy(ProjModel.ProjectPath, newPath);

                //change project name to the newly added name
                ProjModel.ProjectName = Path.GetFileNameWithoutExtension(filePath);

                //get new fullpath                
                string newFullPath = Path.Combine(newPath, Path.GetFileName(filePath));
                if (string.IsNullOrEmpty(newFullPath)) return;

                //save project to .sim file into new path
                Serializer.Serialize(ProjModel, newFullPath);

                //replace old path information with new path information
                string text = File.ReadAllText(newFullPath).Replace(ProjModel.ProjectPath, newPath);
                File.WriteAllText(newFullPath, text);

                //reset IsModified after save as
                IsModified = false;

                //close old project
                OnCloseProject();

                //open previous saved project
                ProjModel = Serializer.Deserialize<ProjectModel>(newFullPath);
                Projects.Add(new ProjectViewModel(ProjModel, _dialogService, _backgroundTaskService));

                IsModified = false;


            }
        }
        private bool CanShowAbout()
        {
            return true;
        }
        private void OnShowAbout()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine(Properties.Resources.ApplicationExample)
                .AppendLine()
                .AppendLine(string.Format(CultureInfo.InvariantCulture, "Version: {0}", Assembly.GetExecutingAssembly().GetName().Version))
                .AppendLine(string.Format(CultureInfo.InvariantCulture, "PLCSIM Advanced Version: {0}", PsaInstalledVersion))
                .AppendLine(Properties.Resources.Copyright);

            _dialogService.ShowMessageBox(this, stringBuilder.ToString(), "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        /// <summary>
        /// Check PLCSIM Advanced Installtion
        /// </summary>
        private void CheckPlcSimAdvInstallation()
        {
            IsPsaInstalled = true;

            // Check for 64bit installation           
            RegistryKey filePathReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Siemens\\Shared Tools\\PLCSIMADV_SimRT");
            if (filePathReg == null)
            {
                IsPsaInstalled = false;
                PsaInstalledVersion = "NOT INSTALLED";
                MessageBox.Show(string.Format("PLCSIM Advanced Installation missing! {0} Please install PLCSIM Advanced first.", Environment.NewLine), "PLCSIM Advanced Installation Check", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            else
            {
                PsaInstalledVersion = filePathReg.GetValue("Release").ToString();
            }

        }

        #endregion

        #region Hidden

        ///// <summary>
        ///// start selected controller 
        ///// </summary>
        //public ICommand StartControllerRouteCommand => new RelayCommand(OnStartControllerRouteCommand, CanStartControllerRoute);

        ///// <summary>
        ///// stop selected controller
        ///// </summary>
        //public ICommand StopControllerRouteCommand => new RelayCommand(OnStopControllerRouteCommand, CanStopControllerRoute);

        //private bool CanStartControllerRoute()
        //{
        //    if (!(SelectedTreeViewItem is VirtualControllerViewModel))
        //        return false;
        //    else
        //        return true;
        //}
        //private void OnStartControllerRouteCommand()
        //{
        //    MessengerInstance.Send<NotificationMessage>(new NotificationMessage("START"));
        //}
        //private bool CanStopControllerRoute()
        //{
        //    if (!(SelectedTreeViewItem is VirtualControllerViewModel))
        //        return false;
        //    else
        //        return true;
        //}
        //private void OnStopControllerRouteCommand()
        //{
        //    MessengerInstance.Send<NotificationMessage>(new NotificationMessage("STOP"));
        //}


        #endregion
    }
}