using AutoMapper;
using SlidFinance.WebApi;
using SlidFinance.Infrastructure;

namespace SlidFinance.WebApi.UnitTests
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
