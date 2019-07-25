using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;

namespace MyFinanceServer.UnitTests
{
    public class AutoMapperTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateMapperProfile_Validated()
        {
            var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
            Mapper.Initialize(x=>x.AddProfile(new MappingProfile(context)));
            Mapper.Configuration.AssertConfigurationIsValid();
        }
    }
}