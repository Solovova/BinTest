using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BinGuiDev.Components.ContDateTime;

public partial class UcSymbolPeriod : UserControl{
    public UcSymbolPeriod(){
        InitializeComponent();
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e){
        if (sender is not ToggleButton clickedButton) return;

        if (clickedButton.IsChecked == false){
            clickedButton.IsChecked = true;
            return;
        }

        buttonGroup.Tag = "Updating";
        foreach (var button in buttonGroup.Children.OfType<ToggleButton>()){
            if (button != clickedButton)
                button.IsChecked = false;
        }

        buttonGroup.Tag = null;
    }
}