using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Pushover.Twincat
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = "PushoverTwincatService",
                DisplayName = "Pushover Twincat Service",
                StartType = ServiceStartMode.Automatic,
            };

            Installers.AddRange(new Installer[] { serviceProcessInstaller, serviceInstaller });
        }
    }
}