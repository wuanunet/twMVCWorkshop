using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using twMVCWorkshop.Helpers;

namespace twMVCWorkshop.Controllers
{
    public class AboutController : Controller
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            try
            {
                int a = 1;
                int b = 0;
                var result = a / b;
            }
            catch (Exception ex)
            {
                //logger.Fatal("error");
                //logger.FatalException("error", ex);
                logger.Fatal(LogHelper.BuildExceptionMessage(ex));
                throw;
            }

            return View();
        }

    }
}
