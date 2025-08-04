using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace BinGuiDev.Components.ContDateTime;



public class DataChangedEventArgsLong(long newValue) : EventArgs{
    public long NewValue{ get; } = newValue;
}

public class DataChangedEventArgsBool(bool newValue) : EventArgs{
    public bool NewValue{ get; } = newValue;
}

public class DataChangedEventArgsString(string newValue) : EventArgs{
    public string NewValue{ get; } = newValue;
}

public partial class ContDateTime : UserControl{
    readonly ContDateTimeInfo _data;
    public event EventHandler<DataChangedEventArgsContDateTimeInfo>? DataChange;

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

        SymbolPeriod.SymbolChanged += MyComponent_SymbolChanged;
        SymbolPeriod.PeriodChanged += MyComponent_PeriodChanged;
        
        DurationTime.SetEnabledField(false);
        SetInitData();
    }

    private void SetInitData(){
        ContDateTimeInfo contDateTimeInfo = new();
        contDateTimeInfo.StartUnixTime = (long)(DateTime.UtcNow.Date - DateTime.UnixEpoch).TotalSeconds*1000000;
        contDateTimeInfo.EndUnixTime = (long)(DateTime.UtcNow.Date.AddDays(1) - DateTime.UnixEpoch).TotalSeconds*1000000;
        contDateTimeInfo.Symbol = "BTCUSDT";
        contDateTimeInfo.Period = "15m";
        _data.StartUnixTime = contDateTimeInfo.StartUnixTime;
        _data.EndUnixTime = contDateTimeInfo.EndUnixTime;
        _data.Symbol = contDateTimeInfo.Symbol;
        _data.Period = contDateTimeInfo.Period;
        SetData(contDateTimeInfo);
        DurationTime.SetStep("0001:00:00:00");
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
        
        DataChange?.Invoke(this, new DataChangedEventArgsContDateTimeInfo(_data));
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
        
        DataChange?.Invoke(this, new DataChangedEventArgsContDateTimeInfo(_data));
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
        _data.StartUnixTime += e.NewValue;
        _data.EndUnixTime += e.NewValue;
        StartDateTime.SetUnixTime(_data.StartUnixTime);
        EndDateTime.SetUnixTime(_data.EndUnixTime);
        DataChange?.Invoke(this, new DataChangedEventArgsContDateTimeInfo(_data));
    }

    private void MyComponent_PeriodChanged(object? sender, DataChangedEventArgsString e){
        Log.Information("Змінено період: {ENewValue}", e.NewValue);
        if (e?.NewValue == _data.Period) return;
        _data.Period = e?.NewValue ?? string.Empty;
        DataChange?.Invoke(this, new DataChangedEventArgsContDateTimeInfo(_data));
    }

    private void MyComponent_SymbolChanged(object? sender, DataChangedEventArgsString e){
        Log.Information("Змінено символ: {ENewValue}", e.NewValue);
        if (e?.NewValue  == _data.Symbol) return;
        _data.Symbol = e?.NewValue ?? string.Empty;
        DataChange?.Invoke(this, new DataChangedEventArgsContDateTimeInfo(_data));
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

    public void SetData(ContDateTimeInfo data){
        data.StartUnixTime = data.StartUnixTime;
        data.EndUnixTime = data.EndUnixTime;
        data.Symbol = data.Symbol;
        data.Period = data.Period;
        
        StartDateTime.SetUnixTime(data.StartUnixTime);
        EndDateTime.SetUnixTime(data.EndUnixTime);
        DurationTime.SetUnixTime(data.EndUnixTime-data.StartUnixTime);
        SymbolPeriod.SetSymbol(data.Symbol);
        SymbolPeriod.SetPeriod(data.Period);
    }

    public ContDateTimeInfo GetData(){
        return _data;
    }
}