﻿using System;
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
        public static bool Execute(string filename,string args="",bool redirect=false,string path=@".\")
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo 
                {
                    FileName = filename,
                    Arguments = args,
                    WorkingDirectory=path,
                    RedirectStandardOutput = redirect,
                    RedirectStandardError = redirect,
                    UseShellExecute = false,
                    CreateNoWindow = false
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing command: {ex.Message}");
                return false; // Error occurred, assuming Scoop is not installed
            }
        }
    }
}
