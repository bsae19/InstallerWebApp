using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallerApp.Composant;
using Microsoft.Maui.Storage;

namespace InstallerApp.Class
{
    class Vite
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

                progressPopup.UpdateStatus("Vérification de React...");

                
                bool isNode=await React.Install(sh, progressPopup);

                if (isNode)
                {
                    progressPopup.UpdateStatus("Création du projet...");
                    string? parentPath = Path.GetDirectoryName(projectPath);
                    string? projectName = Path.GetFileName(projectPath);
                    Debug.WriteLine($"Parent Path: {parentPath}");
                    string args = $"create-vite@latest {projectName.ToLower()} --template react-ts";
                    Cmd.Execute("cmd.exe", "/c npx "+ args, false, parentPath ?? @".\");
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
