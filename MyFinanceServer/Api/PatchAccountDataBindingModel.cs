using System;
using System.Collections.Generic;

namespace MyFinanceServer.Api
{
    public class PatchAccountDataBindingModel
    {
        public string Code { get; set; }

        public float? Balance { get; set; }

        public ICollection<Dto.Transaction> Transactions { get; set; }
    }
}
