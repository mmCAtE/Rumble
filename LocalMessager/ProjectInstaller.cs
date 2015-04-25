using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace Skyfire
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            ManagementBaseObject inPar = null;
            ManagementClass mc = new ManagementClass("Win32_Service");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo["Name"].ToString() == "LocalMessager")
                {
                    inPar = mo.GetMethodParameters("Change");
                    inPar["DesktopInteract"] = true;
                    mo.InvokeMethod("Change", inPar, null);
                }
            }
        }
    }
}
