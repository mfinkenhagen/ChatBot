// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0
using Microsoft.Bot.Builder;
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

namespace MyChatBot.Bots {

	public class EchoBot : ActivityHandler {
		private string task;// { get; set;}
								  //static readonly HttpClient client = new HttpClient();
		static string baseUrl = "https://bswcloud.bswapi.com/odata/";
		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken) {
			await SendWelcomeMessageAsync(turnContext, cancellationToken);

		}


		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken) {
			var reply = ProcessInput(turnContext);
			await turnContext.SendActivityAsync($"Retriving List of {turnContext.Activity.Text}");
			await turnContext.SendActivityAsync(reply, cancellationToken);
			//await ListHospitalsAsync(turnContext, cancellationToken, await GetList());
			await ListFacillities(turnContext, cancellationToken);


		}

		private static async Task ListFacillities(ITurnContext turnContext, CancellationToken cancellationToken) {
			string list = turnContext.Activity.Text.Substring(5);
			string url = $"{baseUrl}/Lookups?$filter=Name eq '{list}'&$expand=LookupItems";
			StringBuilder json = new StringBuilder();
			oData odata;
			Lookup lu;
			List<CardAction> buttons = new List<CardAction>();		

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

			foreach(LookupItem facility in lu.LookupItems) {
				buttons.Add(new CardAction(title: $"{facility.Label}", type: ActionTypes.ImBack, value: $"{facility.Label}"));
			}
			buttons.Add(new CardAction(title: $"Add a new {lu.Name.Substring(lu.Name.Length - 1)}", type: ActionTypes.ImBack, value: $"Add {lu.Name.Substring(lu.Name.Length - 1)}"));

			var card = new HeroCard {
				Title = $"{lu.Name}",
				Text = $"{lu.Description}",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);
		}


		private static IMessageActivity ProcessInput(ITurnContext turnContext) {
			var activity = turnContext.Activity;
			IMessageActivity reply = MessageFactory.Text($"Message from ProcessInput loading {activity.Text} areas");
			return reply;

		}


		private static async Task DisplayFacilityChoicesAsync(ITurnContext turnContext, CancellationToken cancellationToken) {
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
				buttons.Add(new CardAction(title: lu.Name, type: ActionTypes.ImBack, value: $"List {lu.Name}"));
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


		private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken) {
			foreach (var member in turnContext.Activity.MembersAdded) {
				if (member.Id != turnContext.Activity.Recipient.Id) {
					await DisplayFacilityChoicesAsync(turnContext, cancellationToken);
				}
			}
		}
	}
}
