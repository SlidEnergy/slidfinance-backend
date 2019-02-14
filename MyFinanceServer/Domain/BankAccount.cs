using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Data
{
    public class BankAccount
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public float Balance { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public Bank Bank { get; set; }

        [Required]
        public ICollection<Transaction> Transactions { get; set; }

        public BankAccount() { }

        public BankAccount(string title, string code, float balance)
        {
            Title = title;
            Code = code;
            Balance = balance;
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
    }
}
