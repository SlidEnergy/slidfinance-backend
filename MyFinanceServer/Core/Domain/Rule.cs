using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Core
{
    public class Rule
    {
        public int Id { get; set; }

        public int Order { get; set; }

        [Required]
        public virtual Category Category { get; set; }

        public string Description { get; set; }

        public virtual BankAccount Account { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }
    }
}
