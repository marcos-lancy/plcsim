using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SimTableApplication.Models;
using SimTableApplication.PLCSIM_Advanced.Interfaces;
using SimTableApplication.PLCSIM_Advanced.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System;
using MvvmDialogs;
using System.Windows.Data;
using System.Linq;
using SimTableApplication.ViewModels.DialogViewModels;
using SimTableApplication.Views.Dialogs;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using SimTableApplication.PLCSIM_Advanced.Utils;
using System.Collections.Generic;
using System.Globalization;
using SimTableApplication.Core.Exceptions;
using System.Windows;

namespace SimTableApplication.ViewModels
{
    /// <summary>
    /// ViewModel for Simulation Table in UI
    /// </summary>
    public class SimTableViewModel : ViewModelBase
    {
        #region Fields

        private readonly IVirtualController _controller;
        private readonly IDialogService _dialogService;
        private readonly object _locker = new object();

        private SimTableModel _simTableModel;
        private string _simTableName;

        private ObservableCollection<SimTag> _allTags;
        private ObservableCollection<SimTag> _selectedTags;

        private SimTag _selectedSimTag;
        private bool _isModified;
        private bool _isSelected;


        #endregion

        #region Properties    

        /// <summary>
        /// model of simulation table
        /// </summary>
        public SimTableModel SimTableModel
        {
            get { return _simTableModel; }
            set
            {
                _simTableModel = value;
                RaisePropertyChanged(nameof(SimTableModel));
            }
        }

        /// <summary>
        /// collection view for all tags for UI presentation
        /// </summary>
        public ICollectionView AllTagsView
        {
            get;
            set;
        }

        /// <summary>
        /// collection for all tags
        /// </summary>
        public ObservableCollection<SimTag> AllTags
        {
            get { return _allTags; }
            set
            {
                _allTags = value;
                RaisePropertyChanged(nameof(AllTags));

            }
        }

        /// <summary>
        /// collection view for all simulation tags in gridview for UI presentation
        /// </summary>
        public ICollectionView SelectedTagFilters
        {
            get;
            set;
        }

        /// <summary>
        /// collection for all selected tags
        /// </summary>
        public ObservableCollection<SimTag> SelectedTags
        {
            get { return _selectedTags; }
            set
            {
                _selectedTags = value;
                RaisePropertyChanged(nameof(SelectedTags));

            }
        }

        /// <summary>
        /// selected tag within grid view
        /// </summary>
        public SimTag SelectedSimTag
        {
            get { return _selectedSimTag; }
            set
            {
                _selectedSimTag = value;
                RaisePropertyChanged(nameof(SelectedSimTag));
            }
        }

        /// <summary>
        /// indicates if something is modified
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
        /// Name of the Simulation Table
        /// </summary>
        public string SimTableName
        {
            get { return _simTableName; }
            set
            {
                _simTableName = value;
                IsModified = true;
                SimTableModel.Name = value;
                RaisePropertyChanged(nameof(SimTableName));
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
        /// construct viewmodel for the simulation table
        /// </summary>
        /// <param name="model">model of the simulation table</param>
        /// <param name="controller">parent controller</param>
        /// <param name="dialogService">dialog Service</param>
        public SimTableViewModel(SimTableModel model, IVirtualController controller, IDialogService dialogService)
        {
            SimTableModel = model;
            SimTableName = SimTableModel.Name;
            _controller = controller;
            _dialogService = dialogService;

            SelectedTags = new ObservableCollection<SimTag>();

            //get clone from controller allTag 
            var clonedList = controller.AllTags.Select(x => (SimTag)x.Clone()).ToList();
            AllTags = new ObservableCollection<SimTag>(clonedList);

            AllTagsView = CollectionViewSource.GetDefaultView(AllTags);
            AllTagsView.Filter = AllTagsFilter;                               

            controller.AllTags.CollectionChanged += ControllerAllTags_CollectionChanged;
            SelectedTags.CollectionChanged += SelectedTags_CollectionChanged;
            _controller.OnEndOfCycle += OnEndOfCycle;

            lock (_locker)
            {
                foreach (var tag in SimTableModel.SimTags)
                {
                    SelectedTags.Add(tag);
                }
            }
        }        

        #endregion

        #region Commands

        /// <summary>
        /// add new simulation tag to collection
        /// </summary>        
        public ICommand AddCommand => new RelayCommand<KeyEventArgs>(OnAdd,CanAdd);

        /// <summary>
        /// add new simulation tag to collection with button
        /// </summary>
        public ICommand AddButtonCommand => new RelayCommand(OnAddButton, CanAddButton);
        
        /// <summary>
        /// write values of simulation tags to controller
        /// </summary>
        public ICommand WriteCommand => new RelayCommand(OnWrite, CanWrite);

        /// <summary>
        /// delete selected simulation tag 
        /// </summary>
        public ICommand DeleteSimTagCommand => new RelayCommand<IEnumerable<object>>(OnDeleteSimTag, CanDeleteSimtag);

        /// <summary>
        /// loads all tags into the gridview
        /// </summary>
        public ICommand LoadAllTagsCommand => new RelayCommand(OnLoadAllTagsCommand, CanLoadAllTagsCommand);

        /// <summary>
        /// export all selected tags into a csv file
        /// </summary>
        public ICommand ExportCsvCommand => new RelayCommand(OnExportCsvCommand, CanExportCsvCommand);

        /// <summary>
        /// import csv file and add to simulation table
        /// </summary>
        public ICommand ImportCsvCommand => new RelayCommand(OnImportCsvCommand, CanImportCsvCommand);

        /// <summary>
        /// rename simulation table
        /// </summary>
        public ICommand RenameCommand => new RelayCommand(OnRenameCommand, CanRenameCommand);

        
        #endregion

        #region private Methods        

        private bool CanAdd(KeyEventArgs arg)
        {
            if (_selectedSimTag != null)
            {
                return true;
            }
            else
                return false;
        }       

        private void OnAdd(KeyEventArgs e)
        {
            lock (_locker)
            {
                if ((e.Key == Key.Enter) || (e.Key == Key.Return))
                {
                    //add to Observable collection for UI 
                    SelectedTags.Add(SelectedSimTag);

                    //add to model to refresh the model base
                    SimTableModel.SimTags.Add(SelectedSimTag);

                    //refresh UI Filter
                    AllTagsView.Refresh();
                }              
            }
        }

        private bool CanAddButton()
        {
            if (_selectedSimTag != null)
            {
                return true;
            }
            else
                return false;
        }

        private void OnAddButton()
        {
            lock (_locker)
            {
                if (SelectedSimTag != null)
                {
                    if (!SelectedTags.Contains(SelectedSimTag))
                    {
                        //add to Observable collection for UI 
                        SelectedTags.Add(SelectedSimTag);

                        //add to model to refresh the model base
                        SimTableModel.SimTags.Add(SelectedSimTag);

                        //refresh UI Filter
                        AllTagsView.Refresh();

                    }

                }
            }
        }

        private bool CanWrite()
        {           

            if (_controller.IsActiv && SelectedTags.Count(x => x.IsValid) != 0)
            {
                return true;
            }
            else
                return false;
        }

        private void OnWrite()
        {
            try
            {
                _controller.WriteTags(SelectedTags.Where(x => x.IsSelected));
            }
            catch (VirtualControllerException vcException)
            {
                _dialogService.ShowMessageBox(this, vcException.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            catch (FormatException formatEx)
            {
                _dialogService.ShowMessageBox(this, formatEx.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }

            
        }

        private bool CanDeleteSimtag(IEnumerable<object> selectedTags)
        {
            return true;
        }

        private void OnDeleteSimTag(IEnumerable<object> selectedTags)
        {
            if (selectedTags != null)
            {
                lock (_locker)
                {
                    var list = selectedTags.ToList();

                    foreach (var item in list)
                    {
                        var tmp = (SimTag)item;

                        //remove from Observable collection for UI 
                        SelectedTags.Remove(tmp);

                        //remove from model to refresh the model base
                        SimTableModel.SimTags.Remove(tmp);

                        //refresh UI Filter
                        AllTagsView.Refresh();
                    }

                }
            }
        }

        private bool CanLoadAllTagsCommand()
        {

            return _controller.IsActiv && _controller.AllTags.Count!=0;

        }

        private void OnLoadAllTagsCommand()
        {
            var dialogViewModel = new LoadAllProjectTagsFilterDialogViewModel();

            bool? success = _dialogService.ShowCustomDialog<LoadAllProjectTagsCustomFilterDialog>(this, dialogViewModel);
            if (success == true)
            {
                foreach (var item in AllTags)
                {
                    switch (item.Area)
                    {
                        case "Input":
                            if (dialogViewModel.IsInputsSelected)
                            {
                                if (!SelectedTags.Contains(item))
                                {
                                    SelectedTags.Add(item);
                                    SimTableModel.SimTags.Add(item);
                                }                                
                            }
                            break;

                        case "Output":
                            if (dialogViewModel.IsOutputsSelected)
                            {
                                if (!SelectedTags.Contains(item))
                                {
                                    SelectedTags.Add(item);
                                    SimTableModel.SimTags.Add(item);
                                }
                            }
                            break;

                        case "DataBlock":
                            if (dialogViewModel.IsDbSelected)
                            {
                                if (!SelectedTags.Contains(item))
                                {
                                    SelectedTags.Add(item);
                                    SimTableModel.SimTags.Add(item);
                                }
                            }
                            break;
                        case "Marker":
                            if (dialogViewModel.IsMemorySelected)
                            {
                                if (!SelectedTags.Contains(item))
                                {
                                    SelectedTags.Add(item);
                                    SimTableModel.SimTags.Add(item);
                                }
                            }
                            break;


                        default:
                            break;
                    }
                }

                //refresh UI Filter
                AllTagsView.Refresh();
            }
        }

        private bool CanExportCsvCommand()
        {
            return (SelectedTags.Count != 0);
        }

        private void OnExportCsvCommand()
        {
            var settings = new SaveFileDialogSettings
            {
                Title = "Export as CSV File...",                
                Filter = "csv file (*.csv)|*.csv",
                CheckFileExists = false,
                AddExtension = true,
                DefaultExt = ".csv"
            };

            bool? success = _dialogService.ShowSaveFileDialog(this, settings);
            if (success == true)
            {
                string filePath = settings.FileName;               

                //get all Tags from selectedTags collection
                var output = string.Join("\n", SelectedTags.Select(x => string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3}", x.TagName, x.DataType, x.Area, x.ModifyValue)));

                //write data to file
                try
                {
                    File.WriteAllText(filePath, output);
                }
                catch (IOException ioEx)
                {

                    _dialogService.ShowMessageBox(this, ioEx.Message, "IOExeption", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                
            }
        }

        private bool CanImportCsvCommand()
        {
            return true;
        }

        private void OnImportCsvCommand()
        {
            OpenFileDialogSettings openFileSettings = new OpenFileDialogSettings
            {
                Title = "Import CSV file",
                Filter = "CSV file (*.csv)|*.csv"

            };
            //show open project dialog
            bool? success = _dialogService.ShowOpenFileDialog(this, openFileSettings);
            if (success == true)
            {
                string path = openFileSettings.FileName;

                lock (_locker)
                {

                    using (TextFieldParser parser = new TextFieldParser(path))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(";");
                        while (!parser.EndOfData)
                        {
                            SimTag importedTag = new SimTag();
                            //read fields of a row 
                            string[] fields = parser.ReadFields();

                            if (fields == null || fields.Length > 5)
                            {
                                _dialogService.ShowMessageBox(this, "CSV file is not in the correct format");
                            }
                            else
                            {

                                //create simulation tag object from csv data
                                try
                                {
                                    importedTag.TagName = fields[0];
                                    importedTag.DataType = (SimDataType) Enum.Parse(typeof(SimDataType), fields[1]);
                                    importedTag.Area = fields[2];                                    
                                    importedTag.ModifyValue = fields[3];
                                }
                                catch (Exception ex)
                                {
                                    _dialogService.ShowMessageBox(this, ex.Message);
                                }

                                if (!SelectedTags.Contains(importedTag))
                                {
                                    // check if AllTags contains imported tag set IsValid
                                    //-----
                                    if (AllTags.Contains(importedTag))
                                    {
                                        importedTag.IsValid = true;
                                    }

                                    //add importedTag
                                    SelectedTags.Add(importedTag);
                                    SimTableModel.SimTags.Add(importedTag);

                                    AllTagsView.Refresh();

                                }
                            }

                        }
                    }


                }
            }

                
        }

        private bool CanRenameCommand()
        {
            return true;
        }

        private void OnRenameCommand()
        {
            var dialogViewModel = new RenameDialogViewModel();

            bool? success = _dialogService.ShowDialog<RenameDialog>(this, dialogViewModel);
            if (success == true)
            {
                SimTableName = dialogViewModel.NewName;
            }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// close routine for closing simulation table
        /// </summary>
        public void Close()
        {
            _controller.OnEndOfCycle -= OnEndOfCycle;
            lock (_locker)
            {
                SelectedTags.Clear();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// read all tags at the end of each controller cycle
        /// </summary>
        /// <param name="sender">controller</param>
        /// <param name="e">event arguments</param>
        private void OnEndOfCycle(object sender, EventArgs e)
        {
            lock (_locker)
            {
                if (SelectedTags.Count != 0)
                {

                    //read all tags present in the table
                    if (_controller.IsUpToDate)
                    {
                        _controller.ReadTags(SelectedTags);
                    }
                }
            }
        }
        private void SelectedTags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
        private void ControllerAllTags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SimTag newSimTag = new SimTag();

            if (e.NewItems != null)
            {
                // assign newItem as SimTag -> e.NewItems has only one entry each time therefore the "foreach"
                foreach (var newItem in e.NewItems)
                {
                    newSimTag = newItem as SimTag;
                }
            }

            if (newSimTag == null) return;

            lock (_locker)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        //add new simulation tag to collection
                        AllTags.Add(newSimTag);

                        //set IsValid for each simulation tag
                        foreach (var item in SelectedTags)
                        {
                            if (item.TagName.Equals(newSimTag.TagName) && item.DataType.Equals(newSimTag.DataType))
                            {
                                item.IsValid = true;
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                       
                        //Clear collection which includes all controller tags
                        AllTags.Clear();

                        //set isValid for each selected tag to false
                        foreach (var item in SelectedTags)
                        {
                            item.IsValid = false;
                        }
                        break;
                    default:
                        break;
                }
            }



        }


        #endregion

        #region Filter

        /// <summary>
        /// Filter implementation for the AllTags Collection View
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool AllTagsFilter(object obj)
        {
            lock (_locker)
            {
                var simTag = obj as SimTag;
                if (simTag == null) return false;

                //check if simulation tag is already in selected Tags Collection
                if (SelectedTags.Contains(simTag))
                {
                    //Yes don´t display this tag
                    return false;
                }

                //display the tag
                return true;
            }
        }

        #endregion

    }
}
