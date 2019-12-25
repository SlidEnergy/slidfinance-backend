using System.ComponentModel.DataAnnotations;

namespace SlidFinance.WebApi.Dto
{
    public class Mcc
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public string RuTitle { get; set; }

        public string Description { get; set; }

        public string RuDescription { get; set; }

        public MccCategory Category { get; set; }

        public bool IsSystem { get; set; }
    }
}
