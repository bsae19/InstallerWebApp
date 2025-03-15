using CommunityToolkit.Maui.Storage;
using System.Threading;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using LibGit2Sharp;
using System.Diagnostics;
namespace InstallerApp;

[QueryProperty(nameof(Techno), "Techno")]
public partial class Install_Techno : ContentPage
{
    private string? _techno;
    public string? Techno
    {
        get => _techno;
        set
        {
            _techno = value;
            OnPropertyChanged();
            InitializeWithTechno();
        }
    }

    private string? SelectedPath { get; set; }
    private bool _isInitialized = false;

    public Install_Techno()
    {
        InitializeComponent();
    }

    private async void InitializeWithTechno()
    {
        if (string.IsNullOrEmpty(Techno) || _isInitialized)
            return;

        _isInitialized = true;

        // Update UI with technology name
        Label_Techno.Text = Techno;

        // Start folder selection
    }

    private async Task SelectFolderAsync()
    {
        try
        {
            var folderResult = await FolderPicker.PickAsync(cancellationToken: CancellationToken.None);
            if (folderResult.IsSuccessful)
            {
                SelectedPath = folderResult.Folder.Path;
                Label_Path.Text = SelectedPath;
            }
            else
            {
                // User canceled folder selection
                await Shell.Current.GoToAsync("//Installation");
            }
        }
        catch (Exception ex)
        {
            // Display error message
            await DisplayAlert("Erreur", $"Impossible de s�lectionner le dossier: {ex.Message}", "OK");
            await Shell.Current.GoToAsync("//Installation");
        }
    }

    private async void Button_Installer_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedPath))
        {
            await DisplayAlert("Attention", "Veuillez s�lectionner un dossier d'installation", "OK");
            return;
        }
        if (string.IsNullOrEmpty(Project_Name.Text))
        {
            await DisplayAlert("Attention", "Veuillez entrz un nom de projet", "OK");
            return;
        }

        // Add your installation logic here
        try
        {
            // Example: Show loading message
            var toast = Toast.Make($"Installation de {Techno} en cours...", ToastDuration.Short);
            await toast.Show();

            switch (Techno)
            {
                case "Flask":
                    string projectName = Project_Name.Text;
                    string projectPath = Path.Combine(SelectedPath, projectName);

                    // Cr�e le r�pertoire du projet si n�cessaire
                    Directory.CreateDirectory(projectPath);

                    // Clone le d�p�t Flask si vous avez un d�p�t GitHub � cloner
                    string repositoryUrl = $"https://github.com/bsae19/python_flask.git";
                    bool cloned=CloneRepositoryFromGitHub(repositoryUrl, projectPath);
                    if (cloned)
                    {
                        if (DeviceInfo.Platform == DevicePlatform.iOS)
                        {
                            // Show message for iOS users (cannot install Flask directly on iOS)
                            await DisplayAlert("Instructions",
                                "Flask ne peut pas �tre install� directement depuis l'application sur iOS.\n" +
                                "Veuillez installer Python et Flask manuellement sur votre machine.\n\n" +
                                "1. Cr�ez un environnement virtuel avec la commande :\n" +
                                "   python -m venv venv\n" +
                                "2. Installez Flask avec la commande :\n" +
                                "   pip install flask",
                                "OK");
                        }
                        else
                        {
                            if (!IsPythonInstalled())
                            {
                                await DisplayAlert("Erreur", "Python n'est pas install� sur ce syst�me. Veuillez installer Python avant de continuer.", "OK");
                                return;
                            }
                            // For other platforms like Windows or Android
                            // Provide instructions or steps to install Flask

                            // Create a virtual environment (only for non-iOS platforms)
                            await CreateVirtualEnvironment(projectPath);

                            // Install Flask using pip (only for non-iOS platforms)
                            await InstallFlask(projectPath);
                        }
                    }
                    else
                    {

                        await DisplayAlert("Erreur Git", $"Erreur lors du clonage du d�p�t", "OK");
                    }
                    break;
            }

            var toast2 = Toast.Make($"{Techno} a �t� install� avec succ�s dans {SelectedPath}", ToastDuration.Long);
            await toast2.Show();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"�chec de l'installation: {ex.Message}", "OK");
        }
    }

    private bool IsPythonInstalled()
    {
        try
        {
            // Try running "python --version" or "python3 --version" depending on the system
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "python",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = System.Diagnostics.Process.Start(processStartInfo);
            process.WaitForExit();

            // If Python is installed, the process will complete without error
            if (process.ExitCode == 0)
            {
                return true;
            }
        }
        catch (Exception)
        {
            // If an exception occurs, Python is not installed or not accessible
        }

        return false;
    }



    private static bool CloneRepositoryFromGitHub(string repositoryUrl, string localPath)
    {
        try
        {
            // Assurez-vous d'avoir install� une biblioth�que Git comme LibGit2Sharp ou utilisez la commande Git via Process
            using (var repo = new LibGit2Sharp.Repository(Repository.Clone(repositoryUrl, localPath)))
            {
                // Si le clonage est r�ussi
                Debug.WriteLine($"Le projet {repositoryUrl} a �t� clon� avec succ�s � {localPath}");
            }
            return true;
        }
        catch (Exception ex)
        {
            // Affiche une erreur si le clonage �choue
            Debug.WriteLine($"Erreur lors du clonage du d�p�t {repositoryUrl}: {ex.Message}");
            return false;
        }
    }
    private async Task CreateVirtualEnvironment(string projectPath)
    {
        try
        {
            string venvPath = Path.Combine(projectPath, "venv");

            // Cr�er l'environnement virtuel
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"-m venv {venvPath}",
                WorkingDirectory = projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = System.Diagnostics.Process.Start(processStartInfo);
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception("�chec de la cr�ation de l'environnement virtuel");
            }

            Console.WriteLine("Environnement virtuel cr�� avec succ�s.");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"�chec de la cr�ation de l'environnement virtuel: {ex.Message}", "OK");
        }
    }

    private async Task InstallFlask(string projectPath)
    {
        try
        {
            string venvScriptsPath = Path.Combine(projectPath, "venv", "Scripts");
            string pipPath = Path.Combine(venvScriptsPath, "pip.exe");

            // Use pip to install Flask
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = pipPath,
                Arguments = "install flask",
                WorkingDirectory = projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = System.Diagnostics.Process.Start(processStartInfo);
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception("�chec de l'installation de Flask");
            }

            Console.WriteLine("Flask a �t� install� avec succ�s.");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"�chec de l'installation de Flask: {ex.Message}", "OK");
        }
    }



    private async void Button_Retour(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Installation");
    }
    private async void OnSelectFolderClicked(object sender, EventArgs e)
    {

        await SelectFolderAsync();
    }
}