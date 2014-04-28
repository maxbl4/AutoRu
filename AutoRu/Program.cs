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
            //new Crawler().CrawlOnce();
            //return;
            var user = Global.Db.User.AsQueryable().FirstOrDefault(x => x.UserName == "Max");
            if (user == null)
            {
                user = new User{ UserName = "Max" };
                Global.Db.User.Save(user);
            }

            var cufoffDate = new DateTime(2014, 04, 26);

            var readPosts = new HashSet<int>(Global.Db.ReadId.AsQueryable()
                .Where(x => x.Timestamp > cufoffDate)
                .Select(x => x.PostId));
            //var newPosts = Global.Db.Post.AsQueryable()
            //    .Where(x => x.ForumId == "moto" 
            //        && x.Timestamp > cufoffDate
            //        && !readPosts.Contains(x.Id))
            //    .OrderByDescending(x => x.TopicId)
            //    .ThenBy(x => x.Index)
            //    .ToList();
            var posts = Global.Db.Post.AsQueryable()
                .Where(x => x.TopicId == 2529537)
                .OrderBy(x => x.Index)
                .ToList();
            var readIds = new HashSet<int>{
                2529548,
2529551,
2529582,
2529605,
2529585,
2529593,
2529591,
            };
            for (int i = 0; i < posts.Count; i++)
            {
                var post = posts[i];
                if (readIds.Contains(post.Id))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                var skipPost = false;
                if (i > 0 && i < posts.Count - 1)
                {
                    var prevPost = posts[i - 1];
                    var nextPost = posts[i + 1];
                    if (readIds.Contains(prevPost.Id)
                        && readIds.Contains(post.Id)
                        && readIds.Contains(nextPost.Id))
                    {
                        skipPost = true;
                    }
                }
                if (skipPost != true)
                    Console.WriteLine("{1} {0}{2}", new String(' ', post.Offset), post.Id, post.Title);
                else
                {
                    //Console.WriteLine("...");
                }
            }
            Console.ReadLine();
        }
    }
}
