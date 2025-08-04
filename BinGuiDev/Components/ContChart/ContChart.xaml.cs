using System.Windows.Controls;
using Microsoft.Xaml.Behaviors.Core;

namespace BinGuiDev.Components.ContChart;

public partial class ContChart : UserControl{
    public ContChart(){
        InitializeComponent();
    }

    public void SetData(ContDateTimeInfo date){
        LabCaption.Content = $"Start:{date.StartUnixTime} End:{date.EndUnixTime} Symbol:{date.Symbol} Period:{date.Period}";
    }
}