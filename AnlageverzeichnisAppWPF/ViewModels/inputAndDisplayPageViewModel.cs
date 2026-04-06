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
        private AnlageverzeichnisDocument document = new();
        [ObservableProperty]
        private dataEntryLine currentlyEditedLine;
        [ObservableProperty]
        private bool isExpertModeEnabled;

        public inputAndDisplayPageViewModel()
        {
            this.CurrentlyEditedLine = new dataEntryLine(Document.Header.CurrentlyWorkedOnYear);
        }

        public inputAndDisplayPageViewModel(AnlageverzeichnisDocument document)
        {
            this.Document = document;
            this.CurrentlyEditedLine = new dataEntryLine(Document.Header.CurrentlyWorkedOnYear);
        }
    }
}
