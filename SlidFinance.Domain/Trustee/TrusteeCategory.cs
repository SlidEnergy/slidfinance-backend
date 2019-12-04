using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
	public class TrusteeCategory
	{
		public int TrusteeId { get; set; }
		[Required]
		public virtual Trustee Trustee { get; set; }

		public int CategoryId { get; set; }
		[Required]
		public virtual Category Category { get; set; }
	}
}
