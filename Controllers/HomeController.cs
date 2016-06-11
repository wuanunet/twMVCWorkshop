using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using twMVCWorkshop.Models;

namespace twMVCWorkshop.Controllers
{
    public class HomeController : Controller
    {
        private WorkshopEntities db = new WorkshopEntities();

        [OutputCache(CacheProfile = "Home")]
        public ActionResult Index()
        {
            var articles = db.Article.Where(x => x.IsPublish && x.PublishDate <= DateTime.Now)
                .OrderByDescending(x => x.CreateDate)
                .Take(5);

            return View(articles.ToList());
        }

        public ActionResult Articles()
        {
            ViewData["sitemap"] = "我是文章列表";

            return View();
        }


    }

}
