using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class Transaction: IUniqueObject<int>
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

		public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        public string Description { get; set; } = "";

        [Required]
        public string UserDescription { get; set; } = "";

		public int AccountId { get; set; }
		[Required]
        public virtual BankAccount Account { get; set; }

        public int? MccId { get; set; }

        public virtual Mcc Mcc { get; set; }

        [Required]
        public string BankCategory { get; set; }

        public bool Approved { get; set; }
    }
}
