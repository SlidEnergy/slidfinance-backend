using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Core
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

        [Required]
        public virtual Bank Bank { get; set; }

        [Required]
        public virtual ICollection<Transaction> Transactions { get; set; }

        public BankAccount() { }

        public BankAccount(Bank bank, string title, string code, float balance, float creditLimit)
        {
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

        public bool IsBelongsTo(string userId) => Bank.User.Id == userId;
    }
}
