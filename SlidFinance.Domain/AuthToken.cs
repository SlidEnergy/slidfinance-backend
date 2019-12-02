using System;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
	public class AuthToken : IUniqueObject
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		[Required]
		public virtual ApplicationUser User { get; set; }

		[Required]
		public string DeviceId { get; set; }

		[Required]
		public string Token { get; set; }

		[Required]
		public DateTime ExpirationDate { get; set; }

		[Required]
		public AuthTokenType Type { get; set; }

        public bool IsExpired() => DateTime.Now > ExpirationDate;

		public AuthToken() { }

		public AuthToken(string deviceId, string token, ApplicationUser user, AuthTokenType type)
		{
			DeviceId = deviceId;
			Token = token;
			ExpirationDate = DateTime.Today.AddYears(10);
			User = user;
			Type = type;
		}

		public void Update(string deviceId, string token)
		{
			DeviceId = deviceId;
			Token = token;
			ExpirationDate = DateTime.Today.AddYears(10);
		}
    }
}
