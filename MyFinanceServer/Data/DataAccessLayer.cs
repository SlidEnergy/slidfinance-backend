using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinanceServer.Core;

namespace MyFinanceServer.Data
{
    public class DataAccessLayer
    {
        public ICategoriesRepository Categories;

        public IRepository Users => _all;

        private IRepository _all;

        public DataAccessLayer(ICategoriesRepository categories, IRepository repository)
        {
            Categories = categories;
            _all = repository;
        }
    }
}
