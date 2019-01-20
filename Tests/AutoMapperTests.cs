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
        public async Task CreateMapperProfile_Validated()
        {
            Mapper.Initialize(x=>x.AddProfile<MappingProfile>());
            Mapper.Configuration.AssertConfigurationIsValid();
        }
    }
}