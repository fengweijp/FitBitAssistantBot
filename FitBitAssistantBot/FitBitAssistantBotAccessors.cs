namespace FitBitAssistantBot
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    #endregion

    #region  Classes

    public class FitBitAssistantBotAccessors
    {
        public FitBitAssistantBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(ConversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(UserState));
        }
        public static readonly string DialogStateName = $"{nameof(FitBitAssistantBotAccessors)}.DialogState";

        public static readonly string CommandStateName = $"{nameof(FitBitAssistantBotAccessors)}.CommandState";
        public IStatePropertyAccessor<DialogState> ConversationDialogState {get; set;}

        public IStatePropertyAccessor<string> CommandState {get; set;}

        public ConversationState ConversationState {get;}

        public UserState UserState {get;}

        
    }
    #endregion


}