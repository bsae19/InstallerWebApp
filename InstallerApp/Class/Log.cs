using NLog;

namespace InstallerApp.Class
{
    public class Log
    {
        private Logger _logger;

        public Log(string name = "App")
        {
            // Initialisation du logger avec le nom spécifié
            _logger = LogManager.GetLogger(name);
        }

        // Méthode pour obtenir le logger
        public Logger GetLogger()
        {
            return _logger;
        }
    }
}
