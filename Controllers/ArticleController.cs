using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using twMVCWorkshop.Models;

namespace twMVCWorkshop.Controllers
{
    public class ArticleController : Controller
    {
        private WorkshopEntities db = new WorkshopEntities();

        private List<Category> _categories = new List<Category>();
        public List<Category> Categories
        {
            get
            {
                if (_categories.Count.Equals(0))
                {
                    _categories = db.Category.OrderBy(x => x.CreateDate).ToList();
                }
                return _categories;
            }
        }

        private int pageSize = 5;

        //public ActionResult Index(Guid? categoryID, int page = 1)
        //{
        //    ViewBag.CategoryID = !categoryID.HasValue ? "" : categoryID.ToString();
        //    ViewBag.ArticleCategories = new SelectList(this.Categories, "ID", "Name", categoryID);

        //    int pageIndex = page < 1 ? 1 : page;

        //    var articles = db.Article.Where(x => x.IsPublish && x.PublishDate <= DateTime.Now);

        //    if (categoryID.HasValue)
        //    {
        //        articles = db.Article.Where(x => x.CategoryID == categoryID.Value)
        //            .OrderByDescending(x => x.CreateDate);
        //    }
        //    else
        //    {
        //        articles = db.Article.OrderByDescending(x => x.CreateDate);
        //    }

        //    return View(articles.ToPagedList(pageIndex, pageSize));
        //}

        ///// <summary>
        ///// 熱門文章列表
        ///// </summary>
        ///// <param name="categoryID">The category ID.</param>
        ///// <param name="page">The page.</param>
        ///// <returns></returns>
        //public ActionResult Hot(Guid? categoryID, int page = 1)
        //{
        //    ViewBag.CategoryID = !categoryID.HasValue ? "" : categoryID.ToString();
        //    ViewBag.ArticleCategories = new SelectList(this.Categories, "ID", "Name", categoryID);

        //    int pageIndex = page < 1 ? 1 : page;

        //    var articles = db.Article.Where(x => x.IsPublish && x.PublishDate <= DateTime.Now);

        //    if (categoryID.HasValue)
        //    {
        //        articles = articles
        //            .Where(x => x.CategoryID == categoryID.Value)
        //            .OrderByDescending(x => x.ViewCount);
        //    }
        //    else
        //    {
        //        articles = articles.OrderByDescending(x => x.ViewCount);
        //    }

        //    return View(articles.ToPagedList(pageIndex, pageSize));
        //}

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public ActionResult Index(string categoryName = null, int page = 1)
        {
            ViewBag.CategoryName = categoryName;

            ViewBag.ArticleCategories = new SelectList(this.Categories, "Name", "Name", categoryName);

            int pageIndex = page < 1 ? 1 : page;

            var articles = db.Article.Where(x => x.IsPublish && x.PublishDate <= DateTime.Now);

            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = db.Category.FirstOrDefault(x => x.Name == categoryName);

                articles = articles.Where(x => x.CategoryID == category.ID);
            }

            articles = articles.OrderByDescending(x => x.CreateDate);

            return View(articles.ToPagedList(pageIndex, pageSize));
        }

        /// <summary>
        /// 熱門文章列表
        /// </summary>
        /// <param name="categoryName">The category ID.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public ActionResult Hot(string categoryName, int page = 1)
        {
            ViewBag.CategoryName = categoryName;
            ViewBag.ArticleCategories = new SelectList(this.Categories, "Name", "Name", categoryName);

            int pageIndex = page < 1 ? 1 : page;

            var articles = db.Article.Where(x => x.IsPublish && x.PublishDate <= DateTime.Now);

            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = db.Category.FirstOrDefault(x => x.Name == categoryName);

                articles = articles.Where(x => x.CategoryID == category.ID);
            }

            articles = articles.OrderByDescending(x => x.ViewCount);

            return View(articles.ToPagedList(pageIndex, pageSize));
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="year">The category name.</param>
        /// <param name="month">The category name.</param>
        /// <param name="day">The category name.</param>
        /// <param name="categoryName">The category name.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        //[OutputCache(CacheProfile = "Archive")]
        public ActionResult Archive(int year, int? month, int? day, string categoryName, int page = 1)
        {
            ViewBag.ArticleCategories = this.Categories;

            var dateStart = DateTime.Now;
            var dateEnd = dateStart;

            if (day.HasValue)
            {
                dateStart = DateTime.Parse(year + "/" + month + "/" + day);
                dateEnd = dateStart.AddDays(1);

                ViewBag.BreadcrumbCurrent = string.Concat(year, "年", month, "月", day, "日，所有", categoryName, "文章");
            }
            else if (month.HasValue)
            {
                dateStart = DateTime.Parse(year + "/" + month + "/1");
                dateEnd = dateStart.AddMonths(1);

                ViewBag.BreadcrumbCurrent = string.Concat(year, "年", month, "月，所有", categoryName, "文章");
            }
            else
            {
                dateStart = DateTime.Parse(year + "/1/1");
                dateEnd = dateStart.AddYears(1);

                ViewBag.BreadcrumbCurrent = string.Concat(year, "年，所有", categoryName, "文章");
            }

            var articles = db.Article
                                .Where(x => x.IsPublish
                                            && x.PublishDate >= dateStart
                                            && x.PublishDate < dateEnd);

            if (!string.IsNullOrEmpty(categoryName))
            {
                articles = articles.Where(x => x.Category.Name == categoryName);
            }

            articles = articles.OrderByDescending(x => x.PublishDate);

            return View(articles.ToPagedList(Math.Max(1, page), pageSize));
        }

        /// <summary>
        /// 文章明細
        /// </summary>
        /// <param name="year">The category name.</param>
        /// <param name="month">The category name.</param>
        /// <param name="day">The category name.</param>
        /// <param name="categoryName">The category name.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public ActionResult SeoDetails(int year, int month, int day, string categoryName, string subject)
        {
            var dateStart = DateTime.Parse(year + "/" + month + "/" + day).Date;
            var dateEnd = dateStart.AddDays(1);

            var article = db.Article.FirstOrDefault(x => x.IsPublish
                                                         && x.PublishDate >= dateStart
                                                         && x.PublishDate < dateEnd
                                                         && x.Subject == subject);

            this.IncreaseViewCount(article);

            return View("~/Views/Article/Details.cshtml", article);
        }

        public ActionResult Details(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var article = db.Article.FirstOrDefault(x => x.ID == id.Value);
            this.IncreaseViewCount(article);

            return View(article);
        }

        /// <summary>
        /// 查詢結果
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public ActionResult Search(string keyword, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index");
            }

            ViewBag.Keyword = keyword;
            int pageIndex = page < 1 ? 1 : page;

            //查詢欄位：文章標題、文章摘要、文章內容
            var query = db.Article.Where(x => x.IsPublish 
                                         && x.PublishDate <= DateTime.Now);
            query = query.Where(x => 
                                x.Subject.Contains(keyword) 
                                || x.Summary.Contains(keyword) 
                                || x.ContentText.Contains(keyword))
                   .OrderByDescending(x => x.CreateDate);

            return View(query.ToPagedList(pageIndex, pageSize));
        }

        public ActionResult ArticlePhoto(Guid id, int w, int h)
        {
            var photo = db.Photo.FirstOrDefault(x => x.ID == id);

            var filePath = Server.MapPath("~/Uploads/" + photo.ArticleID + "/" + photo.FileName);

            var image = new WebImage(filePath).Resize(w, h);

            return File(image.GetBytes(), "image/jpeg");
        }

        /// <summary>
        /// 增加文章瀏覽次數
        /// </summary>
        /// <param name="article">The article.</param>
        private void IncreaseViewCount(Article article)
        {
            article.ViewCount += 1;
            db.Entry(article).State = EntityState.Modified;
            db.SaveChanges();
        }

    }
}
