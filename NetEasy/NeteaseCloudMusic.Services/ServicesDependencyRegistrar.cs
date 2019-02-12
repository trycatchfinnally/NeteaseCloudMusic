using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Services.UniqueSymbol;
using NeteaseCloudMusic.Services.AudioDecode;
using CommonServiceLocator;
using Prism.Ioc;

namespace NeteaseCloudMusic.Services
{
  public   class ServicesDependencyRegistrar
        //: Core.Infrastructure.DependencyManagement.IDependencyRegistrar
    {
        public int Order => 0;

        public void Register( IContainerRegistry  containerRegistry)
        {

            containerRegistry.RegisterSingleton<IUniqueServices, UniqueServices>();
            containerRegistry.RegisterSingleton<INetWorkServices, NeteaseCloundMusicNetWorkServices>();
            
           
        }
    }
}
