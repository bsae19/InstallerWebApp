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

    public Install_Techno()
    {
        Debug.WriteLine("start");
        InitializeComponent();
        this.Appearing += Install_Techno_Appearing;
    }
    private void Install_Techno_Appearing(object sender, EventArgs e)
    {
        ResetPageData();
    }

    private void ResetPageData()
    {
        // Réinitialiser toutes les données
        SelectedPath = null;
        Label_Path.Text = "choisissez un dossier";
        Project_Name.Text = string.Empty;

        // Déclencher l'initialisation si Techno est déjà défini
        if (!string.IsNullOrEmpty(Techno))
        {
            InitializeWithTechno();
        }
    }

    private async void InitializeWithTechno()
    {
        if (string.IsNullOrEmpty(Techno))
            return;


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
        }
        catch (Exception ex)
        {
            // Display error message
            await DisplayAlert("Erreur", $"Impossible de sélectionner le dossier: {ex.Message}", "OK");
            await Shell.Current.GoToAsync("//Installation");
        }
    }

    private bool IsPhpInstalled()
    {
        try
        {
            // Try running "php --version"
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "php",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = System.Diagnostics.Process.Start(processStartInfo);
            process.WaitForExit();

            // If PHP is installed, the process will complete without error
            if (process.ExitCode == 0)
            {
                return true;
            }
        }
        catch (Exception)
        {
            // If an exception occurs, PHP is not installed or not accessible
        }

        return false;
    }

    private async void Button_Installer_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedPath))
        {
            await DisplayAlert("Attention", "Veuillez sélectionner un dossier d'installation", "OK");
            return;
        }
        if (string.IsNullOrEmpty(Project_Name.Text))
        {
            await DisplayAlert("Attention", "Veuillez entrer un nom de projet", "OK");
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

                    // Crée le répertoire du projet si nécessaire
                    Directory.CreateDirectory(projectPath);

                    // Clone le dépôt Flask si vous avez un dépôt GitHub à cloner
                    string repositoryUrl = $"https://github.com/bsae19/python_flask.git";
                    bool cloned = CloneRepositoryFromGitHub(repositoryUrl, projectPath);
                    if (cloned)
                    {
                        if (DeviceInfo.Platform == DevicePlatform.iOS)
                        {
                            // Show message for iOS users (cannot install Flask directly on iOS)
                            await DisplayAlert("Instructions",
                                "Flask ne peut pas être installé directement depuis l'application sur iOS.\n" +
                                "Veuillez installer Python et Flask manuellement sur votre machine.\n\n" +
                                "1. Créez un environnement virtuel avec la commande :\n" +
                                "   python -m venv venv\n" +
                                "2. Installez Flask avec la commande :\n" +
                                "   pip install flask",
                                "OK");
                        }
                        else
                        {
                            if (!IsPythonInstalled())
                            {
                                await DisplayAlert("Erreur", "Python n'est pas installé sur ce système. Veuillez installer Python avant de continuer.", "OK");
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
                        await DisplayAlert("Erreur Git", $"Erreur lors du clonage du dépôt", "OK");
                    }
                    break;
                case "Symfony":

                    if (DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        // Show message for iOS users (cannot install Symfony directly on iOS)
                        await DisplayAlert("Instructions",
                            "PHP ne peut pas être installé directement depuis l'application sur iOS.\n" +
                            "Veuillez installer PHP manuellement sur votre machine.\n\n" +
                            "1. Installez PHP avec la commande :\n" +
                            "   brew install php\n" +
                            "2. Installez Composer avec la commande :\n" +
                            "   php -r \"copy('https://getcomposer.org/installer', 'composer-setup.php');\"\n" +
                            "   php composer-setup.php\n" +
                            "   php -r \"unlink('composer-setup.php');\"\n" +
                            "3. Installez Symfony avec la commande :\n" +
                            "   composer create-project symfony/skeleton my_project_name",
                            "OK");
                        await DisplayAlert("Instructions",
                            "Symfony ne peut pas être installé directement depuis l'application sur iOS.\n" +
                            "Veuillez installer PHP et Symfony manuellement sur votre machine.\n\n" +
                            "1. Installez PHP avec la commande :\n" +
                            "   brew install php\n" +
                            "2. Installez Composer avec la commande :\n" +
                            "   php -r \"copy('https://getcomposer.org/installer', 'composer-setup.php');\"\n" +
                            "   php composer-setup.php\n" +
                            "   php -r \"unlink('composer-setup.php');\"\n" +
                            "3. Installez Symfony avec la commande :\n" +
                            "   composer create-project symfony/skeleton my_project_name",
                            "OK");
                    }
                    else
                    {

                        if (!IsPhpInstalled())
                        {
                            await DisplayAlert("Erreur", "PHP n'est pas installé sur ce système. Veuillez installer PHP avant de continuer.", "OK");
                            return;
                        }

                        // Add Symfony installation logic for other platforms here
                    }
                    break;
            }

            var toast2 = Toast.Make($"{Techno} a été installé avec succès dans {SelectedPath}", ToastDuration.Long);
            await toast2.Show();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Échec de l'installation: {ex.Message}", "OK");
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
            // Assurez-vous d'avoir installé une bibliothèque Git comme LibGit2Sharp ou utilisez la commande Git via Process
            using (var repo = new LibGit2Sharp.Repository(Repository.Clone(repositoryUrl, localPath)))
            {
                // Si le clonage est réussi
                Debug.WriteLine($"Le projet {repositoryUrl} a été cloné avec succès à {localPath}");
            }
            return true;
        }
        catch (Exception ex)
        {
            // Affiche une erreur si le clonage échoue
            Debug.WriteLine($"Erreur lors du clonage du dépôt {repositoryUrl}: {ex.Message}");
            return false;
        }
    }
    private async Task CreateVirtualEnvironment(string projectPath)
    {
        try
        {
            string venvPath = Path.Combine(projectPath, "venv");

            // Créer l'environnement virtuel
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
                throw new Exception("Échec de la création de l'environnement virtuel");
            }

            Console.WriteLine("Environnement virtuel créé avec succès.");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Échec de la création de l'environnement virtuel: {ex.Message}", "OK");
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
                throw new Exception("Échec de l'installation de Flask");
            }

            Console.WriteLine("Flask a été installé avec succès.");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Échec de l'installation de Flask: {ex.Message}", "OK");
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