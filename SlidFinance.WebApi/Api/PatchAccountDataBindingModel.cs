using System;
using System.Collections.Generic;

namespace SlidFinance.WebApi
{
    public class PatchAccountDataBindingModel
    {
        public string Code { get; set; }

        public float? Balance { get; set; }

        public ICollection<Dto.Transaction> Transactions { get; set; }
    }
}
