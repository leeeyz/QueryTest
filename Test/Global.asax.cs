using MyHelper.Common;
using MyHelper.Common.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Test
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static SubscribeInfo SubscribeInfo = new SubscribeInfo(10);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LogHelper.LogInfo("123");

            SubscribeInfo.Subscribe(async mqmsg=> {
                await Task.Run(() => {
                    LogHelper.LogInfo(mqmsg.Msg);
                });
            });

            SubscribeInfo.ErrorHandle(async (dyn, expc, type) => {
                await Task.Run(() => {
                    LogHelper.LogError(JsonConvert.SerializeObject(dyn), expc);
                });
            });
        }
    }
}
