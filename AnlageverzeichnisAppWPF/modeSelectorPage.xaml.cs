using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        public ICommand NewCommand => new RelayCommand(newFunction);
        public ICommand OpenCommand => new RelayCommand(openFunction);
        public ICommand ExistingCommand => new RelayCommand(newFromExistingFunction);

        public void registerHotkeys()
        {
            // assuming the mainwindow view model is in this.tag and has already been assigned 
            if (this.Tag is MainWindowViewModel mwvw)
            {
                mwvw.ActivePageNewCommand = NewCommand;
                mwvw.ActivePageOpenCommand = OpenCommand;
                mwvw.ActivePageExistingCommand = ExistingCommand;
            }
        }
        public void unregisterHotkeys()
        {
            // assuming the mainwindow view model is in this.tag and has already been assigned 
            if (this.Tag is MainWindowViewModel mwvw)
            {
                mwvw.ActivePageNewCommand = null;
                mwvw.ActivePageOpenCommand = null;
                mwvw.ActivePageExistingCommand = null;
            }
        }
        private void newFunction()
        {
            var generalInformationPage = new generalInformationInputPage();
            generalInformationPage.Tag = this.Tag;
            // before leaving the page remove this page's hotkeys to avoid unforseable behavior
            if (this.Tag is MainWindowViewModel mwvw)
            {
                mwvw.ActivePageNewCommand = null;
                mwvw.ActivePageOpenCommand = null;
                mwvw.ActivePageExistingCommand = null;
            }
            NavigationService.Navigate(generalInformationPage);
        }

        public modeSelectorPage()
        {
            InitializeComponent();
            this.Loaded += (_, __) => registerHotkeys();
            this.Unloaded += (_, __) => unregisterHotkeys();
        }
        private void newButton_Click(object sender, RoutedEventArgs e) => newFunction();
        private void newFromPreviousButton_Click(object sender, RoutedEventArgs e) => newFromExistingFunction();
        private void newFromExistingFunction()
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
                    NavigationService.Navigate(inputAndDisplayPage);
                }
            }
            
        }

        private void loadButton_Click(object sender, RoutedEventArgs e) => openFunction();
        private void openFunction()
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
