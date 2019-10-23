using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
    public class Merchant : IUniqueObject
    {
        public int Id => _model.Id;

        public string Address
        {
            get { return _model.Address; }
            set { _model.Address = value; }
        }

        public int MccId
        {
            get { return _model.MccId; }
            set { _model.MccId = value; }
        }

        public Mcc Mcc
        {
            get { return _model.Mcc; }
            set { _model.Mcc = value; }
        }

        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; }
        }

        public string DisplayName
        {
            get { return _model.DisplayName; }
            set { _model.DisplayName = value; }
        }

        public DateTime Created => _model.Created;

        public DateTime Updated => _model.Updated;

        private IMerchant _model;

        public Merchant(IMerchant merchant)
        {
            _model = merchant;
        }
    }
}
