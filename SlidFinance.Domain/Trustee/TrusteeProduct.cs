using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.Domain
{
	public class TrusteeProduct
	{
		public int TrusteeId { get; set; }
		[Required]
		public virtual Trustee Trustee { get; set; }

		public int ProductId { get; set; }
		[Required]
		public virtual Product Product { get; set; }

		public TrusteeProduct(int trusteeId, int productId)
		{
			TrusteeId = trusteeId;
			ProductId = productId;
		}

		public TrusteeProduct(Trustee trustee, Product product) : this(trustee.Id, product.Id) { }

		public TrusteeProduct(ApplicationUser user, Product product) : this(user.TrusteeId, product.Id) { }

		public TrusteeProduct(ApplicationUser user, int productId) : this(user.TrusteeId, productId) { }
	}
}
