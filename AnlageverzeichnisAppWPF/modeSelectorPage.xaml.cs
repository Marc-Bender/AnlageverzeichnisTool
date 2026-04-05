using CommunityToolkit.Mvvm.ComponentModel;
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
            var generalInformationPage = new generalInformationInputPage();
            generalInformationPage.Tag = this.Tag;
            NavigationService.Navigate(generalInformationPage);
        }

        private void newFromPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON-Dateien | *.json";
            if (
                    (dialog.ShowDialog() == true)
                  &&(dialog.FileName != "")
               )
            {
                using (var outfile = new StreamReader(dialog.FileName))
                {
                    var document = JsonSerializer.Deserialize<AnlageverzeichnisDocument>(outfile.ReadToEnd());
                    if (document is not null)
                    {
                        document.migrateToNextYear();
                    }
                    else
                    {
                        MessageBox.Show("Interner Fehler: parsen von alter Datei lieferte NULL", "Interner Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var inputAndDisplayPage = new inputAndDisplayPage(document, dialog.FileName);
                    inputAndDisplayPage.Tag = this.Tag;
                    if(this.Tag is MainWindowViewModel mwvm)
                    {
                        mwvm.ActivePageSaveCommand = inputAndDisplayPage.SaveCommand;
                        mwvm.ActivePageReloadCommand = inputAndDisplayPage.ReloadCommand;
                        mwvm.ActivePageApplyCommand = inputAndDisplayPage.ApplyCommand;
                    }
                    NavigationService.Navigate(inputAndDisplayPage);
                }
            }
            
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON-Dateien | *.json";
            if (
                    (dialog.ShowDialog() == true)
                  && (dialog.FileName != "")
               )
            {
                using (var outfile = new StreamReader(dialog.FileName))
                {
                    var document = JsonSerializer.Deserialize<AnlageverzeichnisDocument>(outfile.ReadToEnd());
                    if (document is not null)
                    {
                        var inputAndDisplayPage = new inputAndDisplayPage(document, dialog.FileName);
                        inputAndDisplayPage.Tag = this.Tag;
                        if (this.Tag is MainWindowViewModel mwvm)
                        {
                            mwvm.ActivePageSaveCommand = inputAndDisplayPage.SaveCommand;
                            mwvm.ActivePageReloadCommand = inputAndDisplayPage.ReloadCommand;
                            mwvm.ActivePageApplyCommand = inputAndDisplayPage.ApplyCommand;
                        }
                        NavigationService.Navigate(inputAndDisplayPage);
                    }
                    else
                    {
                        MessageBox.Show("Interner Fehler: parsen von zu öffnender Datei lieferte NULL", "Interner Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

        }
    }
}
