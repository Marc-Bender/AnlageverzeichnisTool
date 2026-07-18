using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public abstract class AbstractAnlageVerzeichnisDocument<THeader, TLineCollection, TLine> : ObservableObject where THeader : AbstractDocumentHeader where TLineCollection : Collection<TLine> where TLine : AbstractDataLine
    {
        public virtual THeader? Header { get; set; }
        public virtual TLineCollection? DataEntryLines { get; set; }
    }
}
