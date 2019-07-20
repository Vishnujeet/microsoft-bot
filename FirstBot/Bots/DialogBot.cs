﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FirstBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace FirstBot.Bots
{
    public class DialogBot<T>:ActivityHandler where T: Dialog
    {
        protected readonly Dialog _dialog;
        protected readonly BotStateService _botStateService;
        protected readonly ILogger _logger;

        public DialogBot(BotStateService botStateService, T dialog, ILogger<DialogBot<T>> logger)
        {
            _botStateService = botStateService ?? throw new System.ArgumentException(nameof(botStateService));
            _dialog = dialog ?? throw new System.ArgumentNullException(nameof(dialog));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            await _botStateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _botStateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with message activity.");
            await _dialog.RunAsync(turnContext, _botStateService.DialogStateAccessor, cancellationToken);
        }

    }
}
