using AutoMapper;
using MyFinanceServer.Api;
using MyFinanceServer.Data;

namespace MyFinanceServer.UnitTests
{
    public class AutoMapperFactory
    {
        public IMapper Create(ApplicationDbContext context)
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile(context)));
            return new Mapper(configuration);
        }
    }
}
