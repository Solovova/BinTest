using System.Windows.Controls;

namespace BinGuiDev.Components.ContChart;

public partial class ContChart : UserControl{
    public ContChart(){
        InitializeComponent();
    }

    public void SetData(ContDateTimeInfo date){
        LabCaption.Content = $"Start:{date.StartUnixTime} End:{date.EndUnixTime} Symbol:{date.Symbol} Period:{date.Period}";
    }
}