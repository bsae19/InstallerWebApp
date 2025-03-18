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
    public void setTechno(string techno)
    {
        Label_Techno.Text = techno;
        var technoClass = Type.GetType($"InstallerApp.Class.{techno}");

        if (technoClass != null)
        {
            // Store the created instance in a class-level variable
            _technoInstance = Activator.CreateInstance(technoClass);

            //switch (techno)
            //{
            //    case "Flask":

            //        break;
            //    case "Symfony":
            //        break;

            //}
            Debug.WriteLine($"Techno instance created: {_technoInstance?.GetType()}");
            // Now you can use _technoInstance elsewhere in your class
            //if (_technoInstance is SomeClass someClassInstance)
            //{
            //    // Perform operations specific to SomeClass
            //    someClassInstance.SomeMethod();
            //}
            //else
            //{
            //    // Handle case where the type doesn't match
            //    Console.WriteLine($"Unable to create instance of {Techno}");
            //}
        }
        else
        {
            // Handle error if the class does not exist
            Console.WriteLine($"Class {techno} not found.");
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
                installMethod.Invoke(_technoInstance, new object[] { Path.Combine(SelectedPath, Label_Techno.Text),Shell.Current });
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