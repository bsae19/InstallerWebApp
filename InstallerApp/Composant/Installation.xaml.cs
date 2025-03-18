namespace InstallerApp.Composant;

public partial class Installation : ContentView
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
        Shell.Current.GoToAsync("//InstallTechno?Techno=Symfony");
    }
    private void Button_Flask(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//InstallTechno?Techno=Flask");
    }
}