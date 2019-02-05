using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api
{
    public class EditBankAccountBindingModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public float Balance { get; set; }
    }
}
