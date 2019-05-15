using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Core
{
    public class Rule : IUniqueObject
    {
        public int Id { get; set; }

        public int Order { get; set; }

        [Required]
        public virtual Category Category { get; set; }

        public string Description { get; set; }

        [Required]
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

        public bool IsBelongsTo(string userId) => Account.Bank.User.Id == userId;
    }
}
