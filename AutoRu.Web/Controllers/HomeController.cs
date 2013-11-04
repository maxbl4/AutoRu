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
    }
}
