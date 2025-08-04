using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace BinGuiDev.Components.ContDateTime;

public partial class UcDateTime : UserControl{
    //DateTime
    private long _unixTime;

    public void ManualDateTimeChanged(){
    }

    private void UnixTimeChanged(){
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(_unixTime / 1000000).UtcDateTime;
        DatePicker.SelectedDate = dateTime.Date;
        TimePicker.Text = dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public void SetUnixTime(long unixTime){
        _unixTime = unixTime;
        UnixTimeChanged();
    }

    //Enabled
    public void SetEnabledField(bool value){
        ButtonLock.IsChecked = !value;
        DatePicker.IsEnabled = !ButtonLock.IsChecked ?? false;
        TimePicker.IsEnabled = !ButtonLock.IsChecked ?? false;
    }

    private void ButtonEnable_OnClick(object sender, RoutedEventArgs e){
        if (!ButtonLock.IsChecked ?? false){
            ButtonLock.IsChecked = true;
            return;
        }

        SetEnabledField(!ButtonLock.IsChecked ?? false);
    }
    //-----

    public UcDateTime(){
        InitializeComponent();
    }
}