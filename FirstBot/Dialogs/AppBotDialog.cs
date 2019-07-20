using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FirstBot.Models;
using FirstBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace FirstBot.Dialogs
{
    public class AppBotDialog:ComponentDialog
    {

        private readonly BotStateService _botStateService;
        public AppBotDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();

        }

        private void InitializeWaterfallDialog()
        {
            var waterfallDialog = new WaterfallStep[]
            {              
                GetSportsSiteAsync,
                GetSportsWebFieldAsync,
                SportsWebFieldLightTurnOn
            };
            AddDialog(new WaterfallDialog($"{nameof(AppBotDialog)}.mainFlow", waterfallDialog));
            AddDialog(new ChoicePrompt($"{nameof(AppBotDialog)}.PlaySite"));
            AddDialog(new ChoicePrompt($"{nameof(AppBotDialog)}.PlayField"));
        }

        private async Task<DialogTurnResult>GetSportsSiteAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var allSite = WebServices.GetSites();
            var siteName = allSite.Select(x => x.Name).ToList();
            return await stepContext.PromptAsync($"{nameof(AppBotDialog)}.PlaySite",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please select a play Site"),
                    Choices = ChoiceFactory.ToChoices(siteName)
                }, cancellationToken);

        }

        private async Task<DialogTurnResult> GetSportsWebFieldAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            stepContext.Values["PlaySite"] = ((FoundChoice)stepContext.Result).Value;
            var allFields = WebServices.GetSites().Where(x => x.Name == stepContext.Values["PlaySite"].ToString()).Select(x => x.Fields).ToList();
            var fields = allFields.Select(x => x.Select(y => y.Name)).FirstOrDefault();
            return await stepContext.PromptAsync($"{nameof(AppBotDialog)}.PlayField",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please select a play field"),
                    Choices = ChoiceFactory.ToChoices(fields.ToList())
                }, cancellationToken);

        }

        private async Task<DialogTurnResult> SportsWebFieldLightTurnOn(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            stepContext.Values["PlayField"] = ((FoundChoice)stepContext.Result).Value;
            var allFields = WebServices.GetSites().FirstOrDefault(x => x.Name == stepContext.Values["PlaySite"].ToString())?.Fields;
            var selectedfield = allFields.Where(x => x.Name == stepContext.Values["PlayField"].ToString()).Select(y => y.Id).FirstOrDefault();
          
            //var /*result*/ = SportsWebServices.SetFieldState(selectedfield);
            var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context,
                () => new UserProfile(), cancellationToken);
            await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            return await stepContext.PromptAsync($"{nameof(AppBotDialog)}.PlaySite",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Thank You, Field has been set to match mode!"),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> SetSportsWebReturnStatusAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            stepContext.Values["PlayField"] = ((FoundChoice)stepContext.Result).Value;

            var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context,
                () => new UserProfile(), cancellationToken);

            userProfile.PlaySite = (string)stepContext.Values["PlaySite"];
            userProfile.PlayField = (string)stepContext.Values["PlayField"];

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"PlaySite:https:/temp/state"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"PlaySite : {userProfile.PlaySite}"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"PlayField : {userProfile.PlayField}"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"PlayFieldStatus : On"), cancellationToken);

            await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

     

    }
}

