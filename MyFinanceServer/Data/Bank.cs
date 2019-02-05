using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyFinanceServer.Data
{
    public class Bank
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public ICollection<BankAccount> Accounts { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public Bank() { }

        public Bank(string title, ApplicationUser user) {
            Title = title;
            User = user;
        }

        public void Rename(string title)
        {
            Title = title;
        }

        public BankAccount LinkAccount(string title, string code, float balance)
        {
            var account = new BankAccount(title, code, balance);

            if (Accounts == null)
                Accounts = new List<BankAccount>();

            Accounts.Add(account);

            return account;
        }

        public void UnlinkAccount(string id)
        {
            if (Accounts == null)
                throw new InvalidOperationException();

            var account = Accounts.FirstOrDefault(x => x.Id == id);

            if (account == null)
                throw new ArgumentException("Банк с указанным идентификатором не найден.", id);

            Accounts.Remove(account);
        }
    }
}
