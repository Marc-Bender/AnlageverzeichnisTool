using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnlageverzeichnisAppWPF
{
    public partial class inputAndDisplayPage : Page
    {
        private void registerHotkeys()
        {
            if (this.Tag is MainWindowViewModel mwvm)
            {
                mwvm.ActivePageSaveCommand = SaveCommand;
                mwvm.ActivePageReloadCommand = ReloadCommand;
                mwvm.ActivePageApplyCommand = ApplyCommand;
            }
        }

        private void unregisterHotkeys()
        {
            if (this.Tag is MainWindowViewModel mwvm)
            {
                mwvm.ActivePageSaveCommand = null;
                mwvm.ActivePageReloadCommand = null;
                mwvm.ActivePageApplyCommand = null;
            }

        }

        public ICommand SaveCommand => new RelayCommand(Save);
        public ICommand ReloadCommand => new RelayCommand(Reload);
        public ICommand ApplyCommand => new RelayCommand(Apply);
        private void Save()
        {
            using (var outfile = new StreamWriter(this.filename))
            {
                if (this.DataContext is inputAndDisplayPageViewModel vm)
                {
                    outfile.Write(JsonSerializer.Serialize<AnlageverzeichnisDocument>((AnlageverzeichnisDocument)(vm.Document)));
                }
            }

            MessageBox.Show("Gespeichert!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Reload()
        {
            using (var infile = new StreamReader(this.filename))
            {
                if (this.DataContext is inputAndDisplayPageViewModel vm)
                {
                    vm.Document = JsonSerializer.Deserialize<AnlageverzeichnisDocument>(infile.ReadToEnd());
                }
            }

            MessageBox.Show("Auf gespeicherten Zustand zurückgesetzt!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Apply()
        {
            if (this.DataContext is inputAndDisplayPageViewModel vm)
            {
                try
                {
                    vm.CurrentlyEditedLine.calculateDerivedFields(vm.Document.Header.CurrentlyWorkedOnYear);
                }
                catch (NoNullAllowedException)
                {
                    MessageBox.Show("Ein Wert ist Null der nicht Null sein darf!", "Null-Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                vm.Document.DataEntryLines.Add(vm.CurrentlyEditedLine);
                vm.CurrentlyEditedLine = new dataEntryLine(vm.Document.Header.CurrentlyWorkedOnYear);
                deprecationInYearsCheckBox.IsChecked = false;
                objectDescriptionTextBox.Focus();
            }
        }
    }
}
