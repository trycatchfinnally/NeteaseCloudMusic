using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core
{
  public static   class LogExtension
    {
        public  static void Debug(this ILoggerFacade logger,string msg)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            logger.Log(msg, Category.Debug, Priority.Medium);
        }
        public static void Info(this ILoggerFacade logger,string msg)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            logger.Log(msg, Category.Info, Priority.Low);
        }
        public static void Exception(this ILoggerFacade logger,string msg,Exception e)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            logger.Log(msg+e.Message, Category.Exception, Priority.High);
        }
    }
}
