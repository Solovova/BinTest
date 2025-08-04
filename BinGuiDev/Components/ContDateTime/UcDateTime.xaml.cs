using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BinGuiDev.Components.ContDateTime;

public partial class UcDateTime : UserControl{
    private readonly ContextMenu _menu;
    public event EventHandler<DataChangedEventArgsLong>? DataChanged;
    public event EventHandler<DataChangedEventArgsBool>? EnableChanged;

    //DateTime
    private long _unixTime;

    private bool _suppressTextChanged = false;


    private void ManualDateTimeChanged(){
        var selectedDate = DatePicker.SelectedDate;
        if (!selectedDate.HasValue)
            return;
        if (!TimeSpan.TryParseExact(TimePicker.Text, @"hh\:mm\:ss",
                CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            return;
        DateTime combinedDateTime = selectedDate.Value.Date + timeSpan;
        _unixTime = (long)(combinedDateTime - DateTime.UnixEpoch).TotalSeconds * 1000000;
        DataChanged?.Invoke(this, new DataChangedEventArgsLong(_unixTime));
    }

    public void SetUnixTime(long unixTime, bool inProgram = true){
        _suppressTextChanged = true;
        _unixTime = unixTime;
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(_unixTime / 1000000).DateTime;
        DatePicker.SelectedDate = dateTime.Date;
        TimePicker.Text = dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        _suppressTextChanged = false;
    }

    //Enabled
    public void SetEnabledField(bool value){
        ButtonLock.IsChecked = !value;
        DatePicker.IsEnabled = value;
        TimePicker.IsEnabled = value;
        MainGrid.ContextMenu = value ? _menu : null;
    }

    public bool GetEnabledField(){
        return !ButtonLock.IsChecked ?? false;
    }

    public long GetUnixTime(){
        return _unixTime;
    }

    private void ButtonEnable_OnClick(object sender, RoutedEventArgs e){
        if (!ButtonLock.IsChecked ?? false){
            ButtonLock.IsChecked = true;
            return;
        }

        EnableChanged?.Invoke(this, new DataChangedEventArgsBool(!ButtonLock.IsChecked ?? false));

        SetEnabledField(!ButtonLock.IsChecked ?? false);
    }
    //-----

    public UcDateTime(){
        InitializeComponent();

        RoutedCommand startOfDate = new();
        RoutedCommand endOfDate = new();
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
        if (_suppressTextChanged) return;
        ManualDateTimeChanged();
    }

    private void DatePicker_SelectedDateChanged(object? sender, SelectionChangedEventArgs e){
        if (_suppressTextChanged) return;
        ManualDateTimeChanged();
    }
}