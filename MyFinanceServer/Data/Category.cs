using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Data
{
    public class Category
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        public ApplicationUser User { get; set; }
    }
}
