using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.TelegramBot.Models.Commands
{
	public class CommandList
	{
		private List<Command> commandsList;
		public IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

		public CommandList(
			StartCommand start,
			WhichCardToPayCommand whichToPay,
			GetCategoryStatisticCommand getCategoryStatistic
		) {
			commandsList = new List<Command>() {
				start,
				whichToPay,
				getCategoryStatistic
			};
		}
	}
}
