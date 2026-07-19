using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public partial class inputAndDisplayPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private UIDocument document = new();
        [ObservableProperty]
        private UIDataLine currentlyEditedLine;
        [ObservableProperty]
        private bool isExpertModeEnabled;

        public inputAndDisplayPageViewModel()
        {
            this.CurrentlyEditedLine = new UIDataLine(Document.Header.CurrentlyWorkedOnYear);
        }

        public inputAndDisplayPageViewModel(UIDocument document)
        {
            this.Document = document;
            this.CurrentlyEditedLine = new UIDataLine(Document.Header.CurrentlyWorkedOnYear);
        }
    }
}
