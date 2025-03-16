using CommunityToolkit.Maui.Storage;
using System.Threading;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using LibGit2Sharp;
using System.Diagnostics;
using Microsoft.Maui.Storage;
using System;
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

    #region parametre_page
    public Install_Techno()
    {
        InitializeComponent();
        this.Appearing += Install_Techno_Appearing;
    }
    private void Install_Techno_Appearing(object? sender, EventArgs e)
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

    private void InitializeWithTechno()
    {
        if (string.IsNullOrEmpty(Techno))
            return;


        // Update UI with technology name
        Label_Techno.Text = Techno;
        // Start folder selection
    }

    #endregion


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
            string projectName = Project_Name.Text;
            string projectPath = Path.Combine(SelectedPath, projectName);
            switch (Techno)
            {
                case "Flask":
                    

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
                                bool isScoop = IsScoopInstalled();
                                bool installPython = await DisplayAlert(
                                    "Python non installé",
                                    "Python n'est pas installé sur ce système."+(isScoop?"":"\n(cela entrainera l'installation du service scoop)")+ "\nVoulez-vous l'installer maintenant ?",
                                    "Oui", "Non"
                                );

                                if (installPython)
                                {
                                    if (!isScoop)
                                    {
                                        InstallScoop();
                                    }
                                    InstallPython();
                                    await CreateVirtualEnvironment(projectPath);

                                    // Install Flask using pip (only for non-iOS platforms)
                                    await InstallFlask(projectPath);
                                }
                            }
                            else
                            {
                                await CreateVirtualEnvironment(projectPath);

                                // Install Flask using pip (only for non-iOS platforms)
                                
                            }
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
                        //await DisplayAlert("Instructions",
                        //    "Symfony ne peut pas être installé directement depuis l'application sur iOS.\n" +
                        //    "Veuillez installer PHP et Symfony manuellement sur votre machine.\n\n" +
                        //    "1. Installez PHP avec la commande :\n" +
                        //    "   brew install php\n" +
                        //    "2. Installez Composer avec la commande :\n" +
                        //    "   php -r \"copy('https://getcomposer.org/installer', 'composer-setup.php');\"\n" +
                        //    "   php composer-setup.php\n" +
                        //    "   php -r \"unlink('composer-setup.php');\"\n" +
                        //    "3. Installez Symfony avec la commande :\n" +
                        //    "   composer create-project symfony/skeleton my_project_name",
                        //    "OK");
                    }
                    else
                    {

                        if (!IsPhpInstalled())
                        {
                            bool isScoop = IsScoopInstalled();
                            bool installPhp = await DisplayAlert(
                                "PHP non installé",
                                "PHP n'est pas installé sur ce système." + (isScoop ? "" : "\n(cela entrainera l'installation du service scoop)") + "\nVoulez-vous l'installer maintenant ?",
                                "Oui", "Non"
                            );
                            if (installPhp)
                            {
                                if (!isScoop)
                                {
                                    InstallScoop();
                                }
                                InstallPHP();
                                CreateSymfonyProject(SelectedPath);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("demarrage");
                            await CreateSymfonyProject(projectPath);
                            Debug.WriteLine("fin");
                        }

                        // Add Symfony installation logic for other platforms here
                    }
                    break;
            }

            var toast2 = Toast.Make($"{Techno} a été installé avec succès dans {SelectedPath}", ToastDuration.Long);
            await toast2.Show();
            await Shell.Current.GoToAsync("///Installation");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Échec de l'installation: {ex.Message}", "OK");
        }
    }

    #region Install_Techno

    static void InstallScoop()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser; Invoke-RestMethod -Uri https://get.scoop.sh | Invoke-Expression\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(psi))
            {
                process?.WaitForExit();
                if (process?.ExitCode != 0)
                {
                    throw new Exception("Échec de l'installation de Scoop");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de l'installation de Scoop: {ex.Message}");
        }
    }

    static void InstallPython()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"scoop install main/python\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(psi))
            {
                process?.WaitForExit();
                if (process?.ExitCode != 0)
                {
                    throw new Exception("Échec de l'installation de Python");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de l'installation de Python: {ex.Message}");
        }
    }

    static void InstallPHP()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"scoop install main/php\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(psi))
            {
                process?.WaitForExit();
                if (process?.ExitCode != 0)
                {
                    throw new Exception("Échec de l'installation de PHP");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de l'installation de PHP: {ex.Message}");
        }
    }

    #endregion



    #region isinstalled
    private bool IsPythonInstalled()
    {
        try
        {
            // Try running "python --version" or "python3 --version" depending on the system
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            process?.WaitForExit();

            // If Python is installed, the process will complete without error
            if (process?.ExitCode == 0)
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

    static bool IsScoopInstalled()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Get-Command scoop -ErrorAction SilentlyContinue\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(psi))
            {
                if (process == null) return false;

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return !string.IsNullOrWhiteSpace(output); // If output exists, Scoop is installed
            }
        }
        catch
        {
            return false; // Error occurred, assuming Scoop is not installed
        }
    }

    private bool IsPhpInstalled()
    {
        try
        {
            // Try running "php --version"
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "php",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            process?.WaitForExit();

            // If PHP is installed, the process will complete without error
            if (process?.ExitCode == 0)
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

    #endregion

    #region github

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

    #endregion

    #region Python_project
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
            await process?.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception("Échec de la création de l'environnement virtuel");
            }

            Debug.WriteLine("Environnement virtuel créé avec succès.");
            await InstallFlask(projectPath);
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
            var processStartInfo = new ProcessStartInfo
            {
                FileName = pipPath,
                Arguments = "install flask",
                WorkingDirectory = projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process? process = Process.Start(processStartInfo);
            await process?.WaitForExitAsync();

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

    #endregion

    #region PHP_project


    private async Task CreateSymfonyProject(string projectName)
    {
        EnableOpenSSLInPhpIni();
        string composer_path= Path.Combine(AppContext.BaseDirectory, "Resources","Files","composer.phar");
        Debug.WriteLine($"php {composer_path} create-project symfony/skeleton {projectName}");
        await createSymfony($"{composer_path} create-project symfony/skeleton {projectName}");
        Console.WriteLine("Symfony project created successfully!");
    }

    static void EnableOpenSSLInPhpIni()
    {
        Console.WriteLine("Locating php.ini file...");

        // Get php.ini location
        ProcessStartInfo iniStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c php --ini",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process iniProcess = new Process { StartInfo = iniStartInfo };
        iniProcess.Start();

        string iniOutput = iniProcess.StandardOutput.ReadToEnd();
        iniProcess.WaitForExit();

        // Parse the output to find php.ini path
        string phpIniPath = "";
        Debug.Write(iniOutput);
        foreach (string line in iniOutput.Split('\n'))
        {
            if (line.Contains("\\php.ini"))
            {
                Debug.WriteLine(line);
                phpIniPath = line.Substring(line.IndexOf(':') + 1).Trim();
                Debug.WriteLine(phpIniPath);
                break;
            }
        }

        if (string.IsNullOrEmpty(phpIniPath))
        {
            Console.WriteLine("Could not find php.ini file. Please check your PHP installation.");
            return;
        }

        Console.WriteLine($"Found php.ini at: {phpIniPath}");

        // Read the php.ini file
        if (File.Exists(phpIniPath))
        {
            string content = File.ReadAllText(phpIniPath);

            // Check if OpenSSL is commented out
            if (content.Contains(";extension=openssl"))
            {
                // Uncomment the OpenSSL extension
                content = content.Replace(";extension=openssl", "extension=openssl");

                // Write the updated content back to php.ini
                try
                {
                    File.WriteAllText(phpIniPath, content);
                    Console.WriteLine("Successfully enabled OpenSSL extension in php.ini");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing to php.ini: {ex.Message}");
                    Console.WriteLine("You may need to run this application as administrator.");
                }
            }
            else if (content.Contains("extension=openssl"))
            {
                Console.WriteLine("OpenSSL extension is already enabled in php.ini");
            }
            else
            {
                Console.WriteLine("Could not find OpenSSL configuration in php.ini.");
                Console.WriteLine("Attempting to add it...");

                try
                {
                    File.AppendAllText(phpIniPath, "\nextension=openssl\n");
                    Console.WriteLine("Added OpenSSL extension to php.ini");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error appending to php.ini: {ex.Message}");
                    Console.WriteLine("You may need to run this application as administrator.");
                }
            }
        }
        else
        {
            Console.WriteLine($"php.ini file not found at {phpIniPath}");
        }
    }

    private async Task createSymfony(string command)
    {
        try
        {

            // Use pip to install Flask
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "php",
                Arguments = $"{command}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            await process?.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception("Échec de l'installation de Symfony");
            }

            Debug.WriteLine("Symfony a été installé avec succès.");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Échec de l'installation de Symfony: {ex.Message}", "OK");
        }
    }

    #endregion

    private async void Button_Retour(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Installation");
    }
    private async void OnSelectFolderClicked(object sender, EventArgs e)
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

}