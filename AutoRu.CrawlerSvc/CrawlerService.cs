using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using AutoRu.Common;

namespace AutoRu.CrawlerSvc
{
    public partial class CrawlerService : ServiceBase
    {
        public const string LogName = "AutoRuCrawler";
        private Crawler crawler;
        private Thread schedulerThread;
        private bool serviceIsStopping = false;
        private DateTime lastCrawl = DateTime.MinValue;
        private EventLog log;
        public CrawlerService()
        {
            InitializeComponent();
        }

        public void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            log = new EventLog(LogName);
            log.Source = LogName;
            log.WriteEntry("Starting");
            crawler = new Crawler();
            schedulerThread = new Thread(Scheduler);
            schedulerThread.Start();
        }

        private void Scheduler()
        {
            while (!serviceIsStopping)
            {
                if ((DateTime.Now - lastCrawl).TotalSeconds > 30)
                {
                    DoCrawlCycle();
                    lastCrawl = DateTime.Now;
                }
                Thread.Sleep(2000);
            }
        }

        private void DoCrawlCycle()
        {
            try
            {
                if (serviceIsStopping) return;
                crawler.CrawlOnce();
                log.WriteEntry("Done crawling");
            }
            catch (Exception ex)
            {
                log.WriteEntry("Error in crawling: " + ex.ToString());
            }
        }

        protected override void OnStop()
        {
            serviceIsStopping = true;
            log.WriteEntry("Stopping");
        }
    }
}
