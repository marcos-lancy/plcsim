using MvvmDialogs;
using System.Windows;
using System.Windows.Controls;

namespace SimTableApplication.Views.Dialogs
{
    public sealed class AddNewCustomProjectDialog : IWindow
    {
        private readonly AddNewProjectDialog _dialog;

        public AddNewCustomProjectDialog()
        {
            _dialog = new AddNewProjectDialog();
        }
        object IWindow.DataContext
        {
            get
            {
                return _dialog.DataContext;
            }
            set
            {
                _dialog.DataContext = value;
            }

        }

        bool? IWindow.DialogResult
        {
            get
            {
                return _dialog.DialogResult;
            }

            set
            {
                _dialog.DialogResult = value;
            }
        }

        ContentControl IWindow.Owner
        {
            get
            {
                return _dialog.Owner;
            }

            set
            {
                _dialog.Owner = (Window)value;
            }
        }

        void IWindow.Show()
        {
            _dialog.Show();
        }

        bool? IWindow.ShowDialog()
        {
            return _dialog.ShowDialog();
        }
    }
}
