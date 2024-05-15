using SimTableApplication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimTableApplication.Views.Controls
{
    /// <summary>
    /// Interaction logic for SimTable.xaml
    /// </summary>
    public sealed partial class SimTableView : UserControl
    {
        public SimTableView()
        {
            InitializeComponent();
        }

        private void ListView_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            // Get a reference to the ListView's GridView...
            var listView = sender as ListView;
            if (null != listView)
            {
                var gridView = listView.View as GridView;
                if (null != gridView)
                {
                    // ... and update its column widths
                    ListViewBehaviors.UpdateColumnWidths(gridView);
                }
            }
        }
    }
}
