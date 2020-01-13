using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SlidFinance.Domain.Accounts.Template.Tariff
{
	[Table("CardTariffs")]
	public class CardTariff: ProductTariff
	{
		public float Percent { get; set; }

		public float IssuePayment { get; set; }

		public float MaintenancePayment { get; set; }

		public MaintenancePaymentType MaintenancePaymentType { get; set; }

		public float TotalCashbackLimit { get; set; }
	}
}
