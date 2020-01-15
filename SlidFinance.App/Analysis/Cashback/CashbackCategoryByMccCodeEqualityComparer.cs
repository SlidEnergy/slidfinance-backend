using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App.Analysis
{
    class CashbackCategoryByMccCodeEqualityComparer : IEqualityComparer<CashbackCategoryByMccCode>
    {
        public bool Equals(CashbackCategoryByMccCode a, CashbackCategoryByMccCode b)
        {
            if (b == null && a == null)
                return true;
            else if (a == null || b == null)
                return false;
            else if (a.TariffId == b.TariffId && a.MccCode == b.MccCode)
                return true;
            else
                return false;
        }

        public int GetHashCode(CashbackCategoryByMccCode relation)
        {
            int hCode = relation.TariffId ^ Convert.ToInt32(relation.MccCode);
            return hCode.GetHashCode();
        }
    }
}
