using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BinGuiDev.Components.ContDateTime;
using Serilog;
using Serilog.Events;

namespace BinGuiDev;

public class DataChangedEventArgsContDateTimeInfo(ContDateTimeInfo newValue) : EventArgs{
    public ContDateTimeInfo NewValue{ get; } = newValue;
}

public partial class MainWindow : Window{
    public MainWindow(){
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.File("logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                encoding: System.Text.Encoding.UTF8
            )
            .CreateLogger();
        
        InitializeComponent();
        ContDateTime.DataChange += ContDateTimeOnDataChange;
    }

    private void ContDateTimeOnDataChange(object? sender, DataChangedEventArgsContDateTimeInfo e){
        ContDateTimeInfo data = e.NewValue;
        Log.Information($"Start:{data.StartUnixTime} End:{data.EndUnixTime} Symbol:{data.Symbol} Period:{data.Period}");
    }

    private void OnToggleButtonClick(object sender, RoutedEventArgs e){
        ContDateTimeInfo contDateTimeInfo = new();
        contDateTimeInfo.StartUnixTime = (long)(DateTime.UtcNow.Date - DateTime.UnixEpoch).TotalSeconds*1000000;
        contDateTimeInfo.EndUnixTime = (long)(DateTime.UtcNow.Date.AddDays(1) - DateTime.UnixEpoch).TotalSeconds*1000000;
        contDateTimeInfo.Symbol = "BTCUSDT";
        contDateTimeInfo.Period = "15m";

        ContDateTime.SetData(contDateTimeInfo);
        Log.Information("Button");
    }
    
    
}