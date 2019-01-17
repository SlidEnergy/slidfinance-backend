using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api
{
    public class PatchTransactionBindingModel
    {
        public int CategoryId { get; set; }
    }
}
