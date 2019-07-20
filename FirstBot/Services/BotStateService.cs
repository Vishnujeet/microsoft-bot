using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace FirstBot.Services
{
    public class BotStateService
    {
        //state variables
        public ConversationState ConversationState { get; }
        public UserState UserState { get; }

        //ID
        public static string UserProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";
        public static string ConversationDataId { get; } = $"{nameof(BotStateService)}.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";

        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

        public BotStateService(UserState userState,ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentException(nameof(userState));
            InitializeAccessors();
        }

        public void InitializeAccessors()
        {
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
            UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);
        }

    }
}
