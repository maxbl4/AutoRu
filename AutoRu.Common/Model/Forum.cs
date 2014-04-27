using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoRu.Common.Model
{
    public class Forum
    {
        [BsonId]
        public string ForumId { get; set; }
        public bool DoCrawl { get; set; }
    }

    public class User
    {
        [BsonId]
        public BsonObjectId Id  { get; set; }
        public string UserName { get; set; }
    }

    public class ReadId
    {
        public BsonObjectId UserId { get; set; }
        public int PostId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
