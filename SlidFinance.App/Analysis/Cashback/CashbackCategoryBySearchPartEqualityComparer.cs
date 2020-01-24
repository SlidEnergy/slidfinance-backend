using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App.Analysis
{
    class CashbackCategoryBySearchPartEqualityComparer : IEqualityComparer<CashbackCategoryBySearchPart>
    {
        public bool Equals(CashbackCategoryBySearchPart a, CashbackCategoryBySearchPart b)
        {
            if (b == null && a == null)
                return true;
            else if (a == null || b == null)
                return false;
            else if (a.TariffId == b.TariffId && a.SearchPart == b.SearchPart)
                return true;
            else
                return false;
        }

        public int GetHashCode(CashbackCategoryBySearchPart relation)
        {
            int hCode = relation.SearchPart.GetHashCode() ^ relation.TariffId;
            return hCode.GetHashCode();
        }
    }
}
