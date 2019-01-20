using AutoMapper;
using MyFinanceServer.Api;

namespace MyFinanceServer.Tests
{
    public class AutoMapperFactory
    {
        public IMapper Create()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return new Mapper(configuration);
        }
    }
}
