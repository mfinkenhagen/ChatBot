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
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            IMessageActivity act = turnContext.Activity;
            await turnContext.SendActivityAsync(MessageFactory.Text($"> {turnContext.Activity.Text}"), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
           
            foreach (var member in membersAdded)
            {
                 if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    Debug.WriteLine("*** EchoBot OnMembersAddedAsync call SendActivityAsync");

                    List<CardAction> actions = new List<CardAction>();
                    actions.Add(new CardAction(title: "IT Knowledge", type: ActionTypes.ImBack, value: "IT"));
                    actions.Add(new CardAction(title: "IT Public Knowledge", type: ActionTypes.ImBack, value: "IT Public"));
                    actions.Add(new CardAction(title: "PeoplePlace Knowledge", type: ActionTypes.ImBack, value: "PeoplePlace"));
                    actions.Add(new CardAction(title: "Runbook Knowledge", type: ActionTypes.ImBack, value: "Runbook"));
                    actions.Add(new CardAction(title: "Supply Chain Public Knowledge", type: ActionTypes.ImBack, value: "Supply Chain Public"));
                    actions.Add(new CardAction(title: "Vulnerabilities", type: ActionTypes.ImBack, value: "Vulnerabilities"));
                    actions.ToArray();

                    await turnContext.SendActivityAsync(MessageFactory.SuggestedActions(actions, text:"Select a knowledge base"), cancellationToken);
                    
                }
            }
        }
    }
}
