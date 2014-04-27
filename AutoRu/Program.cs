using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoRu.Common;
using AutoRu.Common.Model;
using HtmlAgilityPack;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AutoRu
{
    class Program
    {
        static void Main(string[] args)
        {
            new Crawler().CrawlOnce();
            return;
            var user = Global.Db.User.AsQueryable().FirstOrDefault(x => x.UserName == "Max");
            if (user == null)
            {
                user = new User{ UserName = "Max" };
                Global.Db.User.Save(user);
            }

            var cufoffDate = new DateTime(2014, 03, 26);

            var readPosts = new HashSet<int>(Global.Db.ReadId.AsQueryable()
                .Where(x => x.Timestamp > cufoffDate)
                .Select(x => x.PostId));
            var newPosts = Global.Db.Post.AsQueryable()
                .Where(x => x.ForumId == "moto" 
                    && x.Timestamp > cufoffDate
                    && !readPosts.Contains(x.Id))
                .OrderByDescending(x => x.TopicId)
                .ThenBy(x => x.Index)
                .ToList();
        }
    }
}
