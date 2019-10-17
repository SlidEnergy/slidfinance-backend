using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class Mcc: IUniqueObject
    {
        public int Id { get; set; }

        [Required]
        public int Code { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public MccCategory Category { get; set; }
    }
}
