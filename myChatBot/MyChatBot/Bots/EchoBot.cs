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
using System;

namespace MyChatBot.Bots {

	public class EchoBot : ActivityHandler {
		private string task;// { get; set;}
								  //static readonly HttpClient client = new HttpClient();
		static string baseUrl = "https://bswcloud.bswapi.com/odata/";
		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken) {
			await SendWelcomeMessageAsync(turnContext, cancellationToken);

		}


		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken) {
			await ProcessInput(turnContext, cancellationToken);
			//await UpdateList(turnContext, cancellationToken);
			//await turnContext.SendActivityAsync($"Retriving List of {turnContext.Activity.Text}");
			//await turnContext.SendActivityAsync(reply, cancellationToken);
			//await ListHospitalsAsync(turnContext, cancellationToken, await GetList());
			//await ListFacillities(turnContext, cancellationToken);


		}

		private static async Task ProcessInput(ITurnContext turnContext, CancellationToken cancellationToken) {
			var activity = turnContext.Activity;
			//turnContext.Activity.Value;
			Lookup lu = JsonConvert.DeserializeObject<Lookup>(activity.Value.ToString());
			LookupItem lui = null;
			IMessageActivity reply = MessageFactory.Text($"Message from ProcessInput loading {lu.Name} areas");
			if (lu.Name == null) {
				lui = JsonConvert.DeserializeObject<LookupItem>(activity.Value.ToString());
				await ListAreas(lui, turnContext, cancellationToken);
			} else {
				await ListFacillities(turnContext, cancellationToken);
			}


		}



		private static async Task ListAreas(LookupItem lui, ITurnContext turnContext, CancellationToken cancellationToken) {
			Lookup lu;
			List<CardAction> buttons = new List<CardAction>();
			string list =lui.Label;
			await turnContext.SendActivityAsync($"Retriving areas for [{list}]");
			string url = baseUrl + "Lookups?$filter=Description eq '" +("List of Areas for Hand Hygien facility " + list + "'").Replace(' ','+') + "&$expand=LookupItems";
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
				buttons.Add(new CardAction(title: $"{facility.Label}", type: ActionTypes.MessageBack, value: facility, text: "LookupItem", displayText: $"List {facility.Label} areas."));
			}
			buttons.Add(new CardAction(title: $"Add a new {lu.Name.Substring(0, lu.Name.Length - 1)}", type: ActionTypes.ImBack, value: $"Add {lu.Name.Substring(0, lu.Name.Length - 1)}"));

			var card = new HeroCard {
				Title = $"{lu.Name}",
				Text = $"{lu.Description}",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);

		}
		private static async Task ListFacillities(ITurnContext turnContext, CancellationToken cancellationToken) {
			Lookup lu = JsonConvert.DeserializeObject<Lookup>(turnContext.Activity.Value.ToString());
			List<CardAction> buttons = new List<CardAction>();		

			//string list = turnContext.Activity.Text.Substring(5);
			//string url = $"{baseUrl}/Lookups?$filter=Name eq '{list}'&$expand=LookupItems";
			//StringBuilder json = new StringBuilder();
			//oData odata;
			//HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			//request.ContentType = "application/json; charset=utf-8";
			//HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			////Movie m = JsonConvert.DeserializeObject<Movie>(json);
			//using (var sr = new StreamReader(response.GetResponseStream())) {
			//	json.Append(sr.ReadToEnd());
			//	odata = JsonConvert.DeserializeObject<oData>(json.ToString());
			//	//var jo = JObject.Parse(json.ToString());
			//	lu = odata.Value[0];
			//}

			foreach(LookupItem facility in lu.LookupItems) {
				buttons.Add(new CardAction(title: $"{facility.Label}", type: ActionTypes.MessageBack, value: facility, text: "LookupItem",  displayText: $"List {facility.Label} areas."));
			}
			buttons.Add(new CardAction(title: $"Add a new {lu.Name.Substring(0,lu.Name.Length - 1)}", type: ActionTypes.ImBack, value: $"Add {lu.Name.Substring(0,lu.Name.Length - 1)}"));

			var card = new HeroCard {
				Title = $"{lu.Name}",
				Text = $"{lu.Description}",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);
		}


		/*
		private static async Task ListAreas(LookupItem lui, ITurnContext turnContext, CancellationToken cancellationToken) {
			string url = $"{baseUrl}/Lookups?$filter=Description eq 'List of Areas for Hand Hygien facility {lui.Label}'";
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
			foreach (Lookup lu in odata.Value) {
				buttons.Add(new CardAction(title: lu.Name, type: ActionTypes.MessageBack, value: lu, displayText: $"List {lu.Name} facilities."));
			}
			var card = new HeroCard {
				Title = $"Areas for {lui.Label}",
				Text = "Which area do you need to manage?",
				Buttons = buttons,
				Subtitle = " Pick one",
			};
			IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
			await turnContext.SendActivityAsync(reply, cancellationToken);

		}
		*/


		private static async Task UpdateList(ITurnContext turnContext, CancellationToken cancellationToken) {
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

		private static async Task AddList(ITurnContext turnContext, CancellationToken cancellationToken) {
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
				buttons.Add(new CardAction(title: lu.Name, type: ActionTypes.MessageBack, value: lu , displayText: $"List {lu.Name} facilities."));
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
