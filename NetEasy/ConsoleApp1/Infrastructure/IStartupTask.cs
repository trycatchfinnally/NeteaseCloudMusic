using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Infrastructure
{
    public interface  IStartupTask
    {
        int Order { get; }
        void Execute();

    }
}
