using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AnlageverzeichnisAppWPF
{
    public class StoredDocument : 
        AbstractDocument<
            StoredHeader, Collection<StoredDataline>, StoredDataline>
    {
        // intended to be merely a type alias if you so will.. 
    }
}
