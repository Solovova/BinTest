using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace BinGuiDev.Components.ContDateTime;

public class SymbolCollection{
    public ObservableCollection<string> Items{ get; set; }
    public ICollectionView FilteredItems{ get; set; }

    public SymbolCollection(){
        Items = new ObservableCollection<string>(DevContext.Symbols);


        FilteredItems = CollectionViewSource.GetDefaultView(Items);
    }

    public void ApplyFilter(string filterText){
        if (FilteredItems == null) return;

        FilteredItems.Filter = item => {
            if (string.IsNullOrEmpty(filterText)) return true;
            return ((string)item).IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
        };

        FilteredItems.Refresh();
    }
    
    
}