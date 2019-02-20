using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Threading.Tasks;
using AutoMapper;

namespace MyFinanceServer.Tests
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