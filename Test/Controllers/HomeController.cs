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

        public ActionResult Buy()
        {
            string guid = Guid.NewGuid().ToString();
            if(MvcApplication.SubscribeInfo.IsFulled())
            {
                return new JsonResult() { Data = new { gudi = guid,msg = "已经结束" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                bool result = MvcApplication.SubscribeInfo.Incr(guid);
                if(result)
                {
                    return new JsonResult() { Data = new { gudi = guid, msg = "成功" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult() { Data = new { gudi = guid, msg = "失败" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        public ActionResult Reset()
        {
            MvcApplication.SubscribeInfo.ResetRedis();
            return new JsonResult() { Data = new { msg = "重设" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}