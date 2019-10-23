using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class Mcc: IUniqueObject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4)]
        public string Code { get; set; }

        [Required]
        public string Title { get; set; }

		public string RuTitle { get; set; }

        public string Description { get; set; }

		public string RuDescription { get; set; }

        [Required]
        public MccCategory Category { get; set; }
    }
}
