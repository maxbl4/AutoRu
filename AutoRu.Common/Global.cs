using System;
using System.Configuration;
using System.Data;
using System.Linq;
using AutoRu.Common.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace AutoRu.Common
{
    public class Global
    {
        private static Database db;
        public static Database Db 
        {
            get 
            {
                if (db == null)
                {
                    db = new Database(ConfigurationManager.ConnectionStrings["mongo"].ConnectionString);
                }
                return db;
            }
        }
    }

    public class Database
    {
        private MongoDatabase db;
        private MongoClient cli;

        public Database(string connectionString)
        {
            cli = new MongoClient(connectionString);
            db = cli.GetServer().GetDatabase("autoru");
            CachedContent.EnsureIndex(IndexKeys<CachedContent>.Descending(x => x.Timestamp), IndexOptions.SetTimeToLive(TimeSpan.FromDays(7)));
            Post.EnsureIndex(IndexKeys<Post>.Descending(x => x.Timestamp), IndexOptions.SetTimeToLive(TimeSpan.FromDays(7)));
            Post.EnsureIndex(IndexKeys<Post>
                .Ascending(x => x.ForumId)
                .Descending(x => x.TopicId)
                .Ascending(x => x.Index)
                );
            if (!Forum.AsQueryable().Any())
            {
                Forum.Save(new Forum {ForumId = "moto", DoCrawl = true});
                Forum.Save(new Forum { ForumId = "scooter", DoCrawl = true });
            }

            User.EnsureIndex(IndexKeys<User>.Ascending(x => x.UserName));
            ReadId.EnsureIndex(IndexKeys<ReadId>.Ascending(x => x.UserId).Ascending(x => x.PostId));
            ReadId.EnsureIndex(IndexKeys<ReadId>.Ascending(x => x.UserId).Descending(x => x.Timestamp));
        }

        public MongoCollection<CachedContent> CachedContent { get { return GetCollection<CachedContent>(); } }
        public MongoCollection<DownloadTask> DownloadTask { get { return GetCollection<DownloadTask>(); } }
        public MongoCollection<Post> Post { get { return GetCollection<Post>(); } }
        public MongoCollection<Forum> Forum { get { return GetCollection<Forum>(); } }
        public MongoCollection<User> User { get { return GetCollection<User>(); } }
        public MongoCollection<ReadId> ReadId { get { return GetCollection<ReadId>(); } }

        private MongoCollection<T> GetCollection<T>()
        {
            return db.GetCollection<T>(typeof(T).Name);
        }
    }
}
