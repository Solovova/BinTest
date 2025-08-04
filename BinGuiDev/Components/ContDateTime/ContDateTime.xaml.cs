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

public class DataChangedEventArgsLong : EventArgs{
    public long NewValue{ get; }

    public DataChangedEventArgsLong(long newValue){
        NewValue = newValue;
    }
}

public class DataChangedEventArgsBool : EventArgs{
    public bool NewValue{ get; }

    public DataChangedEventArgsBool(bool newValue){
        NewValue = newValue;
    }
}

public partial class ContDateTime : UserControl{
    ContDateTimeInfo _data;

    public ContDateTime(){
        InitializeComponent();
        _data = new ContDateTimeInfo();
        StartDateTime.DataChanged += MyComponent_DataChangedStart;
        EndDateTime.DataChanged += MyComponent_DataChangedEnd;
        StartDateTime.EnableChanged += MyComponent_EnableChangedStart;
        EndDateTime.EnableChanged += MyComponent_EnableChangedEnd;
    }

    public void SetData(){
        StartDateTime.SetUnixTime(1751328020233044);
        EndDateTime.SetUnixTime(1753747187975509);
        StartDateTime.SetEnabledField(false);
        EndDateTime.SetEnabledField(false);
    }

    private void MyComponent_DataChangedStart(object? sender, DataChangedEventArgsLong e){
        if (_data.StartUnixTime == e.NewValue) return;
        _data.StartUnixTime = e.NewValue;
        Log.Information($"Дані змінились Start: {e.NewValue}");
    }

    private void MyComponent_DataChangedEnd(object? sender, DataChangedEventArgsLong e){
        if (_data.EndUnixTime == e.NewValue) return;
        _data.EndUnixTime = e.NewValue;
        Log.Information($"Дані змінились End: {e.NewValue}");
    }
    
    private void MyComponent_EnableChangedStart(object? sender, DataChangedEventArgsBool e){
        
    }
    
    private void MyComponent_EnableChangedEnd(object? sender, DataChangedEventArgsBool e){
        
    }
    
    
}