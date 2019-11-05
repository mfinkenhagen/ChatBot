using System;

namespace MyChatBot.Model {
	public class LookupItem {

		public int Id { get; set; }
		public string Label { get; set; }
		public Nullable<int> Sequence { get; set; }
		public string Abbr { get; set; }
		public string Value { get; set; }
		public bool Active { get; set; }
		public int LookupId { get; set; }
	}
}
