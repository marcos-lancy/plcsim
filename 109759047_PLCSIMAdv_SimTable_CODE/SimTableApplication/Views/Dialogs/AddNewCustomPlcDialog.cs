using MvvmDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimTableApplication.Views.Dialogs
{
    public sealed class AddNewCustomPlcDialog : IWindow
    {
        private readonly AddNewPlcDialog _dialog;


        public AddNewCustomPlcDialog()
        {
            _dialog = new AddNewPlcDialog();
        }

        public object DataContext
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

        public bool? DialogResult
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

        public ContentControl Owner
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

        public void Show()
        {
            _dialog.Show();
        }

        public bool? ShowDialog()
        {
            return _dialog.ShowDialog();
        }
    }
}
