using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace twMVCWorkshop.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Index(Exception exception = null)
        {
            //為了資安，除了404以外，一律都用200，正常(成功)回傳出去
            //不要透漏太多訊息
            Response.StatusCode = 200;
            return View();
        }

        [PreventDirectAccess] //不允許有人手動來這頁存取
        public ActionResult PageNotFound(string error, Exception exception)
        {
            ViewData["Description"] = "404 Error, Page not Found!";
            Response.StatusCode = 404;
            return View();
        }

        private class PreventDirectAccessAttribute : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                object value = filterContext.RouteData.Values["from_Application_Error_Event"];
                if (!(value is bool && (bool)value))
                {
                    filterContext.Result = new ViewResult { ViewName = "Index" };
                }
            }
        }

    }
}
