using System.ComponentModel;
using System.Windows.Controls;

namespace BinGuiDev.Components.Dev;

public partial class Dev2 : UserControl
{
    private TimeSpan? _selectedTime = TimeSpan.Zero;

    public event PropertyChangedEventHandler? PropertyChanged;

    public TimeSpan? SelectedTime
    {
        get => _selectedTime;
        set
        {
            if (_selectedTime != value)
            {
                _selectedTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTime)));
            }
        }
    }

    public Dev2()
    {
        InitializeComponent();
        DataContext = this;
        
        for (int i = 0; i < 24; i++)
           HourComboBox.Items.Add(new ComboBoxItem { Content = $"{i:00}" });
        for (int i = 0; i < 60; i+=5)
            MinComboBox.Items.Add(new ComboBoxItem { Content = $"{i:00}" });

    }
}