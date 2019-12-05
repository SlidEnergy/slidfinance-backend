using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class Rule : IUniqueObject
    {
        public int Id { get; set; }

        public int Order { get; set; }

		public int CategoryId { get; set; }
        [Required]
        public virtual Category Category { get; set; }

        public string Description { get; set; }

		public int? AccountId { get; set; }
        public virtual BankAccount Account { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }

        public Rule() { }

        public Rule(BankAccount account, string bankCategory, Category category, string description, int? mcc, int order)
        {
            Account = account;
            BankCategory = bankCategory;
            Category = category;
            Description = description;
            Mcc = mcc;
            Order = order;
        }
    }
}
