namespace FitBitAssistantBot
{
    #region References
    using System;
    using System.Collections.Generic;
    #endregion

    public  class Constants
    {
        public const string ConnectionName = "FitBitApp";

        public const string LoginPromtName = "loginPrompt";

        public const string RootDialogName = "rootDialog";

        public const string WelcomeMessage = "Welcome to the world of FitBit. I am fitbit assistant bot. I will help you with your queries. This is What I can do";

        

        public const string CardImageUrl = "data:image/png;base64,{0}";

        #region UserActions

        enum UserActions
        {
            LogIn,
            MyProfile,
            Help,
            LogOut,
            MagicCode
        }

        #endregion

        #region FitBitApiUrls

        public const string UserProfileUrl  = @"https://api.fitbit.com/1/user/-/profile.json";

        public const string UserBadges = @"https://api.fitbit.com/1/user/-/badges.json";


        #endregion

        public const string AuthorizationToken = "Bearer {0}";



    }
}