using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyFinanceServer.Tests
{
    public static class ControllerExtensions
    {
        public static void AddControllerContext(this ControllerBase controller, Models.User user)
        {
            controller.ControllerContext = CreateControllerContext(user);
        }

        private static ControllerContext CreateControllerContext(Models.User user)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreatePrincipal(user)
                }
            };
        }

        private static ClaimsPrincipal CreatePrincipal(Models.User user)
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
