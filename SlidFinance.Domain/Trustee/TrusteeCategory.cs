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

		public TrusteeCategory(int trusteeId, int categoryId)
		{
			TrusteeId = trusteeId;
			CategoryId = categoryId;
		}

		public TrusteeCategory(Trustee trustee, Category category) : this(trustee.Id, category.Id) { }

		public TrusteeCategory(ApplicationUser user, Category category) : this(user.TrusteeId, category.Id) { }

		public TrusteeCategory(ApplicationUser user, int categoryId) : this(user.TrusteeId, categoryId) { }
	}
}
