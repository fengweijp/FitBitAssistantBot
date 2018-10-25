namespace FitBitAssistantBot
{
    #region  References
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    #endregion


    /// <summary>
    /// Helper Class To Consume FitBit Web Api
    /// </summary>
    public class FitBitApiHelper
    {
        private readonly string _token;

        public FitBitApiHelper(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }
            _token = token;
        }

        /// <summary>
        /// Get the User Data
        /// </summary>
        /// <returns>UserProfile Object</returns>
        public async Task<UserProfile> GetUserProfileAsync()
        {

            string jsonResponse = await GetFitBitDataAsync(Constants.UserProfileUrl);
            UserProfile userProfile = JsonConvert.DeserializeObject<UserProfile>(jsonResponse);
            return userProfile;
            
        }

        /// <summary>
        /// Get the User Badges
        /// </summary>
        /// <returns>UserBadges Object</returns>
        public async Task<UserBadges> GetUserBadgesAsync()
        {
            string jsonResponse = await GetFitBitDataAsync(Constants.UserBadges);
            UserBadges userBadges = JsonConvert.DeserializeObject<UserBadges>(jsonResponse);
            return userBadges;
        }

        /// <summary>
        /// Get Data From FitBitAPi
        /// </summary>
        /// <param name="url">url to get the data from</param>
        /// <returns></returns>
        private async Task<string> GetFitBitDataAsync(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", GenericHelpers.GetAuzthorizationToken(_token));

            using (var response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Fitbitapi returned invalid success code: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                return jsonResponse;

            }


        }


    }
}