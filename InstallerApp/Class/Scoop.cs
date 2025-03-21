using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerApp.Class
{
    internal class Scoop
    {
        public static bool IsScoopInstalled()
        {
            try
            {
                return Cmd.Execute("powershell", "-Command \"Get-Command scoop -ErrorAction SilentlyContinue\"", true);
            }
            catch
            {
                return false;
            }
        }
        public static void InstallScoop()
        {
            try
            {
                Cmd.Execute("powershell", "-Command \"Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser; Invoke-RestMethod -Uri https://get.scoop.sh | Invoke-Expression\"");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de l'installation de Scoop: {ex.Message}");
            }
        }
        public static bool InstallPackage(string packageName)
        {
            try
            {
                return Cmd.Execute("powershell", $"-Command \"scoop install {packageName}\"");
            }
            catch
            {
                return false;
            }
        }
    }
}
