using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoRu.Common.Model
{
    public class Forum
    {
        [BsonId]
        public string ForumId { get; set; }
        public bool DoCrawl { get; set; }
    }
}
