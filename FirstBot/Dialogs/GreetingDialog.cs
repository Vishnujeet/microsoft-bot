using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FirstBot.Models;
using FirstBot.Services;
using Microsoft.AspNetCore.WebSockets.Internal;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace FirstBot.Dialogs
{
    public class GreetingDialog : ComponentDialog
    {
        protected readonly BotStateService _botStateService;
        public GreetingDialog(string dialogId,BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps=new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };
            AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));
            InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";
        }
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile =
                await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.name",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What is your name?")
                    },cancellationToken);

            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile =
                await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                userProfile.Name = (string) stepContext.Result;
                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            }

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Hi {userProfile.Name}. How can I help you today?"), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
