using System.Diagnostics;
using System.Runtime.InteropServices;

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
        Shell.Current.GoToAsync("//SelectFramework?Techno=React");
    }
    private void Button_PHP(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//SelectFramework?Techno=PHP");
    }
    private void Button_Python(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//SelectFramework?Techno=Python");
    }
    private async void Button_Retour(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("//Demarrage");
        }
        catch (COMException ex)
        {
            Debug.WriteLine($"Error during navigation: {ex.Message}");
        }
    }
}