using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class BankAccount : IUniqueObject
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public float Balance { get; set; }

        public float CreditLimit { get; set; }

        public float OwnFunds => Balance - CreditLimit;

        [Required]
        public string Title { get; set; }

		public int BankId { get; set; }
        [Required]
        public virtual Bank Bank { get; set; }

		[Required]
		public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public BankAccount() { }

        public BankAccount(Bank bank, string title, string code, float balance, float creditLimit)
        {
            Bank = bank;
            Title = title;
            Code = code;
            Balance = balance;
            CreditLimit = creditLimit;
        }

        public void Update(string title, string code, float balance, float creditLimit)
        {
            Title = title;
            Code = code;
            Balance = balance;
            CreditLimit = creditLimit;
        }
    }
}
