using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BinGuiDev.Components.ContDateTime;

public partial class UcSymbolPeriod : UserControl{
    private string _period;
    private string _symbol;
    private bool _suppressTextChanged = false;
    public event EventHandler<DataChangedEventArgsString>? PeriodChanged;
    public event EventHandler<DataChangedEventArgsString>? SymbolChanged;
    
    public UcSymbolPeriod(){
        InitializeComponent();
        foreach (var symbol in DevContext.Symbols){
            ComboBoxSymbol.Items.Add(new ComboBoxItem{ Content = $"{symbol}" });    
        }
    }

    public void SetPeriod(string period){
        _suppressTextChanged = true;
        _period= period;
        foreach (ToggleButton button in ButtonGroup.Children.OfType<ToggleButton>()){
            button.IsChecked = (_period == (string)button.Content);
        }
        _suppressTextChanged = false;
    }
    
    public void SetSymbol(string symbol){
        _suppressTextChanged = true;
        _symbol= symbol;
        foreach (ComboBoxItem item in ComboBoxSymbol.Items){
            if ((string)item.Content == symbol){
                ComboBoxSymbol.SelectedItem = item;
                break;
            }
        }
        _suppressTextChanged = false;
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e){
        if (_suppressTextChanged) return;
        if (sender is not ToggleButton clickedButton) return;

        if (clickedButton.IsChecked == false){
            clickedButton.IsChecked = true;
            return;
        }

        ButtonGroup.Tag = "Updating";
        foreach (ToggleButton button in ButtonGroup.Children.OfType<ToggleButton>()){
            if (button != clickedButton)
                button.IsChecked = false;
            else
                _period = (string)button.Content;
        }
        ButtonGroup.Tag = null;
        PeriodChanged?.Invoke(this, new DataChangedEventArgsString(_period));
    }

    private void ComboBoxSymbol_OnSelectionChanged(object sender, SelectionChangedEventArgs e){
        if (_suppressTextChanged) return;
        _symbol = ComboBoxSymbol.Text;
        SymbolChanged?.Invoke(this, new DataChangedEventArgsString(_symbol));
    }
}