using System.Collections.Generic;

namespace MyChatBot.Model {
	public class Lookup {
		public List<LookupItem> LookupItems { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public Lookup(string Name,string Description) {
			this.Name = Name;
			this.Description = Description;
		}
		public string serialize() {
			string id = "";
			if (this.Id != 0) {
				id = $"\"Id\":{this.Id},";
			}
			return "{" + $"{id}\"Name\":\"{this.Name}\",\"Description\":\"{this.Description}\"" + "}";
		}
	}
}
