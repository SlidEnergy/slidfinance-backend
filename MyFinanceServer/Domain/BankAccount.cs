using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Data
{
    public class BankAccount
    {
        public string Id { get; set; }

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

        public BankAccount(string title, string code, float balance, float creditLimit)
        {
            Title = title;
            Code = code;
            Balance = balance;
            CreditLimit = creditLimit;
        }

        public void Rename(string title)
        {
            Title = title;
        }

        public void ChangeCode(string code)
        {
            Code = code;
        }

        public void SetBalance(float balance)
        {
            Balance = balance;
        }

        public void ChangeCreditLimit(float creditLimit)
        {
            CreditLimit = creditLimit;
        }
    }
}
