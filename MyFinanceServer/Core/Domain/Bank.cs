using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyFinanceServer.Core
{
    public class Bank: IUniqueObject
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

        public bool IsBelongsTo(string userId)
        {
            return User.Id == userId;
        }

        public void Rename(string title)
        {
            Title = title;
        }
    }
}
