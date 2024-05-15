using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SimTableApplication.Core.Utils;

namespace SimTableApplication.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            Helper.AssemblyResolve();
            InitializeComponent();
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }
    }
    
}
