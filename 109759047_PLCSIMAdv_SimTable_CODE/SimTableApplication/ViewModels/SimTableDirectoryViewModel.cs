using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using SimTableApplication.Models;
using SimTableApplication.PLCSIM_Advanced.Interfaces;
using SimTableApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SimTableApplication.ViewModels
{
    public class SimTableDirectoryViewModel :ViewModelBase
    {
        #region Fields

        private readonly IDialogService _dialogService;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly IVirtualController _controller;

        private SimTableDirectoryModel _simTableDirModel;
        private ObservableCollection<SimTableViewModel> _simTables;
        private string _name;
        private bool _isSelected;
        private bool _isModified;


        #endregion

        #region Properties

        /// <summary>
        /// Model of the SimTable Directory
        /// </summary>
        public SimTableDirectoryModel SimTableDirModel
        {
            get { return _simTableDirModel; }
            set
            {
                _simTableDirModel = value;
                RaisePropertyChanged(nameof(SimTableDirModel));
            }
        }


        /// <summary>
        /// Collection of Simulation tables
        /// </summary>
        public ObservableCollection<SimTableViewModel> SimTables
        {
            get { return _simTables; }
            set
            {
                _simTables = value;
                RaisePropertyChanged(nameof(SimTables));
            }
        }        

        /// <summary>
        /// UI representation TreeViewItem selected
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
        /// name of the SimTableDirectory
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


        #endregion

        #region Ctor

        public SimTableDirectoryViewModel(SimTableDirectoryModel model, IVirtualController controller, IDialogService dialogService, IBackgroundTaskService backgroundTaskService)
        {
            SimTableDirModel = model;
            _controller = controller;
            _dialogService = dialogService;
            _backgroundTaskService = backgroundTaskService;

            Name = SimTableDirModel.Name;

            SimTables = new ObservableCollection<SimTableViewModel>();
            SimTables.CollectionChanged += SimTables_CollectionChanged;
                        

            foreach (var simModel in SimTableDirModel.SimTableModels)
            {
                SimTables.Add(new SimTableViewModel(simModel, controller, _dialogService));
            }

            var test = SimTables.Where(x => x.SimTableName.Equals("Default SimTable")).ToList();
            if (test.Count == 0)
            {
                //add default SimTable
                SimTableModel defaultSimTableModel = SimTableDirModel.AddNewSimTable("Default SimTable");
                SimTables.Add(new SimTableViewModel(defaultSimTableModel, _controller, _dialogService));
            }
            

        }


        #endregion

        #region Commands

        /// <summary>
        /// adds new simulation table
        /// </summary>
        public ICommand AddNewSimTableCommand => new RelayCommand(OnAddNewSimTable, CanAddNewSimTable);

        /// <summary>
        /// deletes simulation table
        /// </summary>
        public ICommand DeleteSimTableCommand => new RelayCommand<string>(OnDeleteSimTable, CanDeleteSimTable);       

        #endregion

        #region private methods

        private bool CanAddNewSimTable()
        {
            return true;
        }
        private void OnAddNewSimTable()
        {
            SimTableModel model = SimTableDirModel.AddNewSimTable(String.Format(CultureInfo.InvariantCulture, "SimTable_{0}", SimTables.Count));
            SimTables.Add(new SimTableViewModel(model, _controller, _dialogService));
        }
        private bool CanDeleteSimTable(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                if (!name.Equals("Default SimTable"))
                {
                    return true;
                }
            }

            return false;
        }
        private void OnDeleteSimTable(string name)
        {
            var result = MessageBox.Show(
                "Do you really want to delete the Simulation table?",
                "Delete Simulation table",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                if (!String.IsNullOrEmpty(name))
                {
                    var simTablesToRemove = SimTables.Where(x => x.SimTableModel.Name == name);
                    foreach (var simTable in simTablesToRemove)
                    {
                        SimTables.Remove(simTable);
                        SimTableDirModel.SimTableModels.Remove(simTable.SimTableModel);
                        break;
                    }
                }
            }


        }

        #endregion

        #region public methods

        public void Close()
        {
            foreach (var simTableVm in SimTables)
            {
                simTableVm.Close();
            }


        }

        #endregion

        #region Events

        private void SimTables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
