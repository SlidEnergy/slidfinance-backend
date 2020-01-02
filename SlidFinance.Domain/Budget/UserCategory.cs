using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class UserCategory: IUniqueObject
    {
        public int Id { get; set; }

        public int Order { get; set; }

        [Required]
        public string Title { get; set; }

		public UserCategory() { }

        public UserCategory(string title, int order)
        {
            Title = title;
            Order = order;
        }

        public void Rename(string title) => Title = title;

        public void SetOrder(int order) => Order = order;
    }
}
