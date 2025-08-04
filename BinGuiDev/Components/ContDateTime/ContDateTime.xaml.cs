using System.Windows.Controls;

namespace BinGuiDev.Components.ContDateTime;

public partial class ContDateTime : UserControl{
    public ContDateTime(){
        InitializeComponent();
    }

    public void SetData(){
        StartDateTime.SetUnixTime(1751328020233044);
        EndDateTime.SetUnixTime(1753747187975509);
        StartDateTime.SetEnabledField(false);
        EndDateTime.SetEnabledField(false);
    }
}