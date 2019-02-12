using CommonServiceLocator;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core
{
   public static  class CommonTools
    {
        [ThreadStatic]

        private static ILoggerFacade _logger;
        public static ILoggerFacade Logger
        {
            get
            {
                if(_logger==null )
                    _logger= ServiceLocator.Current.GetInstance<ILoggerFacade>();
                return _logger;
            }
        }
         
    }
}
