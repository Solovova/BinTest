using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BinGuiDev.Components.ContDateTime;

public partial class UcDuration : UserControl{
    private readonly ContextMenu _menu;
    private long _unixTimeDuration;
    private bool _suppressTextChanged = false;

    public event EventHandler<DataChangedEventArgsBool>? EnableChanged;

    public event EventHandler<DataChangedEventArgsLong>? DataChanged;
    public event EventHandler<DataChangedEventArgsLong>? ClickLeftRight;

    public UcDuration(){
        InitializeComponent();

        RoutedCommand setDurationOneDay = new();
        RoutedCommand setDurationOneHour = new();
        _menu = new ContextMenu();
        _menu.Items.Add(new MenuItem{ Header = "Day", Command = setDurationOneDay });
        _menu.Items.Add(new MenuItem{ Header = "Hour", Command = setDurationOneHour });

        CommandBindings.Add(new CommandBinding(setDurationOneDay, Menu_SetDurationOneDay));
        CommandBindings.Add(new CommandBinding(setDurationOneHour, Menu_SetDurationOneHour));
        MainGrid.ContextMenu = _menu;
    }

    public void SetEnabledField(bool value){
        ButtonLock.IsChecked = !value;
        DurationTextBox.IsEnabled = value;
        MainGrid.ContextMenu = value ? _menu : null;
    }

    public bool GetEnabledField(){
        return !ButtonLock.IsChecked ?? false;
    }

    private void ButtonEnable_OnClick(object sender, RoutedEventArgs e){
        if (!ButtonLock.IsChecked ?? false){
            ButtonLock.IsChecked = true;
            return;
        }

        EnableChanged?.Invoke(this, new DataChangedEventArgsBool(!ButtonLock.IsChecked ?? false));
        SetEnabledField(!ButtonLock.IsChecked ?? false);
    }

    private void Menu_SetDurationOneDay(object sender, RoutedEventArgs e){
        DurationTextBox.Text = "0001:00:00:00";
    }

    private void Menu_SetDurationOneHour(object sender, RoutedEventArgs e){
        DurationTextBox.Text = "0000:01:00:00";
    }

    private void ManualDateTimeChanged(){
        string durationText = DurationTextBox.Text;
        string[] parts = durationText.Split(':');

        long unixTime = 0;
        if (parts.Length == 4){
            unixTime = int.Parse(parts[0]) * 86400 // дні в секунди
                       + int.Parse(parts[1]) * 3600 // години в секунди
                       + int.Parse(parts[2]) * 60 // хвилини в секунди
                       + int.Parse(parts[3]); // секунди
        }

        _unixTimeDuration = unixTime * 1000000;
        DataChanged?.Invoke(this, new DataChangedEventArgsLong(_unixTimeDuration));
    }

    private void UnixTimeDurationChanged(bool inProgram = true){
        int days = (int)(_unixTimeDuration / 1000000 / 86400);
        int remainder = (int)((_unixTimeDuration / 1000000) % 86400);

        int hours = remainder / 3600;
        remainder = remainder % 3600;

        int minutes = remainder / 60;
        int seconds = remainder % 60;

        DurationTextBox.Text = $"{days:D4}:{hours:D2}:{minutes:D2}:{seconds:D2}";
        if (!inProgram) DataChanged?.Invoke(this, new DataChangedEventArgsLong(_unixTimeDuration));
    }

    public void SetUnixTime(long unixTimeDuration, bool inProgram = true){
        _suppressTextChanged = true;
        _unixTimeDuration = unixTimeDuration;
        UnixTimeDurationChanged(inProgram);
        _suppressTextChanged = false;
    }

    public long GetUnixTime(){
        return _unixTimeDuration;
    }

    private void DurationTextBox_TextChanged(object sender, TextChangedEventArgs e){
        if (_suppressTextChanged) return;
        ManualDateTimeChanged();
    }

    private long GetStepDateTime(){
        string durationText = StepTextBox.Text;
        string[] parts = durationText.Split(':');

        long unixTime = 0;
        if (parts.Length == 4){
            unixTime = int.Parse(parts[0]) * 86400 // дні в секунди
                       + int.Parse(parts[1]) * 3600 // години в секунди
                       + int.Parse(parts[2]) * 60 // хвилини в секунди
                       + int.Parse(parts[3]); // секунди
        }

        return unixTime * 1000000;
    }

    private void ButtonBase_OnClickLeft(object sender, RoutedEventArgs e){
        ClickLeftRight?.Invoke(this, new DataChangedEventArgsLong(-1*GetStepDateTime()));
    }

    private void ButtonBase_OnClickRight(object sender, RoutedEventArgs e){
        ClickLeftRight?.Invoke(this, new DataChangedEventArgsLong(GetStepDateTime()));
    }
}