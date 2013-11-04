using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace AutoRu.Common.Model
{
    public class Post
    {
        private int id;
        private DateTime timestamp;

        public Post(HtmlNode source)
        {
            GetOffset(source);
            HasImageIcon = source.SelectSingleNode("a[1]/img[@src='http://img.auto.ru/photo.gif']") != null;
            HasVideoIcon = source.SelectSingleNode("a[1]/img[@src='http://i.auto.ru//wwwboards/img/video.gif']") != null;
            HtmlNode titleNode;
            if (HasImageIcon || HasVideoIcon)
                titleNode = source.SelectSingleNode("a[2]");
            else
                titleNode = source.SelectSingleNode("a[1]");
            if (titleNode != null)
            {
                var t = titleNode.InnerText;
                var parts = t.Split(new char[] { '\r', '\n' });
                HasBody = parts.Last().EndsWith("(+)");
                Title = string.Join((string) " ", (IEnumerable<string>) parts.Take(parts.Length - 1).Where(x => !string.IsNullOrWhiteSpace(x)));
                var idString = titleNode.GetAttributeValue("href", "");
                idString = Regex.Replace(idString, @"[^\d]", "");
                int.TryParse(idString, out id);
            }
            var authorNode = source.SelectSingleNode("b");
            if (authorNode != null) Author = authorNode.InnerText;
            var timestampNode = source.SelectSingleNode("i");
            if (timestampNode != null)
            {
                DateTime.TryParse(timestampNode.InnerText, out timestamp);
            }
        }

        void GetOffset(HtmlNode source)
        {
            var style = source.GetAttributeValue("style", null);
            if (string.IsNullOrEmpty(style)) return;
            var styles = style.ToLowerInvariant().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in styles)
            {
                var pair = s.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (pair.Length != 2) continue;
                var key = pair[0].Trim();
                if (key == "margin-left")
                { 
                    var value = Regex.Replace(pair[1], @"[^\d]*", "");
                    int i;
                    if (int.TryParse(value, out i)) Offset = i / 40;
                }
            }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public int TopicId { get; set; }
        public int ParentId { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public bool HasImageIcon { get; set; }
        public bool HasVideoIcon { get; set; }
        public bool HasBody { get; set; }
        public int Offset { get; set; }
        public int Index { get; set; }
        public string ForumId { get; set; }
    }
}