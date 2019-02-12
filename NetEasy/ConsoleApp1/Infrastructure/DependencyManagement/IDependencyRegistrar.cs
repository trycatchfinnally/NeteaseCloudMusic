using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// 通过实现此接口进行注册操作
    /// </summary>
    public interface  IDependencyRegistrar
    {
        int Order { get; }
        void Register(IContainerRegistry containerRegistry);

    }
}
