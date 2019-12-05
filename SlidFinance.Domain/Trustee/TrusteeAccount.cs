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

		public TrusteeAccount(int trusteeId, int accountId)
		{
			TrusteeId = trusteeId;
			AccountId = accountId;
		}

		public TrusteeAccount(Trustee trustee, BankAccount account) : this(trustee.Id, account.Id) { }

		public TrusteeAccount(ApplicationUser user, BankAccount account) : this(user.TrusteeId, account.Id) { }

		public TrusteeAccount(ApplicationUser user, int accountId) : this(user.TrusteeId, accountId) { }
	}
}
