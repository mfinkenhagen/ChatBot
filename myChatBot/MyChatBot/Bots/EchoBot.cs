// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;


namespace MyChatBot.Bots
{
    
    public class EchoBot : ActivityHandler
    {


        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendWelcomeMessageAsync(turnContext, cancellationToken);

        }


        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = ProcessInput(turnContext);
            await turnContext.SendActivityAsync($"Retriving List of {turnContext.Activity.Text}");
            await turnContext.SendActivityAsync(reply, cancellationToken);
            await DisplayOptionsAsync(turnContext, cancellationToken);

            /* From initila EchoBot 
            IMessageActivity act = turnContext.Activity;
            await turnContext.SendActivityAsync(MessageFactory.Text($"> {turnContext.Activity.Text}"), cancellationToken);
            */
        }

        private static IMessageActivity ProcessInput(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            IMessageActivity reply = MessageFactory.Text("Message from ProcessInput");
            return reply;
            
        }


        private static async Task DisplayOptionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            { 
                Title= "Hospital or Clinic",
                Text = "This is a Hero Card asking which facility catigory we are going to be working with",
                Buttons = new List<CardAction>
                {
                    new CardAction(title: "Hospitals", type: ActionTypes.ImBack, value: "Hospitals"),
                    new CardAction(title: "Clinics", type: ActionTypes.ImBack, value: "Clinics"),
                },
                Subtitle=" Pick one",
            };
            IMessageActivity reply = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }


        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    Debug.WriteLine("*** EchoBot OnMembersAddedAsync call SendActivityAsync");

                    List<CardAction> actions = new List<CardAction>();
                    actions.Add(new CardAction(title: "Hospitals", type: ActionTypes.ImBack, value: "Hospitals"));
                    actions.Add(new CardAction(title: "Clinics", type: ActionTypes.ImBack, value: "Clinics"));

                    await turnContext.SendActivityAsync(MessageFactory.SuggestedActions(actions, text: "Hello, wich facility catigory are we going to be working with?"), cancellationToken);
                }
            }
        }
    }
}
