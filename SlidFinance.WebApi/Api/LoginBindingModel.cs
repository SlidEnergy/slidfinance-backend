using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.WebApi
{
    public class LoginBindingModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
