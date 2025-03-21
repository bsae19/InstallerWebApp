namespace InstallerApp.Composant;

public partial class ProgressPopup : ContentPage
{
	public ProgressPopup()
	{
		InitializeComponent();
	}
    public void UpdateStatus(string message)
    {
        lblStatus.Text = message;
    }
}