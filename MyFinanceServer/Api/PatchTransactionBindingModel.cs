using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api
{
    public class PatchTransactionBindingModel
    {
        public string CategoryId { get; set; }
    }
}
