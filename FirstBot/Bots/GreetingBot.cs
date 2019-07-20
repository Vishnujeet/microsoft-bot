using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FirstBot.Models;
using FirstBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace FirstBot.Bots
{
    public class GreetingBot:ActivityHandler
    {
        private readonly BotStateService _botStateService;

        public GreetingBot(BotStateService botStateService)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var userProfile= await _botStateService.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
            var conversationData =
                await _botStateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text($"Hi {userProfile.Name}. How can i help you today:"),
                    cancellationToken);
            }
            else
            {
                if (conversationData.PromptedUserForName)
                {
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thanks {userProfile.Name}. How can I help you today"),cancellationToken);
                    conversationData.PromptedUserForName = false;
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"),cancellationToken);
                    conversationData.PromptedUserForName = true;

                }
            }

            await _botStateService.UserProfileAccessor.SetAsync(turnContext, userProfile);
            await _botStateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

            await _botStateService.UserState.SaveChangesAsync(turnContext);
            await _botStateService.ConversationState.SaveChangesAsync(turnContext);
        }
    }
}
