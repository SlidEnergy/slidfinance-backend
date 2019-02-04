using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace MyFinanceServer.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public ICollection<Bank> Banks { get; set; }

        [Required]
        public IEnumerable<Category> Categories { get; set; }

        public Bank LinkBank(string title)
        {
            if (Banks == null)
                throw new InvalidOperationException();

            var bank = new Bank() { Title = title };

            Banks.Add(bank);

            return bank;
        }

        public Bank UnlinkBank(string id) {
            if (Banks == null)
                throw new InvalidOperationException();

            var bank = Banks.FirstOrDefault(x => x.Id == id);

            if (bank == null)
                throw new ArgumentException("Банк с указанным идентификатором не найден.", id);

            Banks.Remove(bank);

            return bank;
        }
    }
}
