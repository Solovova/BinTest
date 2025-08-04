using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Serilog;

namespace BinGuiDev.Components.ContDateTime;

public partial class UcDateTime : UserControl{
    private readonly ContextMenu _menu;
    public event EventHandler<DataChangedEventArgsLong>? DataChanged;
    public event EventHandler<DataChangedEventArgsBool>? EnableChanged;

    //DateTime
    private long _unixTime;

    private void ManualDateTimeChanged(){
        DateTime? selectedDate = DatePicker.SelectedDate;
        if (!selectedDate.HasValue)
            return;
        if (!TimeSpan.TryParseExact(TimePicker.Text, "hh\\:mm\\:ss",
                CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            return;
        DateTime combinedDateTime = selectedDate.Value.Date + timeSpan;
        _unixTime = new DateTimeOffset(combinedDateTime).ToUnixTimeSeconds() * 1000000;
        DataChanged?.Invoke(this, new DataChangedEventArgsLong(_unixTime));
    }

    private void UnixTimeChanged(){
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(_unixTime / 1000000).UtcDateTime;
        DatePicker.SelectedDate = dateTime.Date;
        TimePicker.Text = dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        DataChanged?.Invoke(this, new DataChangedEventArgsLong(_unixTime));
    }

    public void SetUnixTime(long unixTime){
        _unixTime = unixTime;
        UnixTimeChanged();
    }

    //Enabled
    public void SetEnabledField(bool value){
        var enabled = !ButtonLock.IsChecked ?? false;
        ButtonLock.IsChecked = !enabled;
        DatePicker.IsEnabled = enabled;
        TimePicker.IsEnabled = enabled;
        MainGrid.ContextMenu = enabled ? _menu : null;
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

        RoutedCommand startOfDate = new ();
        RoutedCommand endOfDate = new ();
        _menu = new ContextMenu();
        _menu.Items.Add(new MenuItem{ Header = "Start of date", Command = startOfDate });
        _menu.Items.Add(new MenuItem{ Header = "End of date", Command = endOfDate });
        //_menu.Items.Add(new MenuItem{ Header = "Start of hour", Command = null });
        //_menu.Items.Add(new MenuItem{ Header = "End of hour", Command = null });

        CommandBindings.Add(new CommandBinding(startOfDate, MenuItem_StartDate_OnClick));
        CommandBindings.Add(new CommandBinding(endOfDate, MenuItem_EndDate_OnClick));

        SetEnabledField(true);
        DatePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
    }

    private void MenuItem_StartDate_OnClick(object sender, RoutedEventArgs e){
        TimePicker.Text = "00:00:00";
    }

    private void MenuItem_EndDate_OnClick(object sender, RoutedEventArgs e){
        TimePicker.Text = "23:59:59";
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e){
        ManualDateTimeChanged();
    }

    private void DatePicker_SelectedDateChanged(object? sender, SelectionChangedEventArgs e){
        ManualDateTimeChanged();
    }
}