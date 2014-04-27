using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AutoRu.CrawlerSvc
{
    static class Program
    {
        private const string InstallUtilPath = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Contains("--console"))
            {
                new CrawlerService().Start();
                Console.WriteLine("Started in console mode, press enter to close.");
                Console.ReadLine();
                return;
            }
            var exeName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (args.Contains("--install"))
            {
                System.Diagnostics.Process.Start(InstallUtilPath, string.Format("\"{0}\" /i", exeName));
                return;
            }
            if (args.Contains("--uninstall"))
            {
                System.Diagnostics.Process.Start(InstallUtilPath, string.Format("\"{0}\" /u", exeName));
                return;
            }
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new CrawlerService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
