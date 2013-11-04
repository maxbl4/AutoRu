using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoRu.Common.Model
{
    public class DownloadTask
    {
        [BsonId]
        public string Uri { get; set; }
        public DateTime Timestamp { get; set; }
        public DownloadType Type { get; set; }
    }

    public enum DownloadType
    { 
        Page,
        Topic
    }
}
