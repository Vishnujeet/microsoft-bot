using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FirstBot.Services;
using Microsoft.Bot.Builder.Dialogs;

namespace FirstBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        public MainDialog(BotStateService botStateService) : base(nameof(MainDialog))
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };
            AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", _botStateService));
            AddDialog(new AppBotDialog($"{nameof(MainDialog)}.sportsWeb", _botStateService));

            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hi").Success) 
            {
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.sportsWeb", null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}
