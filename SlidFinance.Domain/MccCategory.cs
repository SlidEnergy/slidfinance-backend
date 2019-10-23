using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SlidFinance.Domain
{
    public enum MccCategory : byte
    {
        [Description("")]
        None = 0,
        [Description("Контрактные услуги")]
        ContractServices = 1,
        [Description("контрактные работы")]
        ContractWork = 2,
        [Description("Оптовые распростронители и производители")]
        WholesaleDistributorsAndManufacturers = 3,
        [Description("Авиалинии")]
        Airlines = 4,
        [Description("Аренда автомобилей")]
        CarRent = 5,
        [Description("Отели")]
        Hotels = 6,
        [Description("Транспортировка")]
        Transportation = 7,
        [Description("Коммунальные услуги")]
        Utilities = 8,
        [Description("Провайдеры услуг")]
        ServiceProviders = 9,
        [Description("Оптовые поставщики и производители")]
        WholesaleSuppliersAndManufacturers = 10,
        [Description("Продажа в розницу")]
        RetailSale = 11,
        [Description("Автомобили и прочие транспортные средства")]
        CarsAndOtherVehicles = 12,
        [Description("магазины одежды")]
        ClothingStores = 13,
        [Description("Различные магазины")]
        VariousShops = 14,
        [Description("Поставщики почтовых/ телефонных услуг")]
        PostalTelephoneProviders = 15,
        [Description("Индивидуальные сервис провайдеры")]
        IndividualServiceProviders = 16,
        [Description("Бизнес услуги")]
        BusinessServices = 17,
        [Description("Ремонтные услуги")]
        RepairServices = 18,
        [Description("Развлечения")]
        Entertainment = 19,
    }
}
