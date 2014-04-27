using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoRu.Common;
using MongoDB.Driver.Linq;

namespace AutoRu.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var posts = Global.Db.Post.AsQueryable()
                .OrderByDescending(x => x.TopicId)
                .ThenBy(x => x.Index)
                .Take(20)
                .ToList();
            return View(posts);
        }

        public ActionResult Forum(string id)
        {
            ViewBag.ForumId = id;
            var posts = Global.Db.Post.AsQueryable()
                .Where(x => x.ForumId == id)
                .OrderByDescending(x => x.TopicId)
                .ThenBy(x => x.Index)
                .Take(100)
                .ToList();
            return View(posts);
        }
    }
}
