using System;

namespace SlidFinance.WebApi.Dto
{
    public class ImportTransaction
    {
        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public string Description { get; set; }
		
		public int? Mcc { get; set; }

        public string Category { get; set; }
    }
}
