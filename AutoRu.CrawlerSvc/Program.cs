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
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new CrawlerService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
