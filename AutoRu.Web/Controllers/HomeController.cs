using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoRu.Common;
using Microsoft.AspNet.Identity;
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
            var readIds = new HashSet<int>();
            var userName = User.Identity.GetUserName();
            if (!string.IsNullOrEmpty(userName))
            {
                var user = Global.Db.User.AsQueryable().FirstOrDefault(x => x.UserName == userName);
                if (user != null)
                {
                    readIds.UnionWith(Global.Db.ReadId.AsQueryable().Where(x => x.UserId == user.Id).Select(x => x.PostId));
                }
            }

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
