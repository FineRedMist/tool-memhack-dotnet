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
using System.Windows.Shapes;
using ProcessTools;

namespace MemHack2
{
    /// <summary>
    /// Interaction logic for HackingWindow.xaml
    /// </summary>
    public partial class HackingWindow : Window
    {
        private ProcessQueryMemory Query { get { return DataContext as ProcessQueryMemory; } }

        public HackingWindow()
        {
            InitializeComponent();
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Query == null)
            {
                Title = "Hacking...";
            }
            else if (!string.IsNullOrEmpty(Query.ProcessInfo.DefaultFriendlyName))
            {
                Title = "Hacking '" + Query.ProcessInfo.Name + "': " + Query.ProcessInfo.DefaultFriendlyName;
            }
            else
            {
                Title = "Hacking '" + Query.ProcessInfo.Name + "'";
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
