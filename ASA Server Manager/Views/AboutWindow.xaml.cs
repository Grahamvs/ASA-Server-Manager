using System.Windows.Navigation;

namespace ASA_Server_Manager.Views;

public partial class AboutWindow
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        ViewModel.OpenWeblink(e.Uri.AbsoluteUri);

        e.Handled = true;
    }
}