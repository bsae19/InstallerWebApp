using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallerApp.Composant;

namespace InstallerApp.Class
{
    class React
    {
        public static async Task<bool> Install(Shell sh, ProgressPopup progressPopup)
        {
            try
            {
                bool isNode = Cmd.Execute("node", "-v");
                bool isNpm = Cmd.Execute("cmd.exe", "/c npm --version", true);
                Debug.WriteLine($"Node: {isNode}, Npm: {isNpm}");
                bool isScoop = Scoop.IsScoopInstalled();

                if (!isNode || !isNpm)
                {
                    bool answer = await sh.DisplayAlert("Installation", (!isNode?!isNpm?"Node et npm":"Node":"npm")+" n'est pas installé. Voulez-vous l'installer ?", "Oui", "Non");
                    if (!answer)
                    {
                        return false;
                    }
                    progressPopup.UpdateStatus("Installation de Scoop...");
                    if (!isScoop)
                    {
                        Scoop.InstallScoop();
                    }

                    progressPopup.UpdateStatus("Installation de Node...");
                    if (!Scoop.InstallPackage("main/nodejs"))
                        throw new Exception("Échec de l'installation de Node");
                }
                return true;
            }
            catch (Exception ex)
            {
                await sh.DisplayAlert("Erreur", $"Erreur lors de l'installation de Flask: {ex.Message}", "OK");
                return false;
            }
        }
    }
}
