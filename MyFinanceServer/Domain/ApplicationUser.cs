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
        public virtual ICollection<Bank> Banks { get; set; }

        [Required]
        public virtual ICollection<Category> Categories { get; set; }

        public void RenameBank(int id, string title)
        {
            if (Banks == null)
                throw new InvalidOperationException();

            var bank = Banks.FirstOrDefault(x => x.Id == id);

            if (bank == null)
                throw new ArgumentException($"Банк с идентификатором {id} не найден.");

            bank.Rename(title);
        }

        public Bank LinkBank(string title)
        {
            var bank = new Bank(title, this);

            if (Banks == null)
                Banks = new List<Bank>();

            Banks.Add(bank);

            return bank;
        }

        public void UnlinkBank(int id)
        {
            if (Banks == null)
                throw new InvalidOperationException();

            var bank = Banks.FirstOrDefault(x => x.Id == id);

            if (bank == null)
                throw new ArgumentException($"Банк с идентификатором {id} не найден.");

            Banks.Remove(bank);
        }

        public Category AddCategory(string title)
        {
            if (Categories == null)
                throw new InvalidOperationException();

            var order = Categories.Max(x => x.Order);
            order++;

            var category = new Category(title, order, this);

            if (Categories == null)
                Categories = new List<Category>();

            Categories.Add(category);

            return category;
        }

        public void DeleteCategory(int id)
        {
            if (Categories == null)
                throw new InvalidOperationException();

            var category = Categories.FirstOrDefault(x => x.Id == id);

            if (category == null)
                throw new ArgumentException($"Категория с идентификатором {id} не найдена.");

            Categories.Remove(category);
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
