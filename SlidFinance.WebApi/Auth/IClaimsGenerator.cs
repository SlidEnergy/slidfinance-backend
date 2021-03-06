﻿using Microsoft.AspNetCore.Identity;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Security.Claims;

namespace SlidFinance.WebApi
{
    public interface IClaimsGenerator
    {
        IEnumerable<Claim> CreateClaims(ApplicationUser user, IEnumerable<string> roles, AccessMode accessMode);
    }
}
