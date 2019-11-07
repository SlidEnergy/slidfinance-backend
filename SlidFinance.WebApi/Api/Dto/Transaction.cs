using System;

namespace SlidFinance.WebApi.Dto
{
    public class Transaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public int? CategoryId { get; set; }

        public string Description { get; set; }
		
		public string UserDescription { get; set; }

		public int? Mcc { get; set; }

        public string BankCategory { get; set; }

        public bool Approved { get; set; }
    }
}
