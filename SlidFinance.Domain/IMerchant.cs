using System;

namespace SlidFinance.Domain
{
    public interface IMerchant
    {
        string Address { get; set; }
        DateTime Created { get; set; }
        string DisplayName { get; set; }
        int Id { get; set; }
        string Name { get; set; }
        DateTime Updated { get; set; }
        int MccId { get; set; }
        Mcc Mcc { get; set; }
        bool IsPublic { get; set; }
    }
}