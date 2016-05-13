using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ThemeSelector
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private bool returnFromEventHandler = false;
        private void cmbThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (returnFromEventHandler) return;
            Uri uri = null;
            if (cmbThemes.SelectedIndex > 0)
            {
                uri = new Uri((cmbThemes.SelectedItem as ComboBoxItem).Tag.ToString(), UriKind.Relative);
            }

            returnFromEventHandler = true;
            MkThemeSelector.SetCurrentThemeDictionary(rootGrid, uri);
            returnFromEventHandler = false;
        }

        private void ChangeToRedTheme()
        {
            MkThemeSelector.SetCurrentThemeDictionary(this, new Uri("/ThemeSelector;component/Themes/ShinyRed.xaml", UriKind.Relative));
        }
    }
}
