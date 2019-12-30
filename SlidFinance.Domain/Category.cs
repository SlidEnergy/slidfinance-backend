using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class Category: IUniqueObject
    {
        public int Id { get; set; }

        public int Order { get; set; }

        [Required]
        public string Title { get; set; }

		public Category() { }

        public Category(string title, int order)
        {
            Title = title;
            Order = order;
        }

        public void Rename(string title) => Title = title;

        public void SetOrder(int order) => Order = order;
    }
}
