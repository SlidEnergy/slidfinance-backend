using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
	public class Bank: IUniqueObject
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public Bank() { }

        public Bank(string title) {
            Title = title;
        }

        public void Rename(string title)
        {
            Title = title;
        }
    }
}
