using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SlidFinance.Domain
{
    public class ApplicationUser : IdentityUser, IUniqueObject<string>
    {
		public int TrusteeId { get; set; }
		[Required]
		public virtual Trustee Trustee { get; set; }
    }
}
