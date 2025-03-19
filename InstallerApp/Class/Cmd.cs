using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace InstallerApp.Class
{
    internal class Cmd
    {
        public static bool Execute(string filename,string args="",bool redirect=false)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo 
                {
                    FileName = filename,
                    Arguments = args,
                    RedirectStandardOutput = redirect,
                    RedirectStandardError = redirect,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process? process = Process.Start(psi))
                {
                    if (process == null) return false;
                    if (redirect)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();
                        return !string.IsNullOrWhiteSpace(output);
                    }
                    else
                    {
                        process.WaitForExit();
                        return process.ExitCode == 0;
                    }
                    // If output exists, Scoop is installed
                }
            }
            catch
            {

                return false; // Error occurred, assuming Scoop is not installed
            }
        }
    }
}
