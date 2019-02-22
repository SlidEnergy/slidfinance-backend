using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyFinanceServer.Core
{
    public class ApplicationUser : IdentityUser, IUniqueObject<string>
    {
        [Required]
        public virtual ICollection<Bank> Banks { get; set; } = new List<Bank>();

        [Required]
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

        public void RenameBank(int id, string title)
        {
            if (Banks == null)
                throw new InvalidOperationException();

            var bank = Banks.FirstOrDefault(x => x.Id == id);

            if (bank == null)
                throw new ArgumentException($"Банк с идентификатором {id} не найден.");

            bank.Rename(title);
        }

        public void ReorderCategories(Category category, int order)
        {
            if (category.Order == order)
                return;

            if (Categories == null)
                throw new InvalidOperationException();

            category.SetOrder(order);

            int newOrder = order + 1;

            foreach (Category c in Categories.Where(x => x.Order >= order && x.Id != category.Id ).OrderBy(x => x.Order).ToList())
            {
                c.SetOrder(newOrder);
                newOrder++;
            }
        }
    }
}
