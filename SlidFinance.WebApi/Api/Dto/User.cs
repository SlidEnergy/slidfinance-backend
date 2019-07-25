namespace SlidFinance.WebApi.Dto
{
    public class User
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public int[] BankIds { get; set; }

        public int[] CategoryIds { get; set; }
    }
}
