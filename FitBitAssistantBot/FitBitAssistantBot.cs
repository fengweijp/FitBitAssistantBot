namespace FitBitAssistantBot
{
    #region  References
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;
    #endregion

    public class FitBitAssistantBot : IBot
    {
        private readonly FitBitAssistantBotAccessors _accessors;

        private readonly DialogSet _dialogs;
        public FitBitAssistantBot(FitBitAssistantBotAccessors accessors)
        {
            if(string.IsNullOrEmpty(Constants.ConnectionName))
            {
                throw new InvalidOperationException("Connection Name needs to be set in the constants class");
            }

            _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

            _dialogs = new DialogSet(_accessors.ConversationDialogState);
            _dialogs.Add(DialogHelpers.OAuthPrompt(Constants.ConnectionName));
//            _dialogs.Add(new ChoicePrompt("choicePrompt"));
            _dialogs.Add(new WaterfallDialog(Constants.RootDialogName, new WaterfallStep[] { PromptStepAsync, ProcessStepAsync }));



        }
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            DialogContext dc = null;

            switch(turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    await ProcessInputAsync(turnContext, cancellationToken);
                    break;


                case ActivityTypes.Event:

                    dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dc.ContinueDialogAsync(cancellationToken);
                    if (!turnContext.Responded)
                    {
                        await dc.BeginDialogAsync(Constants.RootDialogName, cancellationToken: cancellationToken);
                    }

                    break;

                case ActivityTypes.ConversationUpdate:
                    await DialogHelpers.SendWelcomeMessageAsync(turnContext, cancellationToken);
                    break;
                

            }
        }


        #region DialogTasks

        public async Task<DialogContext> ProcessInputAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            switch (turnContext.Activity.Text.ToLowerInvariant())
            {
                case "signout":
                case "logout":
                case "signoff":
                case "logoff":
                    // The bot adapter encapsulates the authentication processes and sends
                    // activities to from the Bot Connector Service.
                    var botAdapter = (BotFrameworkAdapter)turnContext.Adapter;
                    await botAdapter.SignOutUserAsync(turnContext, Constants.ConnectionName, cancellationToken: cancellationToken);

                    // Let the user know they are signed out.
                    await turnContext.SendActivityAsync("You are now signed out. Please close the chat window", cancellationToken: cancellationToken);
                    

                    break;
                case "help":

                    var reply = turnContext.Activity.CreateReply();
                    reply.Attachments = new List<Attachment>()
                    {
                        DialogHelpers.CreateBotHelpHeroCard().ToAttachment()
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken: cancellationToken);
                    await dc.EndDialogAsync(Constants.RootDialogName, cancellationToken);
                    break;

                default:
                    await dc.ContinueDialogAsync(cancellationToken);
                    if (!turnContext.Responded)
                    {
                        await dc.BeginDialogAsync(Constants.RootDialogName, cancellationToken: cancellationToken);
                    }

                    break;
                    
            }
            return dc;
        }

        private async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext step, CancellationToken cancellationToken)
        {
            var activity = step.Context.Activity;

            // Set the context if the message is not the magic code.
            if (activity.Type == ActivityTypes.Message &&
                !Regex.IsMatch(activity.Text, @"(\d{6})"))
            {
                await _accessors.CommandState.SetAsync(step.Context, activity.Text, cancellationToken);
                await _accessors.UserState.SaveChangesAsync(step.Context, cancellationToken: cancellationToken);
            }

            return await step.BeginDialogAsync(Constants.LoginPromtName, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessStepAsync(WaterfallStepContext step, CancellationToken cancellationToken)
        {
            if (step.Result != null)
            {
                var tokenResponse = step.Result as TokenResponse;

                // If we have the token use the user is authenticated so we may use it to make API calls.
                if (tokenResponse?.Token != null)
                {
                    var parts = _accessors.CommandState.GetAsync(step.Context, () => string.Empty, cancellationToken: cancellationToken).Result.Split(' ');
                    string command = parts[0].ToLowerInvariant();

                    if (command == "myprofile")
                    {
                        FitBitApiHelper fitBitApiHelper = new FitBitApiHelper(tokenResponse.Token);
                        UserProfile userProfile = await fitBitApiHelper.GetUserProfileAsync();
                        var reply = step.Context.Activity.CreateReply();
                        reply.Attachments = new List<Attachment>()
                        {
                             DialogHelpers.CreateUserProfileAdaptiveCard(userProfile)
                        };

                        await step.Context.SendActivityAsync(reply, cancellationToken);

                    }
                    else
                    {
                        await step.Context.SendActivityAsync($"Your token is: {tokenResponse.Token}", cancellationToken: cancellationToken);
                    }

                   
                }
                
            }
            else
            {
                await step.Context.SendActivityAsync("We couldn't log you in. Please try again later.", cancellationToken: cancellationToken);
            }
            await _accessors.CommandState.DeleteAsync(step.Context, cancellationToken);
            return await step.EndDialogAsync(cancellationToken: cancellationToken);
        }
            #endregion

        }
}