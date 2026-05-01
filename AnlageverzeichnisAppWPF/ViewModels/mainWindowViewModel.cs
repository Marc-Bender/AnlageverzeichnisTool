using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AnlageverzeichnisAppWPF
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ICommand? activePageSaveCommand;
        [ObservableProperty]
        private ICommand? activePageReloadCommand;
        [ObservableProperty]
        private ICommand? activePageApplyCommand;
        [ObservableProperty]
        private ICommand? activePageNewCommand;
        [ObservableProperty]
        private ICommand? activePageOpenCommand;
        [ObservableProperty]
        private ICommand? activePageExistingCommand;
        [ObservableProperty]
        private ICommand? activePagePDFCommand;
    }
}
