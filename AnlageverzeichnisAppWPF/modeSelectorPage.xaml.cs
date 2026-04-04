using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "*.json | JSON-Dateien";
            if (
                    (dialog.ShowDialog() == true)
                  &&(dialog.FileName != "")
               )
            {
                using (var outfile = new StreamReader(dialog.FileName))
                {
                    var document = JsonSerializer.Deserialize<AnlageverzeichnisDocument>(outfile.ReadToEnd());
                    var inputAndDisplayPage = new inputAndDisplayPage();
                    if (document is not null)
                    {
                        document.migrateToNextYear();
                    }
                    else
                    {
                        MessageBox.Show("Interner Fehler: parsen von alter Datei lieferte NULL", "Interner Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    inputAndDisplayPage.Tag = document;
                }
            }
            
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
