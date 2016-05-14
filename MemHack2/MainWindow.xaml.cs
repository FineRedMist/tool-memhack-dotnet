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
using mkLibrary.ThemeSelector;
using Microsoft.Win32;
using ProcessTools;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MemHack2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SortState mProcessSortState = new SortState();

        public ObservableCollection<ProcessInformation> ActiveProcesses = new ObservableCollection<ProcessInformation>();
        
        public MainWindow()
        {
            InitializeComponent();

            var theme = GetThemeFromRegistry();
            foreach(var item in cmbThemes.Items)
            {
                var cmbItem = item as ComboBoxItem;
                if (cmbItem != null && (cmbItem.Tag as string) == theme)
                {
                    cmbThemes.SelectedItem = cmbItem;
                    break;
                }
            }
        }

        private void cmbThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cmbThemes.SelectedItem as ComboBoxItem;
            var theme = selectedItem != null
                ? selectedItem.Tag as string
                : null;
            var lastTheme = GetThemeFromRegistry();
            if (theme != lastTheme)
            {
                SetThemeInRegistry(theme);
            }
        }

        private RegistryKey GetRegistryKey()
        {
            return Registry.CurrentUser.CreateSubKey("Software\\OneOddSock\\MemHack2");
        }

        private string GetThemeFromRegistry()
        {
            using (var key = GetRegistryKey())
            {
                return key.GetValue("Theme", null) as string;
            }
        }

        private void SetThemeInRegistry(string themeTag)
        {
            using (var key = GetRegistryKey())
            {
                if (string.IsNullOrEmpty(themeTag))
                {
                    try
                    {
                        key.DeleteValue("Theme");
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    key.SetValue("Theme", themeTag);
                }
            }
        }

        private void RunningProcessesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            mProcessSortState.OnColumnHeader_Click(RunningProcesses, sender, e);
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RunningProcesses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
