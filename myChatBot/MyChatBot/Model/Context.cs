using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChatBot.Model {
	public class Context {
		public Lookup lookup { get; set; } 
		public LookupItem lookupItem { get; set; }
		public string action { get; set; }
		public Context() { }
		public Context(string action, Lookup lookup = null, LookupItem lookupitem = null) {
			this.lookup = lookup;
			this.action = action;
			this.lookupItem = lookupitem;
		}
	}
}
