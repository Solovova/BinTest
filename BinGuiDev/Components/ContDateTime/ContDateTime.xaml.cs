using System.Windows.Controls;
using Serilog;

namespace BinGuiDev.Components.ContDateTime;

public partial class ContDateTime : UserControl{
    readonly ContDateTimeInfo _data;
    public event EventHandler<ContDateTimeInfo>? DataChange;

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

    private void MyComponent_DataChangedStart(object? sender, long newValue){
        if (_data.StartUnixTime == newValue) return;
        _data.StartUnixTime = newValue;
        Log.Information("Дані змінились Start: {ENewValue}", newValue);

        if (EndDateTime.GetEnabledField()){
            _data.EndUnixTime = _data.StartUnixTime + DurationTime.GetUnixTime();
            EndDateTime.SetUnixTime(StartDateTime.GetUnixTime() + DurationTime.GetUnixTime());
        }
        else{
            DurationTime.SetUnixTime(EndDateTime.GetUnixTime() - StartDateTime.GetUnixTime());
        }
        
        DataChange?.Invoke(this, _data);
    }

    private void MyComponent_DataChangedEnd(object? sender, long newValue){
        if (_data.EndUnixTime == newValue) return;
        _data.EndUnixTime = newValue;
        Log.Information("Дані змінились End: {ENewValue}", newValue);

        if (StartDateTime.GetEnabledField()){
            _data.StartUnixTime = _data.EndUnixTime - DurationTime.GetUnixTime();
            StartDateTime.SetUnixTime(EndDateTime.GetUnixTime() - DurationTime.GetUnixTime());
        }
        else{
            DurationTime.SetUnixTime(EndDateTime.GetUnixTime() - StartDateTime.GetUnixTime());
        }
        
        DataChange?.Invoke(this, _data);
    }

    private void MyComponent_DataChangedDuration(object? sender, long newValue){
        long oldValue = _data.EndUnixTime - _data.StartUnixTime;
        if (oldValue == newValue) return;
        Log.Information("Дані змінились Duration: {ENewValue}", newValue);
        if (StartDateTime.GetEnabledField()){
            _data.StartUnixTime = _data.EndUnixTime - DurationTime.GetUnixTime();
            StartDateTime.SetUnixTime(EndDateTime.GetUnixTime() - DurationTime.GetUnixTime());
        }
        else{
            _data.EndUnixTime = _data.StartUnixTime + DurationTime.GetUnixTime();
            EndDateTime.SetUnixTime(StartDateTime.GetUnixTime() + DurationTime.GetUnixTime());
        }
    }

    private void MyComponent_DataChangedDurationLeftRight(object? sender, long newValue){
        Log.Information("Дані рухаємо Duration: {ENewValue}", newValue);
        _data.StartUnixTime += newValue;
        _data.EndUnixTime += newValue;
        StartDateTime.SetUnixTime(_data.StartUnixTime);
        EndDateTime.SetUnixTime(_data.EndUnixTime);
        DataChange?.Invoke(this, _data);
    }

    private void MyComponent_PeriodChanged(object? sender, string newValue){
        Log.Information("Змінено період: {ENewValue}", newValue);
        if (newValue == _data.Period) return;
        _data.Period = newValue;
        DataChange?.Invoke(this, _data);
    }

    private void MyComponent_SymbolChanged(object? sender, string newValue){
        Log.Information("Змінено символ: {ENewValue}", newValue);
        if (newValue  == _data.Symbol) return;
        _data.Symbol = newValue;
        DataChange?.Invoke(this, _data);
    }

    private void MyComponent_EnableChangedStart(object? sender, bool newValue){
        if (StartDateTime.GetEnabledField()) return;
        EndDateTime.SetEnabledField(true);
        DurationTime.SetEnabledField(true);
    }

    private void MyComponent_EnableChangedEnd(object? sender, bool newValue){
        if (EndDateTime.GetEnabledField()) return;
        StartDateTime.SetEnabledField(true);
        DurationTime.SetEnabledField(true);
    }

    private void MyComponent_EnableChangedDuration(object? sender, bool newValue){
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