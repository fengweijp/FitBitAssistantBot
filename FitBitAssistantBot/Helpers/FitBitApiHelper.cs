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

        public async Task<UserProfile> GetUserProfileAsync()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", GenericHelpers.GetAuzthorizationToken(_token));

            using ( var response = await client.GetAsync(Constants.UserProfileUrl))
            {
                if(!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Fitbitapi returned invalid success code: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                UserProfile userProfile = JsonConvert.DeserializeObject<UserProfile>(jsonResponse);

                return userProfile;

                
            }
        }




    }
}