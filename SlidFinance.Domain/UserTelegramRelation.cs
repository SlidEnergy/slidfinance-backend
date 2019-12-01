using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.Domain
{
	public class UserTelegramRelation
	{
		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }

		public long? TelegramChatId { get; set; }
	}
}
