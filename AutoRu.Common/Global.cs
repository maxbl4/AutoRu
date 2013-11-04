using System;
using AutoRu.Common.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace AutoRu.Common
{
    public class Global
    {
        public static Database Db { get; private set; }

        public static void Initialize(string connectionString)
        {
            Db = new Database(connectionString);
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
        }

        public MongoCollection<CachedContent> CachedContent { get { return GetCollection<CachedContent>(); } }
        public MongoCollection<DownloadTask> DownloadTask { get { return GetCollection<DownloadTask>(); } }
        public MongoCollection<Post> Post { get { return GetCollection<Post>(); } }
        public MongoCollection<Forum> Forum { get { return GetCollection<Forum>(); } }

        private MongoCollection<T> GetCollection<T>()
        {
            return db.GetCollection<T>(typeof(T).Name);
        }
    }
}
