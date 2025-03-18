namespace InstallerApp.Composant;

public partial class Demarrage : ContentView
{
	public Demarrage()
	{
		InitializeComponent();
	}
    private async void Button_Install(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///Installation");
    }
    private void Button_Exit(object sender, EventArgs e)
    {
        Application.Current?.Quit();
    }
}