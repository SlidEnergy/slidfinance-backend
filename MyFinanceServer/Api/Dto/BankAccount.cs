namespace MyFinanceServer.Api.Dto
{
    public class BankAccount
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public float Balance { get; set; }

        public string Title { get; set; }

        public string BankId { get; set; }

        public string[] TransactionIds { get; set; }
    }
}
