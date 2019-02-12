using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Infrastructure.Mapper
{
    public interface  IMapperConfiguration
    {
        int Order { get; }
        Action<AutoMapper.IMapperConfigurationExpression> GetConfiguration();

    }
}
