namespace MyFinanceServer.Api
{
    public class AddBankAccountBindingModel
    {
        public string BankId { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public float Balance { get; set; }

        public float CreditLimit { get; set; }
    }
}
