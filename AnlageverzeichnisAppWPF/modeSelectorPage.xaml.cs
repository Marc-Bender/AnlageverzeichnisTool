using System;
using System.Collections.Generic;
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

namespace AnlageverzeichnisAppWPF
{
    /// <summary>
    /// Interaktionslogik für modeSelectorPage.xaml
    /// </summary>
    public partial class modeSelectorPage : Page
    {
        public modeSelectorPage()
        {
            InitializeComponent();
        }
        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new generalInformationInputPage());
        }

        private void newFromPreviousButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
