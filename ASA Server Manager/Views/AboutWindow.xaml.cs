using System.Diagnostics;
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
        // Use Process.Start to open the URL in the default browser.
        Process.Start(
            new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            }
        );

        e.Handled = true;
    }
}