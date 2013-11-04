using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoRu.Common.Model
{
    public class CachedContent
    {
        [BsonId]
        public string Uri { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
