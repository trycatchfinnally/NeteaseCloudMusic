using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Infrastructure.Mapper
{
  public   class AutoMapperConfiguration
    {
        private static MapperConfiguration _mapperConfiguration;
        private static IMapper _mapper;
        public static IMapper Mapper => _mapper;
        public static MapperConfiguration MapperConfiguration => _mapperConfiguration;
        public static void Init(IEnumerable<Action<IMapperConfigurationExpression>> configurationActions)
        {
            if (configurationActions == null)
                throw new ArgumentNullException(nameof(configurationActions));

            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                configurationActions.Each(x => x(cfg));
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }
    }
}
