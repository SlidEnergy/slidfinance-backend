using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
	public class TrusteeSaltedgeAccount
	{
		public int TrusteeId { get; set; }
		[Required]
		public virtual Trustee Trustee { get; set; }

		public int SaltedgeAccountId { get; set; }

		[Required]
		public virtual SaltedgeAccount SaltedgeAccount { get; set; }

		public TrusteeSaltedgeAccount(int trusteeId, int saltedgeAccountId)
		{
			if (trusteeId <= 0)
				throw new ArgumentOutOfRangeException(nameof(trusteeId));

			if (saltedgeAccountId <= 0)
				throw new ArgumentOutOfRangeException(nameof(saltedgeAccountId));

			TrusteeId = trusteeId;
			SaltedgeAccountId = saltedgeAccountId;
		}

		public TrusteeSaltedgeAccount(Trustee trustee, SaltedgeAccount saltedgeAccount) : this(trustee.Id, saltedgeAccount.Id) { }

		public TrusteeSaltedgeAccount(ApplicationUser user, SaltedgeAccount saltedgeAccount) : this(user.TrusteeId, saltedgeAccount.Id) { }

		public TrusteeSaltedgeAccount(ApplicationUser user, int saltedgeAccountId) : this(user.TrusteeId, saltedgeAccountId) { }
	}
}
