using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoRu.Common;
using AutoRu.Common.Model;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace AutoRu
{
    class DownloadService
    {
        private DateTime lastDownload = DateTime.MinValue;
        private Timer timer;
        bool doingDownload = false;

        public DownloadService()
        {
            timer = new Timer(TimerCallback, null, 5000, 5000);
        }

        void TimerCallback(object state)
        {
            if (doingDownload) return;
            doingDownload = true;
            try
            {
                var task = Global.Db.DownloadTask.AsQueryable()
                    .OrderByDescending(x => x.Timestamp)
                    .FirstOrDefault();
                if (task == null) return;
                DoDownload(task);
            }
            finally {
                doingDownload = false;
            }
        }

        void DoDownload(DownloadTask task)
        {
            var uri = task.Uri.ToLowerInvariant();
            var cacheExpiration = DateTime.MinValue;
            switch (task.Type)
            {
                case DownloadType.Page:
                    cacheExpiration = DateTime.Now.AddMinutes(-5);
                    break;
                case DownloadType.Topic:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var cachedContent = Global.Db.CachedContent.FindOneById(uri);
            string text;
            if (cachedContent.Timestamp < cacheExpiration)
            {
                Global.Db.CachedContent.Remove(Query<CachedContent>.EQ(x => x.Uri, uri));
                cachedContent = null;
            }
            if (cachedContent == null)
            {
                var wc = new WebClient();
                var bytes = wc.DownloadData(uri);
                text = Encoding.UTF8.GetString(bytes);
                Global.Db.CachedContent.Save(new CachedContent { Uri = uri, Content = text, Timestamp = DateTime.Now });
                Global.Db.DownloadTask.Remove(Query<DownloadTask>.EQ(x => x.Uri, task.Uri));
            }
            else
            {
                text = cachedContent.Content;
            }
            switch (task.Type)
            {
                case DownloadType.Page:
                    cacheExpiration = DateTime.Now.AddMinutes(-5);
                    break;
                case DownloadType.Topic:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
