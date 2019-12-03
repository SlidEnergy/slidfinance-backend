using System.ComponentModel.DataAnnotations;

namespace SlidFinance.WebApi.Dto
{
    public class Category
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string Title { get; set; }
    }
}
