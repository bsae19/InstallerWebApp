namespace InstallerApp;

public partial class Installation : ContentPage
{
	public Installation()
	{
		InitializeComponent();
	}
    private void Button_Exit(object sender, EventArgs e)
    {
        Application.Current?.Quit();
    }
    private void Button_Node(object sender, EventArgs e)
    {
    }
    private void Button_Symfony(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Install_Techno?Techno=Symfony");
    }
    private void Button_Flask(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Install_Techno?Techno=Flask");
    }
}