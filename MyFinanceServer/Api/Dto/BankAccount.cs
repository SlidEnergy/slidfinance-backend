namespace MyFinanceServer.Api.Dto
{
    public class BankAccount
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public float Balance { get; set; }

        public float CreditLimit { get; set; }

        public string Title { get; set; }

        public int BankId { get; set; }
    }
}
