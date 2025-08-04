using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BinGuiDev.Components.ContDateTime;

public partial class UcSymbolPeriod : UserControl{
    private string _period;
    private string _symbol;
    public event EventHandler<DataChangedEventArgsString>? PeriodChanged;
    public event EventHandler<DataChangedEventArgsString>? SymbolChanged;
    
    public UcSymbolPeriod(){
        InitializeComponent();
        foreach (var symbol in DevContext.Symbols){
            ComboBoxSymbol.Items.Add(new ComboBoxItem{ Content = $"{symbol}" });    
        }

        ComboBoxSymbol.SelectedIndex = 0;
    }

    public void SetPeriod(string period){
        _period= period;
    }
    
    public void SetSymbol(string symbol){
        _symbol= symbol;
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e){
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
        _symbol = ComboBoxSymbol.Text;//(ComboBoxSymbol.SelectedItem as string) ?? string.Empty;
        SymbolChanged?.Invoke(this, new DataChangedEventArgsString(_symbol));
    }
}