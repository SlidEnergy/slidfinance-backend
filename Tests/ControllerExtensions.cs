using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Data;
using System.Collections.Generic;
using System.Security.Claims;

namespace MyFinanceServer.Tests
{
    public static class ControllerExtensions
    {
        public static void AddControllerContext(this ControllerBase controller, ApplicationUser user)
        {
            controller.ControllerContext = CreateControllerContext(user);
        }

        private static ControllerContext CreateControllerContext(ApplicationUser user)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreatePrincipal(user)
                }
            };
        }

        private static ClaimsPrincipal CreatePrincipal(ApplicationUser user)
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
