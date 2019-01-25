﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api.Dto
{
    public class Bank
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string[] AccountIds { get; set; }

        public float Balance { get; set; }
    }
}
