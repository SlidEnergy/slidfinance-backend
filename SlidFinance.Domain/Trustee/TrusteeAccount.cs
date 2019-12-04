using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
	public class TrusteeAccount
	{
		public int TrusteeId { get; set; }
		[Required]
		public virtual Trustee Trustee { get; set; }

		public int AccountId { get; set; }
		[Required]
		public virtual BankAccount Account { get; set; }
	}
}
