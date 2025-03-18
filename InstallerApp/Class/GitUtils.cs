using System.Diagnostics;
using LibGit2Sharp;

namespace InstallerApp.Class
{
    internal class GitUtils
    {

        public static bool CloneRepositoryFromGitHub(string repositoryUrl, string localPath)
        {
            try
            {
                // Assurez-vous d'avoir installé une bibliothèque Git comme LibGit2Sharp ou utilisez la commande Git via Process
                using (var repo = new LibGit2Sharp.Repository(Repository.Clone(repositoryUrl, localPath)))
                {
                    // Si le clonage est réussi
                    Debug.WriteLine($"Le projet {repositoryUrl} a été cloné avec succès à {localPath}");
                }
                return true;
            }
            catch (Exception ex)
            {
                // Affiche une erreur si le clonage échoue
                Debug.WriteLine($"Erreur lors du clonage du dépôt {repositoryUrl}: {ex.Message}");
                return false;
            }
        }
    }
}
