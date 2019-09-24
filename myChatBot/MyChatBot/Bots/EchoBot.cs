// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

/**
 https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-primitive-prompts?view=azure-bot-service-4.0&tabs=csharp
 Working on state at the moment

 for examples https://github.com/microsoft/BotBuilder-Samples.git

 https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0

 */
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using MyChatBot.Model;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Logging;

namespace MyChatBot.Bots {
	//public class EchoBot<T> : ActivityHandler where T : Dialog {
	public class EchoBot : ActivityHandler {
		private string task;// { get; set;}
								  //static readonly HttpClient client = new HttpClient();
		static string baseUrl = "https://bswcloud.bswapi.com/odata/";

	
		protected readonly BotState _conversationState;
		protected readonly BotState _userState;
	


		public EchoBot(ConversationState conversationState, UserState userState) {
			_conversationState = conversationState;
			_userState = userState;
		}

		//public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken)) {
		//	await base.OnTurnAsync(turnContext, cancellationToken);

		//	// Save any state changes that might have occured during the turn.
		//	await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
		//	await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
		//}

		private async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken) {
			foreach (var member in turnContext.Activity.MembersAdded) {
				if (member.Id != turnContext.Activity.Recipient.Id) {
					await DisplayFacilityChoicesAsync(turnContext, cancellationToken);
				}
			}
		}
		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken) {
			await SendWelcomeMessageAsync(turnContext, cancellationToken);

		}


		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken) {

			var conversationStateAccessors = _conversationState.CreateProperty<ConversationFlow>(nameof(ConversationFlow));
			var flow = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationFlow());


			await ProcessInput(flow, turnContext, cancellationToken);

			await _conversationState.SaveChangesAsync(turnContext);
			//await UpdateList(turnContext, cancellationToken);
			//await turnContext.SendActivityAsync($"Retriving List of {turnContext.Activity.Text}");
			//await turnContext.SendActivityAsync(reply, cancellationToken);
			//await ListHospitalsAsync(turnContext, cancellationToken, await GetList());
			//await ListFacillities(turnContext, cancellationToken);


		}

		private async Task ProcessInput(ConversationFlow flow, ITurnContext turnContext, CancellationToken cancellationToken) {
			var activity = turnContext.Activity;
			string input = activity.Text?.Trim();
			string message;
		
			//turnContext.Activity.Value;
			Context context = JsonConvert.DeserializeObject<Context>(activity.Value.ToString());
			flow.context = context;
			//IMessageActivity reply = MessageFactory.Text($"Message from ProcessInput loading {lu.Name} areas");
			if (flow.context.lookup == null) {
				await ProcessLookupItem(context, turnContext, cancellationToken);
			} else {
				await ProcessLookup(context,turnContext, cancellationToken);
			}


		}
		private async Task ProcessLookup(Context context, ITurnContext turnContext, CancellationToken cancellationToken) {

			switch (context.action) {
				case "prompt add":
					await PromptAddArea(context.lookup,turnContext, cancellationToken);
					break;
				case "add":
					break;
				case "list":
					await ListFacillities(context.lookup, turnContext, cancellationToken);
					break;
				case "update":
					break;
				case "delete":
					break;
				default:
					//await ManageFacillity(context.lookup, turnContext, cancellationToken);
					break;
			}
		}

		private async Task ProcessLookupItem(Context context, ITurnContext turnContext, CancellationToken cancellationToken) {
			switch (context.action) {
				case "manage facility":
					await ManageFacility(context.lookupItem, turnContext, cancellationToken);
					break;
				case "prompt update":
					break;
				case "prompt delete":
					break;

				case "add":
					break;
				case "list":
					await ListAreas(context.lookupItem, turnContext, cancellationToken);
					break;
				case "update":
					break;
				case "delete":
					break;
				default:
					await ManageArea(context.lookupItem, turnContext, cancellationToken);
					break;
			}
		}

		private async Task ManageFacility(LookupItem lui, ITurnContext turnContext, CancellationToken cancellationToken) {
			Lookup lu;
			List<CardAction> buttons = new List<CardAction>();
			string list = lui.Label;
			await turnContext.SendActivityAsync($"Retriving areas for [{list}]");
			string url = baseUrl + "Lookups?$filter=Description eq '" +("List of Areas for Hand Hygien facility " + list + "'").Replace(' ','+') + "&$expand=LookupItems";
			//string url = $"{baseUrl}Lookups({lui.LookupId})?$expand=LookupItems";
			await turnContext.SendActivityAsync($"{list} url {url}");
			StringBuilder json = new StringBuilder();
			oData odata;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json; charset=utf-8";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			//Movie m = JsonConvert.DeserializeObject<Movie>(json);
			using (var sr = new StreamReader(response.GetResponseStream())) {
				json.Append(sr.ReadToEnd());
				odata = JsonConvert.DeserializeObject<oData>(json.ToString());
				//var jo = JObject.Parse(json.ToString());
				lu = odata.Value[0];
			}

			foreach (LookupItem facility in lu.LookupItems) {
				buttons.Add(new CardAction(title: $"{facility.Label}", type: ActionTypes.MessageBack, value: new Context(action: "manage", lookupitem: facility), text: "LookupItem", displayText: $"List {facility.Label} areas."));
			}
			buttons.Add(new CardAction(title: $"ManageFacility Add a new area to {lu.Name}", type: ActionTypes.MessageBack, value: new Context(action: "prompt add", lookup: lu), displayText: $"Add {lu.Name}"));

			var card = new HeroCard {
				Title = $"{lu.Name}",
				Text = $"Select an area to manage or add a new one.",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);
		}

		private async Task PromptAddArea(Lookup lu, ITurnContext turnContext, CancellationToken cancellationToken) {
			await turnContext.SendActivityAsync($"What is the name of the new area for {lu.Name}?");
			//await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
		}
		private async Task ManageFacility(Lookup lu, ITurnContext turnContext, CancellationToken cancellationToken) {

		}
		private async Task ManageArea(LookupItem lui, ITurnContext turnContext, CancellationToken cancellationToken) {
			List<CardAction> buttons = new List<CardAction>();
			buttons.Add(new CardAction(title: $"Rename", type: ActionTypes.MessageBack, value: new Context(action: "prompt update", lookupitem: lui), displayText: $"Rename"));

			buttons.Add(new CardAction(title: $"Delete", type: ActionTypes.MessageBack, value: new Context(action: "prompt delete", lookupitem: lui), displayText: $"Delete"));


			var card = new HeroCard {
				Title = $"{lui.Label}",
				Text = $"What would you like to do?",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
		}

		private async Task PromptUpdateArea(LookupItem lui, ITurnContext turnContext, CancellationToken cancellationToken) {
			List<CardAction> buttons = new List<CardAction>();
			buttons.Add(new CardAction(title: $"Rename", type: ActionTypes.MessageBack, value: new Context(action: "prompt update", lookupitem: lui), displayText: $"Rename"));

			buttons.Add(new CardAction(title: $"Delete", type: ActionTypes.MessageBack, value: new Context(action: "prompt delete", lookupitem: lui), displayText: $"Delete"));


			var card = new HeroCard {
				Title = $"{lui.Label}",
				Text = $"What would you like to do?",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);
		}

		private async Task ListAreas(LookupItem lui, ITurnContext turnContext, CancellationToken cancellationToken) {
			Lookup lu;
			List<CardAction> buttons = new List<CardAction>();
			string list =lui.Label;
			await turnContext.SendActivityAsync($"Retriving areas for [{list}]");
			//string url = baseUrl + "Lookups?$filter=Description eq '" +("List of Areas for Hand Hygien facility " + list + "'").Replace(' ','+') + "&$expand=LookupItems";
			string url = $"{baseUrl}Lookups({lui.LookupId})";
			await turnContext.SendActivityAsync($"{list} url {url}");
			StringBuilder json = new StringBuilder();
			oData odata;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json; charset=utf-8";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			//Movie m = JsonConvert.DeserializeObject<Movie>(json);
			using (var sr = new StreamReader(response.GetResponseStream())) {
				json.Append(sr.ReadToEnd());
				odata = JsonConvert.DeserializeObject<oData>(json.ToString());
				//var jo = JObject.Parse(json.ToString());
				lu = odata.Value[0];
			}

			foreach (LookupItem facility in lu.LookupItems) {
				buttons.Add(new CardAction(title: $"{facility.Label}", type: ActionTypes.MessageBack, value: new Context(action:"manage",lookupitem:facility), text: "LookupItem", displayText: $"List {facility.Label} areas."));
			}
			buttons.Add(new CardAction(title: $"ListAreas Add a new area to {lu.Name}", type: ActionTypes.MessageBack, value: new Context(action:"prompt add",lookup:lu) , displayText: $"Add {lu.Name}"));

			var card = new HeroCard {
				Title = $"{lu.Name}",
				Text = $"{lu.Description}",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);

		}
		private async Task ListFacillities(Lookup lu, ITurnContext turnContext, CancellationToken cancellationToken) {
			//Lookup lu = JsonConvert.DeserializeObject<Lookup>(turnContext.Activity.Value.ToString());
			List<CardAction> buttons = new List<CardAction>();		

			foreach(LookupItem facility in lu.LookupItems) {
				buttons.Add(new CardAction(title: $"{facility.Label}", type: ActionTypes.MessageBack, value: new Context(action:"manage facility",lookupitem:facility), text: "LookupItem",  displayText: $"List {facility.Label} areas."));
			}
			buttons.Add(new CardAction(title: $"Add a new {lu.Name.Substring(0,lu.Name.Length - 1)}", type: ActionTypes.MessageBack, value: new Context("prompt add",lu), displayText: $"Add {lu.Name.Substring(0,lu.Name.Length - 1)}"));

			var card = new HeroCard {
				Title = $"{lu.Name}",
				Text = $"{lu.Description}",
				Buttons = buttons,
				Subtitle = "Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);
		}

		private async Task UpdateList(ITurnContext turnContext, CancellationToken cancellationToken) {
			string url = $"{baseUrl}Lookups(7)";



			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json; charset=utf-8";
			request.Method = "PUT";
			request.Accept = "application/json";
			Lookup facility = new Lookup("Temple Childrens", "List of Areas for Hand Hygien facility Temple Childrens");
			facility.Id = 7;




			byte[] bary = Encoding.UTF8.GetBytes(facility.serialize());
			request.ContentLength = bary.Length;
			Stream dataStream = request.GetRequestStream();
			dataStream.Write(bary, 0, bary.Length);
			dataStream.Close();
			try {
				WebResponse response = request.GetResponse();

				using (dataStream = response.GetResponseStream()) {
					StreamReader reader = new StreamReader(dataStream);
					string respString = reader.ReadToEnd();

				}
				response.Close();
			} catch (Exception e) {
				string msg = e.Message;
			}
			await turnContext.SendActivityAsync($"Added Temple Childrens");
		}

		private async Task AddList(ITurnContext turnContext, CancellationToken cancellationToken) {
			string url = $"{baseUrl}Lookups";



			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json; charset=utf-8";
			request.Method = "POST";
			request.Accept = "application/json";
			Lookup killeen = new Lookup("Killeen", "List of Areas for Hand Hygien facility Killeen");
	


			byte[] bary = Encoding.UTF8.GetBytes(killeen.serialize());
			request.ContentLength = bary.Length;
			Stream dataStream = request.GetRequestStream();
			dataStream.Write(bary, 0, bary.Length);
			dataStream.Close();
			try {
				WebResponse response = request.GetResponse();

				using (dataStream = response.GetResponseStream()) {
					StreamReader reader = new StreamReader(dataStream);
					string respString = reader.ReadToEnd();

				}
				response.Close();
			} catch (Exception e) {
				string msg = e.Message;
			}
			await turnContext.SendActivityAsync($"Added Temple Childrens");
		}

		private async Task DisplayFacilityChoicesAsync(ITurnContext turnContext, CancellationToken cancellationToken) {
			string url = $"{baseUrl}/Lookups?$filter=Name eq 'Hospitals' or Name eq 'Clinics'&$expand=LookupItems";
			StringBuilder json = new StringBuilder();
			
			oData odata;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ContentType = "application/json; charset=utf-8";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using (var sr = new StreamReader(response.GetResponseStream())) {
				json.Append(sr.ReadToEnd());
				odata = JsonConvert.DeserializeObject<oData>(json.ToString());
				//var jo = JObject.Parse(json.ToString());
				var label = odata.Value[0].Name;
			}


			List<CardAction> buttons = new List<CardAction>();
			foreach(Lookup lu in odata.Value) {
				buttons.Add(new CardAction(title: lu.Name, type: ActionTypes.MessageBack, value: new Context(action:"list",lookup:lu) , displayText: $"List {lu.Name} facilities."));
			}


			var card = new HeroCard { 
				Title = "Hospital or Clinic",
				Text = "Which type of facility do you need to manage?",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);
		}



	}
}
