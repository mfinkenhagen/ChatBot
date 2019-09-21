using System.Collections.Generic;

namespace MyChatBot.Model {
	public class Lookup {
		public List<LookupItem> LookupItems { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
