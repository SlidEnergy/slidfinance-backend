using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MyFinanceServer.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public IEnumerable<Bank> Banks { get; set; }

        [Required]
        public IEnumerable<Category> Categories { get; set; }
    }
}
