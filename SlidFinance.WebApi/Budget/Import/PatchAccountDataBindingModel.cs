using System;
using System.Collections.Generic;

namespace SlidFinance.WebApi
{
    public class PatchAccountDataBindingModel
    {
        public string Code { get; set; }

        public int? AccountId { get; set; }

        public float? Balance { get; set; }

        public ICollection<Dto.ImportTransaction> Transactions { get; set; }
    }
}
