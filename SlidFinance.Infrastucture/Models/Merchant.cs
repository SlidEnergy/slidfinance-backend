﻿using SlidFinance.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Infrastucture.Models
{
    public class Merchant: IMerchant
    {
        public int Id { get; set; }

        public string Address { get; set; }

        [Required]
        public int MccId { get; set; }

        [Required]
        public Mcc Mcc { get; set; }

        [Required]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
