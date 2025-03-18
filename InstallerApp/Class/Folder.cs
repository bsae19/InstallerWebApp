using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Storage;

namespace InstallerApp.Class
{
    internal partial class Folder
    {
        public static async Task<string?> GetFolder() // Le type de retour doit être Task<string?>
        {
            try
            {
                    var folderResult = await FolderPicker.PickAsync(cancellationToken: CancellationToken.None);
                    if (folderResult.IsSuccessful)
                    {
                        return folderResult.Folder.Path;
                    }
            }
            catch (Exception ex)
            {
                // Enregistrer l'erreur dans les logs
                new Log().GetLogger().Error($"Impossible de sélectionner le dossier: {ex.Message}");
                await Shell.Current.GoToAsync("//Installation"); // Retourner à l'écran d'installation en cas d'erreur
            }
            return null; // Retourner null en cas d'échec ou de non-sélection
        }
    }
}
