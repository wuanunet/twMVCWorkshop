using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using twMVCWorkshop.Areas.Backend.Models;
using twMVCWorkshop.Helpers;
using twMVCWorkshop.Models;
using twMVCWorkshop.ViewModels;

namespace twMVCWorkshop.Areas.Backend.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private WorkshopEntities db = new WorkshopEntities();

        //
        // GET: /Backend/Article/

        public ActionResult Index(QueryOption<ArticleViewModel> queryOption)
        {
            var article = db.Article.Include(a => a.Category)
                .Select(x => new ArticleViewModel
                {
                    ID = x.ID,
                    CategoryID = x.CategoryID,
                    Category = x.Category,
                    Subject = x.Subject,
                    Summary = x.Summary,
                    ContentText = x.ContentText,
                    IsPublish = x.IsPublish,
                    ViewCount = x.ViewCount,
                    PublishDate = x.PublishDate,
                    CreateUser = x.CreateUser,
                    CreateDate = x.CreateDate,
                    UpdateUser = x.UpdateUser,
                    UpdateDate = x.UpdateDate
                });

            if (!string.IsNullOrEmpty(queryOption.Keyword))
            {
                article = article.Where(x => x.Subject.Contains(queryOption.Keyword)
                                          || x.ContentText.Contains(queryOption.Keyword));
            }

            queryOption.SetSource(article);

            return View(queryOption);
        }

        //
        // GET: /Backend/Article/Details/5

        public ActionResult Details(Guid id)
        {
            Article article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        //
        // GET: /Backend/Article/Create

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "Name");
            return View();
        }

        //
        // POST: /Backend/Article/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Article article, HttpPostedFileBase[] uploads)
        {
            CheckFiles(uploads);

            if (ModelState.IsValid)
            {
                article.ID = Guid.NewGuid();
                article.CreateUser = WebSiteHelper.CurrentUserID;
                article.CreateDate = DateTime.Now;
                article.UpdateDate = DateTime.Now;

                db.Article.Add(article);

                HandleFiles(article, uploads);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Category, "ID", "Name", article.CategoryID);
            return View();
        }

        //
        // GET: /Backend/Article/Edit/5

        public ActionResult Edit(Guid id)
        {
            Article article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "Name", article.CategoryID);

            ArticleViewModel instance = new ArticleViewModel
            {
                ID = article.ID,
                CategoryID = article.CategoryID,
                Subject = article.Subject,
                Summary = article.Summary,
                ContentText = article.ContentText,
                IsPublish = article.IsPublish,
                PublishDate = article.PublishDate,
                ViewCount = article.ViewCount,
                CreateUser = article.CreateUser,
                CreateDate = article.CreateDate,
                UpdateUser = article.UpdateUser,
                UpdateDate = article.UpdateDate,
                Photo = article.Photo
            };

            return View(instance);
        }

        //
        // POST: /Backend/Article/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Article article, HttpPostedFileBase[] uploads)
        {
            //CheckFiles(uploads);

            if (ModelState.IsValid)
            {
                var instance = db.Article.FirstOrDefault(x => x.ID == article.ID);

                instance.CategoryID = article.CategoryID;
                instance.Subject = article.Subject;
                instance.Summary = article.Summary;
                instance.ContentText = article.ContentText;
                instance.IsPublish = article.IsPublish;
                instance.PublishDate = article.PublishDate;
                instance.UpdateUser = new Guid();
                instance.UpdateDate = DateTime.Now;

                db.Entry(instance).State = EntityState.Modified;

                HandleFiles(article, uploads);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "Name", article.CategoryID);
            return View();
        }

        //
        // GET: /Backend/Article/Delete/5

        public ActionResult Delete(Guid id)
        {
            Article article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        //
        // POST: /Backend/Article/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Article article = db.Article.Find(id);
            db.Article.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void CheckFiles(HttpPostedFileBase[] uploads)
        {
            if (uploads != null)
            {
                for (var i = 0; i < uploads.Length; i++)
                {
                    var upload = uploads[i];

                    // 檢查附檔名
                    // 檢查檔案格式（利用 Bitmap 或 WebImage 嘗試開啟圖片）為求範例簡單，這裡先不做到第二點

                    if (upload != null && !Regex.IsMatch(upload.FileName, @"\.(jpg|jpeg|gif|png)$"))
                    {
                        ModelState.AddModelError("Uploads[" + i + "]", "附件必須為圖片");
                    }
                }
            }
        }

        private void HandleFiles(Article article, HttpPostedFileBase[] uploads)
        {
            var timeStamp = DateTime.Now;

            // 在 Create 的時候，自己 new 出來的 Article 物件 Photo 屬性尚未被初始化
            if (article.Photo == null)
            {
                article.Photo = new List<Photo>();
            }

            // 逐一處理上傳檔案
            foreach (var upload in uploads)
            {
                if (upload == null)
                {
                    continue;
                }

                var photo = article.Photo.FirstOrDefault(x => x.FileName == upload.FileName);

                if (photo == null)
                {
                    photo = new Photo
                    {
                        ID = Guid.NewGuid(),
                        ArticleID = article.ID,
                        FileName = upload.FileName,
                        CreateDate = timeStamp,
                        UpdateDate = timeStamp
                    };
                    db.Photo.Add(photo);
                    article.Photo.Add(photo);
                }
                else
                {
                    photo.UpdateDate = timeStamp;
                    db.Entry(photo).State = EntityState.Modified;
                }

                var path = Server.MapPath("~/Uploads/" + article.ID + "/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                upload.SaveAs(Path.Combine(path, photo.FileName));
            }
        }

        public ActionResult ArticlePhoto(Guid id, int w, int h)
        {
            var photo = db.Photo.FirstOrDefault(x => x.ID == id);

            var filePath = Server.MapPath("~/Uploads/" + photo.ArticleID + "/" + photo.FileName);

            var image = new WebImage(filePath).Resize(w, h);

            return File(image.GetBytes(), "image/jpeg");
        }

        public ActionResult DeletePhoto(Guid id)
        {
            var photo = db.Photo.FirstOrDefault(x => x.ID == id);

            db.Photo.Remove(photo);
            db.SaveChanges();

            var path = Server.MapPath("~/Uploads/" + photo.ArticleID + "/");
            var filePath = Server.MapPath("~/Uploads/" + photo.ArticleID + "/" + photo.FileName);

            System.IO.File.Delete(filePath);

            if (Directory.EnumerateFiles(path).Count() == 0)
            {
                Directory.Delete(path);
            }
            return new EmptyResult();
        }

    }
}