using System.Xml.Linq;
using NLog;

namespace InstallerApp.Class
{
    public class Log
    {
        public Logger GetLogger(string name = "App")
        {
            return LogManager.GetLogger(name);
        }
    }
}
