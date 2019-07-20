using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace FirstBot.Helpers
{
    public static class DialogExtensions
    {
        public static async Task Run(this Dialog dialog, ITurnContext turnContext,
            IStatePropertyAccessor<DialogState> accessor, CancellationToken cancellationToken)
        {
            var dialogset=new DialogSet(accessor);
            dialogset.Add(dialog);
            var dialogContext = await dialogset.CreateContextAsync(turnContext, cancellationToken);
            var results = await dialogContext.ContinueDialogAsync(cancellationToken);
            if (results.Status == DialogTurnStatus.Empty)
            {
                await dialogContext.BeginDialogAsync(dialog.Id, null, cancellationToken);
            }

        }
    }
}
