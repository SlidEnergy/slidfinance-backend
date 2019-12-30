using SlidFinance.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.WebApi.Dto
{
    public class Merchant
    {
        public int Id { get; set; }

        public string Address { get; set; }

        [Required]
        public int MccId { get; set; }

        [Required]
        public Dto.Mcc Mcc { get; set; }

        [Required]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
