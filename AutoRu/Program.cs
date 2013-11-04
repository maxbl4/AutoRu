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
        const string MongoConnectionString = "mongodb://mx2";
        static void Main(string[] args)
        {
            Global.Initialize(MongoConnectionString);
            new Crawler().CrawlOnce();
        }
    }
}
