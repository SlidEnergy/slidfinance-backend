﻿using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api.Dto
{
    public class Category
    {
        public string Id { get; set; }

        public int Order { get; set; }

        public string Title { get; set; }
    }
}
