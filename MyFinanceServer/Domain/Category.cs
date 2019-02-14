using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Data
{
    public class Category
    {
        public string Id { get; set; }

        public int Order { get; set; }

        [Required]
        public string Title { get; set; }

        public virtual ApplicationUser User { get; set; }

        public Category() { }

        public Category(string title, int order, ApplicationUser user)
        {
            Title = title;
            Order = order;
            User = user;
        }

        public void Rename(string title)
        {
            Title = title;
        }

        public void SetOrder(int order)
        {
            Order = order;
        }
    }
}
