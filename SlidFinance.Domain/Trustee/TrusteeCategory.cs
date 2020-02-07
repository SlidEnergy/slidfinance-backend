using System;
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
		public virtual UserCategory Category { get; set; }

		public TrusteeCategory(int trusteeId, int categoryId)
		{
			if (trusteeId <= 0)
				throw new ArgumentOutOfRangeException(nameof(trusteeId));

			if (categoryId <= 0)
				throw new ArgumentOutOfRangeException(nameof(categoryId));

			TrusteeId = trusteeId;
			CategoryId = categoryId;
		}

		public TrusteeCategory(Trustee trustee, UserCategory category) : this(trustee.Id, category.Id) { }

		public TrusteeCategory(ApplicationUser user, UserCategory category) : this(user.TrusteeId, category.Id) { }

		public TrusteeCategory(ApplicationUser user, int categoryId) : this(user.TrusteeId, categoryId) { }
	}
}
