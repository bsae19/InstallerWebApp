using InstallerApp.Composant;

namespace InstallerApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            List<KeyValuePair<string, ContentView>> routes = new List<KeyValuePair<string, ContentView>>()
            {
                new KeyValuePair<string, ContentView>("Demarrage", new Demarrage()),
                new KeyValuePair<string, ContentView>("Installation", new Composant.Installation()),
                new KeyValuePair<string, ContentView>("InstallTechno", new InstallTechno()),
                new KeyValuePair<string, ContentView>("SelectFramework", new SelectFramework()),
                //new KeyValuePair<string, ContentView>("Demarrage", new Composant.Demarrage()),
                //new KeyValuePair<string, string>("Installation", "Installation"),
                //new KeyValuePair<string, string>("Configuration", "Configuration"),
                //new KeyValuePair<string, string>("Finalisation", "Finalisation"),
                //new KeyValuePair<string, string>("Exit", "Exit")
            };
            foreach (var item in routes)
            {
                var contentView = new Composant.Page(item.Value);
                var contentTemplate = new DataTemplate(() => contentView);

                var demarrageContent = new ShellContent
                {
                    Title = item.Key,
                    ContentTemplate = contentTemplate,
                    Route = item.Key
                };

                // Add it to Shell
                this.Items.Add(demarrageContent);
            }
            
        }
    }
}
