using System.Diagnostics;
using InstallerApp.Composant;

namespace InstallerApp.Class
{
    internal class Django
    {
        public async Task<bool> Install(string projectPath, Shell sh)
        {
            var progressPopup = new ProgressPopup();
            await sh.Navigation.PushModalAsync(progressPopup);
            await Task.Delay(500);

            try
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    await sh.DisplayAlert("Instructions",
                    "Django ne peut pas être installé directement depuis l'application sur iOS.\n" +
                    "Veuillez installer Python et Django manuellement sur votre machine.\n\n" +
                    "1. Créez un environnement virtuel avec la commande :\n" +
                    "   python -m venv venv\n" +
                    "2. Installez Django avec la commande :\n" +
                    "   pip install django",
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
                    await sh.DisplayAlert("Erreur", $"Erreur lors de l'installation de Python: {ex.Message}", "OK");
                }

                if (!answer)
                {
                    return false;
                }

                if (isPython)
                {

                    progressPopup.UpdateStatus("Installation de Flask...");
                    string venvScriptsPath = Path.Combine(projectPath, "venv", "Scripts");
                    string pipPath = Path.Combine(venvScriptsPath, "pip.exe");
                    Cmd.Execute("python", "-m pip install django");
                    progressPopup.UpdateStatus("Création du projet...");
                    Cmd.Execute("django-admin", $"startproject {Path.GetFileName(projectPath)}", path: Path.GetDirectoryName(projectPath) ?? @".\");
                    if (Cmd.Execute("python", $"manage.py startapp myapp",path:projectPath))
                    {
                        Debug.WriteLine("Création du projetkjkkkhhhhhhhhhhhhhhhhhhhhhhhh...");
                        await sh.DisplayAlert("Succès", "Le projet a été créé avec succès", "OK");
                        return true;
                    }
                }
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
