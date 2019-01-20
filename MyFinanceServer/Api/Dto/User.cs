namespace MyFinanceServer.Api.Dto
{
    public class User
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string[] BankIds { get; set; }

        public string[] CategoryIds { get; set; }
    }
}
