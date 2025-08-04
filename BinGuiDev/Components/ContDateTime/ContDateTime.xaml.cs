using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace BinGuiDev.Components.ContDateTime;

public class ContDateTimeInfo{
    public long StartUnixTime;
    public long EndUnixTime;
    public string Symbol;
    public string Period;
}

public class DataChangedEventArgsLong(long newValue) : EventArgs{
    public long NewValue{ get; } = newValue;
}

public class DataChangedEventArgsBool(bool newValue) : EventArgs{
    public bool NewValue{ get; } = newValue;
}

public partial class ContDateTime : UserControl{
    readonly ContDateTimeInfo _data;

    public ContDateTime(){
        InitializeComponent();
        _data = new ContDateTimeInfo();
        StartDateTime.DataChanged += MyComponent_DataChangedStart;
        EndDateTime.DataChanged += MyComponent_DataChangedEnd;
        DurationTime.DataChanged += MyComponent_DataChangedDuration;
        DurationTime.ClickLeftRight += MyComponent_DataChangedDurationLeftRight;

        StartDateTime.EnableChanged += MyComponent_EnableChangedStart;
        EndDateTime.EnableChanged += MyComponent_EnableChangedEnd;

        DurationTime.EnableChanged += MyComponent_EnableChangedDuration;
        DurationTime.SetEnabledField(false);
    }

    public void SetData(){
        StartDateTime.SetUnixTime(1751328020233044);
        //EndDateTime.SetUnixTime(1751328020233044+(long)24*60*60*1000000);;
        //EndDateTime.SetUnixTime(1753747187975509);
        //StartDateTime.SetEnabledField(false);
        //EndDateTime.SetEnabledField(false);
    }

    private void MyComponent_DataChangedStart(object? sender, DataChangedEventArgsLong e){
        if (_data.StartUnixTime == e.NewValue) return;
        _data.StartUnixTime = e.NewValue;
        Log.Information("Дані змінились Start: {ENewValue}", e.NewValue);

        if (EndDateTime.GetEnabledField()){
            EndDateTime.SetUnixTime(StartDateTime.GetUnixTime() + DurationTime.GetUnixTime());
        }
        else{
            DurationTime.SetUnixTime(EndDateTime.GetUnixTime() - StartDateTime.GetUnixTime());
        }
    }

    private void MyComponent_DataChangedEnd(object? sender, DataChangedEventArgsLong e){
        if (_data.EndUnixTime == e.NewValue) return;
        _data.EndUnixTime = e.NewValue;
        Log.Information("Дані змінились End: {ENewValue}", e.NewValue);

        if (StartDateTime.GetEnabledField()){
            StartDateTime.SetUnixTime(EndDateTime.GetUnixTime() - DurationTime.GetUnixTime());
        }
        else{
            DurationTime.SetUnixTime(EndDateTime.GetUnixTime() - StartDateTime.GetUnixTime());
        }
    }

    private void MyComponent_DataChangedDuration(object? sender, DataChangedEventArgsLong e){
        long oldValue = _data.EndUnixTime - _data.StartUnixTime;
        if (oldValue == e.NewValue) return;
        Log.Information("Дані змінились Duration: {ENewValue}", e.NewValue);
        if (StartDateTime.GetEnabledField()){
            StartDateTime.SetUnixTime(EndDateTime.GetUnixTime() - DurationTime.GetUnixTime());
        }
        else{
            EndDateTime.SetUnixTime(StartDateTime.GetUnixTime() + DurationTime.GetUnixTime());
        }
    }

    private void MyComponent_DataChangedDurationLeftRight(object? sender, DataChangedEventArgsLong e){
        Log.Information("Дані рухаємо Duration: {ENewValue}", e.NewValue);
        StartDateTime.SetUnixTime(StartDateTime.GetUnixTime() + e.NewValue);
        EndDateTime.SetUnixTime(EndDateTime.GetUnixTime() + e.NewValue);
    }

    private void MyComponent_EnableChangedStart(object? sender, DataChangedEventArgsBool e){
        if (StartDateTime.GetEnabledField()) return;
        EndDateTime.SetEnabledField(true);
        DurationTime.SetEnabledField(true);
    }

    private void MyComponent_EnableChangedEnd(object? sender, DataChangedEventArgsBool e){
        if (EndDateTime.GetEnabledField()) return;
        StartDateTime.SetEnabledField(true);
        DurationTime.SetEnabledField(true);
    }

    private void MyComponent_EnableChangedDuration(object? sender, DataChangedEventArgsBool e){
        if (DurationTime.GetEnabledField()) return;
        StartDateTime.SetEnabledField(true);
        EndDateTime.SetEnabledField(true);
    }
}