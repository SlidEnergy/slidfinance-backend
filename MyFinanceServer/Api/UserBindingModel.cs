using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api
{
    public class UserBindingModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
