using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Common.Log
{
    public class LogHelper
    {
        // Fields
        public static readonly ILog logerror = LogManager.GetLogger("logerror");
        public static readonly ILog loginfo = LogManager.GetLogger("loginfo");
        public static readonly ILog logwarn = LogManager.GetLogger("logwarn");

        // Methods
        public static void LogError(Exception ex)
        {
            logerror.Error(ex);
        }

        public static void LogError(string info, Exception ex)
        {
            logerror.Error(info, ex);
        }

        public static void LogInfo(string info)
        {
            loginfo.Info(info);
        }

        public static void LogWarn(Exception ex)
        {
            logwarn.Warn(ex);
        }

        public static void LogWarn(string info, Exception ex)
        {
            logwarn.Warn(info, ex);
        }

        public static void SetConfig()
        {
            XmlConfigurator.Configure();
        }

        public static void SetConfig(FileInfo configFile)
        {
            XmlConfigurator.Configure(configFile);
        }
    }
}
