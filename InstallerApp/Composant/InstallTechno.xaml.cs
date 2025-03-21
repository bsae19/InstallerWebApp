using System.Diagnostics;
using System.Runtime.InteropServices;
using InstallerApp.Class;

namespace InstallerApp.Composant;

public partial class InstallTechno : ContentView
{
    private string? SelectedPath { get; set; }
    private object? _technoInstance;
    public InstallTechno()
	{
		InitializeComponent();
    }

    private void ResetPageData()
    {
        // Réinitialiser toutes les données
        try
        {
            SelectedPath = null;
            Label_Path.Text = "choisissez un dossier";
            Project_Name.Text = string.Empty;
        }
        catch (COMException ex)
        {
            Debug.WriteLine($"Error in ResetPageData: {ex.Message}");
        }
    }



    private async void OnSelectFolderClicked(object sender, EventArgs e) 
	{
        var folderPath = await Folder.GetFolder();
        if (folderPath != null)
        {
            SelectedPath = folderPath;
            Label_Path.Text = SelectedPath;
        }
    }
    public void setFramework(string techno)
    {
        Label_Techno.Text = techno;
        var technoClass = Type.GetType($"InstallerApp.Class.{techno}");

        if (technoClass != null)
        {
            _technoInstance = Activator.CreateInstance(technoClass);
            Debug.WriteLine($"Techno instance created: {_technoInstance?.GetType()}");
        }
        else
        {
            Debug.WriteLine($"Class {techno} not found.");
        }
    }
    private async void Button_Installer_Clicked(object sender, EventArgs e)
    {
        if (_technoInstance != null && SelectedPath != null && !string.IsNullOrEmpty(Label_Techno.Text))
        {
            var installMethod = _technoInstance.GetType().GetMethod("Install");
            if (installMethod != null)
            {
                Debug.WriteLine($"Calling Install method on {_technoInstance?.GetType()}");
                object? result = installMethod.Invoke(_technoInstance, new object[] { Path.Combine(SelectedPath, Project_Name.Text),Shell.Current });
                if (result is Task<bool> taskBool)
                {
                    bool clean = await taskBool;
                    if (clean)
                    {
                        ResetPageData();
                    }
                }

            }
            else
            {
                Debug.WriteLine("Install method not found on the techno instance.");
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Erreur",
                        "Veuilllez renseigner tout les champs",
                        "OK");
        }
    }
    private async void Button_Retour(object sender, EventArgs e)
    {
        try
        {
            ResetPageData();
            await Shell.Current.GoToAsync("//Installation");
        }
        catch (COMException ex)
        {
            Debug.WriteLine($"Error during navigation: {ex.Message}");
        }
    }
}