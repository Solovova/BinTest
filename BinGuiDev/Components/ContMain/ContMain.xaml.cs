using System.Windows.Controls;
using BinGuiDev.Components.ContDateTime;
using Serilog;

namespace BinGuiDev.Components.ContMain;

public partial class ContMain : UserControl{
    public ContMain(){
        InitializeComponent();
        ContDateTime.DataChange += ContDateTimeOnDataChange;
        
        ContChart.SetData(ContDateTime.GetData());
    }

    private void ContDateTimeOnDataChange(object? sender, ContDateTimeInfo newValue){
        ContDateTimeInfo data = newValue;
        ContChart.SetData(data);
        Log.Information(
            "Start:{DataStartUnixTime} End:{DataEndUnixTime} Symbol:{DataSymbol} Period:{DataPeriod}",
            data.StartUnixTime, data.EndUnixTime, data.Symbol, data.Period);
    }
}