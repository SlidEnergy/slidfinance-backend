using Newtonsoft.Json;
using SlidFinance.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class Merchant
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public int MccId { get; set; }

        [JsonIgnore]
        public virtual Mcc Mcc { get; set; }

        [Required]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string CreatedById { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser CreatedBy { get; set; }

        public bool IsPublic { get; set; }
    }
}
