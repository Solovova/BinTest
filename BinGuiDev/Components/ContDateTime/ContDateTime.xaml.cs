using System.Windows.Controls;

namespace BinGuiDev.Components.ContDateTime;

public partial class ContDateTime : UserControl
{
    public ContDateTime()
    {
        InitializeComponent();
    }
    
    public void SetData()
    {
        StartDateTime.UnixTime = 1751328020233044;
        EndDateTime.UnixTime = 1753747187975509;
        StartDateTime.EnabledField = false;
        EndDateTime.EnabledField = false;
    }
}