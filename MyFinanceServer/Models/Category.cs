using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public User User { get; set; }
    }
}
