﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.WebApi
{
    public class EditBankAccountBindingModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public float Balance { get; set; }
        public float CreditLimit { get; set; }
    }
}
