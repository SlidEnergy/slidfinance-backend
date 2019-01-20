using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;

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
