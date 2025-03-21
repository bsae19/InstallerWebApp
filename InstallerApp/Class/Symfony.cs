using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallerApp.Composant;

namespace InstallerApp.Class
{
    internal class Symfony
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
                            "PHP ne peut pas être installé directement depuis l'application sur iOS.\n" +
                            "Veuillez installer PHP, composer et symfony manuellement sur votre machine.\n\n" +
                            "1. Installez PHP avec la commande :\n" +
                            "   brew install php\n" +
                            "2. Installez Composer avec la commande :\n" +
                            "   php -r \"copy('https://getcomposer.org/installer', 'composer-setup.php');\"\n" +
                            "   php composer-setup.php\n" +
                            "   php -r \"unlink('composer-setup.php');\"\n" +
                            "3. Installez Symfony avec la commande :\n" +
                            "   composer create-project symfony/skeleton my_project_name",
                            "OK");
                    await sh.Navigation.PopModalAsync();
                    return false;
                }

                bool answer = true;
                bool isSymfony = true;
                bool isScoop = true;
                bool isPHP = true;
                bool isComposer = true;

                progressPopup.UpdateStatus("Vérification de Symfony...");

                try
                {
                    isSymfony = Cmd.Execute("symfony", "version");
                    isScoop = Scoop.IsScoopInstalled();
                    isPHP = Cmd.Execute("php", "--version");
                    isComposer = Cmd.Execute("powershell", "-Command \"Get-Command composer -ErrorAction SilentlyContinue\"", true);


                    if (!isSymfony || !isPHP || !isComposer)
                    {
                        Debug.WriteLine(isSymfony.ToString() + isPHP.ToString() + isComposer.ToString());
                        answer = await sh.DisplayAlert("Installation", "Symfony n'est pas installé. Voulez-vous l'installer ?", "Oui", "Non");
                        if (answer)
                        {
                            progressPopup.UpdateStatus("Installation de Scoop...");
                            if (!isScoop)
                            {
                                Scoop.InstallScoop();
                            }
                            if (!isPHP)
                            {
                                progressPopup.UpdateStatus("Installation de PHP...");
                                if (!Scoop.InstallPackage("main/php"))
                                    throw new Exception("Échec de l'installation de PHP");
                            }
                            if (!isComposer)
                            {
                                progressPopup.UpdateStatus("Installation de PHP...");
                                if (!Scoop.InstallPackage("main/composer"))
                                    throw new Exception("Échec de l'installation de PHP");
                            }

                            progressPopup.UpdateStatus("Installation de Symfony...");
                            if (!Scoop.InstallPackage("main/symfony-cli"))
                                throw new Exception("Échec de l'installation de Symfony");

                            isSymfony = true;
                            isPHP = true;
                            isComposer = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await sh.DisplayAlert("Erreur", $"Erreur lors de l'installation de Symfony: {ex.Message}", "OK");
                }
                if (isSymfony && isPHP && isComposer)
                {
                    EnableOpenSSLInPhpIni();
                    string action = await sh.DisplayActionSheet("ActionSheet: Que voulez-vous construire?", null, null, "WebApp (Application web)", "microservice (API,console application)","Learning Demo");
                    await Task.Delay(500);
                    switch (action)
                    {
                        case "WebApp (Application web)":
                            if (!Cmd.Execute("symfony", "new " + projectPath + " --webapp" ))
                            {
                                throw new Exception("Échec de la création du projet");
                            }
                            break;
                        case "microservice (API,console application)":
                            if (!Cmd.Execute("symfony", "new " + projectPath))
                            {
                                throw new Exception("Échec de la création du projet");
                            }
                            break;
                        case "Learning Demo":
                            if (!Cmd.Execute("symfony", "new " + projectPath + " --demo"))
                            {
                                throw new Exception("Échec de la création du projet");
                            }
                            break;
                    }
                    Debug.WriteLine("symfony", "new " + projectPath + (action == "WebApp (Application web)" ? " --webapp" : ""));
                    if(!Cmd.Execute("symfony", "new " + projectPath+(action== "WebApp (Application web)"?" --webapp" : "")))
                    {
                        throw new Exception("Échec de la création du projet");
                    }
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
    }
}
