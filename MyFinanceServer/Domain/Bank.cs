using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyFinanceServer.Data
{
    public class Bank
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public float OwnFunds => Accounts == null ? 0 : Accounts.Sum(x => x.OwnFunds);

        [Required]
        public virtual ICollection<BankAccount> Accounts { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }

        public Bank() { }

        public Bank(string title, ApplicationUser user) {
            Title = title;
            User = user;
        }

        public void Rename(string title)
        {
            Title = title;
        }

        public BankAccount LinkAccount(string title, string code, float balance, float creditLimit)
        {
            var account = new BankAccount(title, code, balance, creditLimit);

            if (Accounts == null)
                Accounts = new List<BankAccount>();

            Accounts.Add(account);

            return account;
        }

        public void UnlinkAccount(int id)
        {
            if (Accounts == null)
                throw new InvalidOperationException();

            var account = Accounts.FirstOrDefault(x => x.Id == id);

            if (account == null)
                throw new ArgumentException($"Банк с идентификатором {id} не найден.");

            Accounts.Remove(account);
        }
    }
}
