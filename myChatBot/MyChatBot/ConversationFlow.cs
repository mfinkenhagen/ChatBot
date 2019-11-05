using MyChatBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChatBot.Bots {

	public class ConversationFlow{
		public Context context { get; set; }
	
		 // Identifies the last question asked.
		 public enum Question{
			  Name,
			  Age,
			  Date,
			  None, // Our last action did not involve a question.
		 }

		 // The last question asked.
		 public Question LastQuestionAsked { get; set; } = Question.None;

	}
}
