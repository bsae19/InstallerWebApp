using System.Diagnostics;
using InstallerApp.Composant;

namespace InstallerApp.Class
{
    internal class Flask
    {
        public async Task<bool> Install(string projectPath, Shell sh)
        {
            var progressPopup = new ProgressPopup();
            await sh.Navigation.PushModalAsync(progressPopup);

            try
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    await sh.DisplayAlert("Instructions",
                    "Flask ne peut pas être installé directement depuis l'application sur iOS.\n" +
                    "Veuillez installer Python et Flask manuellement sur votre machine.\n\n" +
                    "1. Créez un environnement virtuel avec la commande :\n" +
                    "   python -m venv venv\n" +
                    "2. Installez Flask avec la commande :\n" +
                    "   pip install flask",
                    "OK");
                    await sh.Navigation.PopModalAsync();
                    return false;
                }

                bool answer = true;
                bool isPython = true;

                progressPopup.UpdateStatus("Vérification de Python...");

                try
                {
                    isPython = Cmd.Execute("python", "--version");
                    bool isScoop = Scoop.IsScoopInstalled();

                    if (!isPython)
                    {
                        answer = await sh.DisplayAlert("Installation", "Python n'est pas installé. Voulez-vous l'installer ?", "Oui", "Non");
                        if (answer)
                        {
                            progressPopup.UpdateStatus("Installation de Scoop...");
                            if (!isScoop)
                            {
                                Scoop.InstallScoop();
                            }

                            progressPopup.UpdateStatus("Installation de Python...");
                            if (!Scoop.InstallPackage("main/python"))
                                throw new Exception("Échec de l'installation de Python");

                            isPython = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await sh.DisplayAlert("Erreur", $"Erreur lors de l'installation de Flask: {ex.Message}", "OK");
                }

                if (!answer)
                {
                    bool createProject = await sh.DisplayAlert("Installation", "Un projet peut être créé, voulez-vous le créer ?", "Oui", "Non");
                    if (!createProject)
                    {
                        await sh.Navigation.PopModalAsync();
                        return false;
                    }
                }

                progressPopup.UpdateStatus("Création du projet...");
                Directory.CreateDirectory(projectPath);

                string repositoryUrl = $"https://github.com/bsae19/python_flask.git";
                bool cloned = GitUtils.CloneRepositoryFromGitHub(repositoryUrl, projectPath);

                if (isPython)
                {
                    progressPopup.UpdateStatus("Configuration de l'environnement virtuel...");
                    Cmd.Execute("python", "-m venv " + Path.Combine(projectPath, "venv"), true);

                    progressPopup.UpdateStatus("Installation de Flask...");
                    string venvScriptsPath = Path.Combine(projectPath, "venv", "Scripts");
                    string pipPath = Path.Combine(venvScriptsPath, "pip.exe");
                    Cmd.Execute(pipPath, "install flask");
                }

                await sh.DisplayAlert("Succès", "Le projet a été créé avec succès", "OK");
            }
            catch (Exception ex)
            {
                await sh.DisplayAlert("Erreur", $"Erreur: {ex.Message}", "OK");
            }
            finally
            {
                await sh.Navigation.PopModalAsync();
            }
            return true;
        }

    }
}
