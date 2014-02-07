using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace AutoRu.CrawlerSvc
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            if (System.Diagnostics.EventLog.SourceExists(CrawlerService.LogName))
                System.Diagnostics.EventLog.CreateEventSource(CrawlerService.LogName, CrawlerService.LogName);
        }

        public override void Uninstall(IDictionary savedState)
        {
            if (System.Diagnostics.EventLog.SourceExists(CrawlerService.LogName))
                System.Diagnostics.EventLog.DeleteEventSource(CrawlerService.LogName);
            base.Uninstall(savedState);
        }
    }
}
