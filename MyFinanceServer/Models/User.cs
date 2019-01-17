using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public IEnumerable<Models.Bank> Banks { get; set; }

        [Required]
        public IEnumerable<Models.Category> Categories { get; set; }
    }
}
