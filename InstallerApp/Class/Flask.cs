using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;

namespace InstallerApp.Class
{
    internal class Flask
    {
        public async void Install(string projectPath,Shell sh)
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Show message for iOS users (cannot install Flask directly on iOS)
                await sh.DisplayAlert("Instructions",
                    "Flask ne peut pas être installé directement depuis l'application sur iOS.\n" +
                    "Veuillez installer Python et Flask manuellement sur votre machine.\n\n" +
                    "1. Créez un environnement virtuel avec la commande :\n" +
                    "   python -m venv venv\n" +
                    "2. Installez Flask avec la commande :\n" +
                    "   pip install flask",
                    "OK");
                return;
            }
            //sh.ShowPopup
            Directory.CreateDirectory(projectPath);

            // Clone le dépôt Flask si vous avez un dépôt GitHub à cloner
            string repositoryUrl = $"https://github.com/bsae19/python_flask.git";
            bool cloned = GitUtils.CloneRepositoryFromGitHub(repositoryUrl, projectPath);
            if (cloned)
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // Show message for iOS users (cannot install Flask directly on iOS)
                    _ = new Page().DisplayAlert("Instructions",
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
                        bool isScoop = Scoop.IsScoopInstalled();
                        bool installPython = await new Page().DisplayAlert(
                            "Python non installé",
                            "Python n'est pas installé sur ce système." + (isScoop ? "" : "\n(cela entrainera l'installation du service scoop)") + "\nVoulez-vous l'installer maintenant ?",
                            "Oui", "Non"
                        );

                        if (installPython)
                        {
                            if (!isScoop)
                            {
                                Scoop.InstallScoop();
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
                await new Page().DisplayAlert("Erreur Git", $"Erreur lors du clonage du dépôt", "OK");
            }
        }
        private void InstallPython()
        {
            // Installe Python avec scoop
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c scoop install python",
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }
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
                await new Page().DisplayAlert("Erreur", $"Échec de la création de l'environnement virtuel: {ex.Message}", "OK");
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
                await new Page().DisplayAlert("Erreur", $"Échec de l'installation de Flask: {ex.Message}", "OK");
            }
        }
    }
}
