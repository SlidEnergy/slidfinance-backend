using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class Transaction: IUniqueObject<int>
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public virtual Category Category { get; set; }

        [Required]
        public string Description { get; set; }

		[Required]
		public string UserDescription { get; set; }

		[Required]
        public virtual BankAccount Account { get; set; }

        public int? Mcc { get; set; }

        [Required]
        public string BankCategory { get; set; }

        public bool Approved { get; set; }

        public bool IsBelongsTo(string userId) => Account.Bank.User.Id == userId;
    }
}
