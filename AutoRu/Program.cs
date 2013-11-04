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
        const string baseUri = "http://wwwboards.auto.ru";
        const string mongoConnectionString = "mongodb://mx2";
        
        static void Main(string[] args)
        {
            Global.Initialize(mongoConnectionString);
            foreach (var forum in Global.Db.Forum.AsQueryable().Where(x => x.DoCrawl))
            {
                for (int i = 1; i < 10; i++)
                {
                    var cnt = GetPage(forum.ForumId, i);
                    ParsePage(forum.ForumId, cnt);
                }
            }
        }

        static void ParsePage(string forumId, string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var topics = doc.DocumentNode.SelectNodes("//ul[li]");
            foreach (var topic in topics)
            {
                var posts = topic.SelectNodes("li");
                var index = 0;
                int? topicId = null;
                var parents = new Stack<ParentId>();
                foreach (var post in posts)
                {
                    var msg = new Post(post);
                    msg.ForumId = forumId;
                    if (topicId == null)
                    {
                        topicId = msg.Id;
                    }
                    msg.Index = index++;
                    msg.TopicId = topicId.Value;
                    if (parents.Count > 0)
                    {
                        do
                        {
                            var p = parents.Peek();
                            if (p.Offset < msg.Offset)
                            {
                                msg.ParentId = p.Id;
                                break;
                            }
                            parents.Pop();
                        } while (parents.Any());
                    }
                    parents.Push(new ParentId{ Id = msg.Id, Offset = msg.Offset });
                    Console.WriteLine("{0,-8} {1} {2} {3}{4}{5}[{6}]{7}", 
                        msg.Id,
                        msg.Author,
                        msg.Timestamp,
                        msg.HasImageIcon ? "IMG|" : "", 
                        msg.HasVideoIcon ? "VID|" : "",
                        msg.HasBody ? "BODY|" : "", 
                        msg.Offset,
                        msg.Title);
                    if (msg.Id > 0)
                    {
                        if (msg.HasBody)
                        {
                            msg.Body = GetBody(string.Format("{0}/{1}/{2}/", baseUri, forumId, msg.Id));
                        }
                        Global.Db.Post.Save(msg);
                    }
                }
            }
        }

        private static string GetBody(string uri)
        {
            var content = Download(uri);
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc.DocumentNode.SelectSingleNode("//body").ChildNodes.ToList();
            var startIndex = nodes.FindIndex(x => x.InnerText.Contains("отправленным"));
            if (startIndex < 0) startIndex = nodes.FindIndex(x => x.InnerText.Contains("Отправлено"));
            if (startIndex < 0) return "";
            startIndex = nodes.FindIndex(startIndex, x => x.Name == "b");
            if (startIndex < 0) return "";
            startIndex += 2;
            var endIndex = nodes.FindIndex(startIndex, x => x.Name == "#comment");
            if (endIndex > startIndex + 5) endIndex -= 5;
            else endIndex = startIndex;
            var body = new StringBuilder();
            for (int i = startIndex; i <= endIndex; i++)
            {
                body.AppendLine(nodes[i].OuterHtml);
            }
            return body.ToString();
        }

        static string GetPage(string forumId, int number)
        {
            var uri = string.Format("{0}/{1}/page/{2}/", baseUri, forumId, number);
            return Download(uri, true);
        }

        static string Download(string uri, bool avoidCache = false)
        {
            uri = uri.ToLowerInvariant();
            var cacheItem = Global.Db.CachedContent.FindOneById(uri);
            if (!avoidCache && cacheItem != null) return cacheItem.Content;
            var wc = new WebClient();
            var bytes = wc.DownloadData(uri);
            var text = Encoding.UTF8.GetString(bytes);
            cacheItem = new CachedContent {Uri = uri, Timestamp = DateTime.UtcNow, Content = text};
            Global.Db.CachedContent.Save(cacheItem);
            System.Threading.Thread.Sleep(300);
            return text;
        }
    }

    internal class UriCacheItem
    {

    }

    internal class ParentId
    {
        public int Id { get; set; }
        public int Offset { get; set; }
    }
}
