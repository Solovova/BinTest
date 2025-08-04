using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Serilog;

namespace BinGuiDev.Components.ContDateTime;

public partial class UcSymbolPeriod : UserControl{
    private string _period = string.Empty;
    private string _symbol = string.Empty;
    private string _pendingSymbol = string.Empty;

    private bool _suppressTextChanged;
    public SymbolCollection SymbolCollection{ get; } = new();

    public event EventHandler<DataChangedEventArgsString>? PeriodChanged;
    public event EventHandler<DataChangedEventArgsString>? SymbolChanged;

    public UcSymbolPeriod(){
        
        InitializeComponent();
        DataContext = this;
        Loaded += UcSymbolPeriod_Loaded;
    }

    private void UcSymbolPeriod_Loaded(object sender, RoutedEventArgs e){
        if (!string.IsNullOrEmpty(_pendingSymbol)){
            SetSymbol(_pendingSymbol);
            _pendingSymbol = string.Empty;
        }
    }

    public void SetPeriod(string period){
        _suppressTextChanged = true;
        _period = period;
        foreach (ToggleButton button in ButtonGroup.Children.OfType<ToggleButton>()){
            button.IsChecked = (_period == (string)button.Content);
        }

        _suppressTextChanged = false;
    }

    public void SetSymbol(string symbol){
        _suppressTextChanged = true;
        if (ComboBoxSymbol.Items.Count == 0){
            _pendingSymbol = symbol;
            return;
        }
        _symbol = symbol;
        foreach (string item in ComboBoxSymbol.Items){
            if (item == symbol){
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
        if (!ComboBoxSymbol.IsDropDownOpen) return;

        if (_suppressTextChanged) return;
        _symbol = ComboBoxSymbol.SelectedItem?.ToString() ?? string.Empty;
        SymbolChanged?.Invoke(this, new DataChangedEventArgsString(_symbol));

        ComboBoxSymbol.IsDropDownOpen = false;
        DependencyObject? scope = FocusManager.GetFocusScope(ComboBoxSymbol);
        FocusManager.SetFocusedElement(scope, null);
        Keyboard.ClearFocus();
    }

    private void ComboBoxSymbol_KeyUp(object sender, KeyEventArgs e){
        if (sender is not ComboBox comboBox) return;
        string filterText = comboBox.Text;
        SymbolCollection.ApplyFilter(filterText);
    }

    private void ComboBoxSymbol_OnGotFocus(object sender, RoutedEventArgs e){
        _suppressTextChanged = true;
        ComboBoxSymbol.SelectedItem = null;
        ComboBoxSymbol.Text = string.Empty;
        SymbolCollection.ApplyFilter(string.Empty);
        ComboBoxSymbol.IsDropDownOpen = true;
        _suppressTextChanged = false;
    }

    private void ComboBoxSymbol_OnLostFocus(object sender, RoutedEventArgs e){
        string newSymbol = ComboBoxSymbol.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(newSymbol)){
            SetSymbol(_symbol);
        }
    }
}