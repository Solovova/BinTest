using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace BinGuiDev.Components.Dev;

public partial class Dev3 : UserControl
{
    private long _unixTime;

    public long UnixTime
    {
        get => _unixTime;
        set
        {
            if (_unixTime != value / 1000000)
            {
                _unixTime = value / 1000000;
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(_unixTime).UtcDateTime;
                DatePicker.SelectedDate = dateTime.Date;
                TimePicker.Text = dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            }
        }
    }

    private bool _enabledField;

    public bool EnabledField
    {
        get => _enabledField;
        set
        {
            if (_enabledField != value)
            {
                _enabledField = value;
                DatePicker.IsEnabled = _enabledField;
                TimePicker.IsEnabled = _enabledField;
                ButtonLock.IsChecked = !_enabledField;
            }
        }
    }

    public Dev3()
    {
        InitializeComponent();
        EnabledField = true;
    }


    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        EnabledField = !ButtonLock.IsChecked ?? false;
    }
}