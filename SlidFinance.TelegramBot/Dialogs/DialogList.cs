using Microsoft.Bot.Builder.Dialogs;
using SlidFinance.TelegramBot.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.TelegramBot.Dialogs
{
	public class DialogList
	{
		private Dictionary<string, Dialog> dialogList;

		//public IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

		public DialogList(
			WhichCardToPayDialog whichCardToPayDialog
		) {
			dialogList = new Dictionary<string, Dialog>() {
				{ "/whichcardtopay", whichCardToPayDialog }
			};
		}

		public Dialog Get(string command)
		{
			return dialogList[command];
		}
	}
}
