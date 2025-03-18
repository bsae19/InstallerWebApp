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
                //ProcessStartInfo psi = new ProcessStartInfo
                //{
                //    FileName = "powershell",
                //    Arguments = "-Command \"Get-Command scoop -ErrorAction SilentlyContinue\"",
                //    RedirectStandardOutput = true,
                //    UseShellExecute = false,
                //    CreateNoWindow = true
                //};

                //using (Process? process = Process.Start(psi))
                //{
                //    if (process == null) return false;

                //    string output = process.StandardOutput.ReadToEnd();
                //    process.WaitForExit();

                //    return !string.IsNullOrWhiteSpace(output); // If output exists, Scoop is installed
                //}
            }
            catch
            {
                return false; // Error occurred, assuming Scoop is not installed
            }
        }
        public static void InstallScoop()
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
    }
}
