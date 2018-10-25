namespace FitBitAssistantBot
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json.Linq;
    #endregion

    public static class DialogHelpers
    {
        #region Cards
        public static HeroCard CreateWelcomeHeroCard(string newlyAddedUserName)
        {
            var heroCard = new HeroCard($"Welcome {newlyAddedUserName}", "FitBtAssistantBot")
            {
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "LogIn", text:"LogIn", displayText: "LogIn", value: "LogIn"),
                    new CardAction(ActionTypes.ImBack, "MyProfile", text:"MyProfile", displayText: "MyProfile", value: "MyProfile"),
                    new CardAction(ActionTypes.ImBack, "Help", text: "Help", displayText: "Help", value:"Help"),
                    new CardAction(ActionTypes.ImBack, "LogOut", text: "LogOut", displayText: "LogOut", value: "LogOut")
                },
                Images = new List<CardImage>()
                {
                    new CardImage()
                    {
                        Url = string.Format(Constants.CardImageUrl,GenericHelpers.ConvertImageToBase64String(@".\Images\BotPic.png"))
                    }
                }

              
            };

            return heroCard;
            
        }
        public static HeroCard CreateBotHelpHeroCard()
        {
            var heroCard = new HeroCard($"Click on any one to begin", "FitBtAssistantBot")
            {
                Buttons = new List<CardAction>
                {
                    
                    new CardAction(ActionTypes.ImBack, "MyProfile", text:"MyProfile", displayText: "MyProfile", value: "MyProfile"),
                    new CardAction(ActionTypes.ImBack, "LogOut", text: "LogOut", displayText: "LogOut", value: "LogOut")
                }
               

            };

            return heroCard;

        }
        public static HeroCard CreateErrorHeroCard()
        {
            var heroCard = new HeroCard($"Sorry, seems I have run into an issue. Please close the chat and try again", "FitBitAssistantBot")
            {
                Images = new List<CardImage>()
                {
                    new CardImage ()
                    {
                        //Reading Error Image from Local folders in solutiom
                        Url = string.Format(Constants.CardImageUrl,GenericHelpers.ConvertImageToBase64String(@".\Images\BrokenBot.png"))
                    }
                }
            };

            return heroCard;

        }

        public static Attachment CreateUserProfileAdaptiveCard(UserProfile userProfile)
        {
            

            string jsonAdaptiveCard = File.ReadAllText(@".\REsources\AdaptiveCards\UserProfileAdaptiveCard.json");
            string fullName = userProfile.user.fullName;
            string age = userProfile.user.age.ToString();
            DateTime tempDateOfBirth = DateTime.ParseExact(userProfile.user.dateOfBirth, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            string dateOfBirth = tempDateOfBirth.ToString("dd MMM, yyyy");
            string gender = userProfile.user.gender;
            string height = $"{userProfile.user.height} cms";
            DateTime tempMemberSince= DateTime.ParseExact(userProfile.user.memberSince, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            string memberSince = tempMemberSince.ToString("dd MMM, yyyy");

            Dictionary<string, string> replaceMap = new Dictionary<string, string>()
            {
                { "{0}", userProfile.user.avatar },
                { "{1}", fullName},
                { "{2}", gender},
                { "{3}", height},
                { "{4}", age},
                { "{5}", dateOfBirth},
                { "{6}", memberSince }
            };

            foreach (var entry in replaceMap)
                jsonAdaptiveCard = jsonAdaptiveCard.Replace(entry.Key, entry.Value);
            



            //jsonAdaptiveCard = string.Format(jsonAdaptiveCard, userProfile.user.avatar, fullName, gender, height, age, dateOfBirth, memberSince);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(jsonAdaptiveCard),
            };
            return adaptiveCardAttachment;

        }

        public static OAuthCard CreateOuathCard(string connectionName, string userName)
        {
            var oauthCard = new OAuthCard(string.Format(Constants.WelcomeMessage, userName))
            {
                ConnectionName = connectionName ?? throw new ArgumentNullException("Connection Name cannot be blank."),
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.Signin, "SignIn",text: "SignIn", displayText: "SignIn", value: "SignIn" )

                }

            };

            return oauthCard;

        }

        public static OAuthPrompt OAuthPrompt(string connectionName)
        {
            var oauthPrompt = new OAuthPrompt(
                Constants.LoginPromtName, new OAuthPromptSettings
                {
                    ConnectionName = connectionName ?? throw new ArgumentNullException("Connection Name cannot be blank."),
                    Text = "Please Sign In",
                    Timeout = 300000,
                    Title = "SignIn"    
                }

            );

            return oauthPrompt;
            
        }

        

        #endregion


        #region Tasks

        public static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach(var member in turnContext.Activity.MembersAdded)
            {
                if(member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = turnContext.Activity.CreateReply();
                    reply.Text = Constants.WelcomeMessage;
                    reply.Attachments = new List<Attachment>()
                    {
                        CreateWelcomeHeroCard(member.Name).ToAttachment()
                    };

                    await turnContext.SendActivityAsync(reply, cancellationToken);
                     
                }
            }
        }

        public static async Task SenErrorMessageAsync(ITurnContext turnContext)
        {
            var reply = turnContext.Activity.CreateReply();
            reply.Attachments = new List<Attachment>()
            {
                CreateErrorHeroCard().ToAttachment()
            };

            await turnContext.SendActivityAsync(reply);
        }
        #endregion

    }
}