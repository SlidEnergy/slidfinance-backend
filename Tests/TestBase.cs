using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;

namespace MyFinanceServer.Tests
{
    public class TestBase
    {
        protected IMapper CreateAutoMapper()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return new Mapper(configuration);
        }

        protected ControllerContext CreateControllerContext(Models.User user)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreatePrincipal(user)
                }
            };
        }

        protected ClaimsPrincipal CreatePrincipal(Models.User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            return claimsPrincipal;
        }
    }
}
