using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Buy(string id)
        {
            if(MvcApplication.SubscribeInfo.IsFulled())
            {
                return new JsonResult() { Data = new { id = id, msg = "已经结束" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                int result = MvcApplication.SubscribeInfo.Incr(id);
                if(result == 0)
                {
                    return new JsonResult() { Data = new { id = id, msg = "成功" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else if (result == 1)
                {
                    return new JsonResult() { Data = new { id = id, msg = "已在队列" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult() { Data = new { id = id, msg = "失败" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        public ActionResult Reset()
        {
            MvcApplication.SubscribeInfo.RedisFlush();
            return new JsonResult() { Data = new { msg = "重设" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}