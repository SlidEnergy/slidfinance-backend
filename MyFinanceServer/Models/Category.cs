namespace MyFinanceServer.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public User User { get; set; }
    }
}
