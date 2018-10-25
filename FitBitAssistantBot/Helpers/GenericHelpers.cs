namespace FitBitAssistantBot
{
    #region References
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    #endregion

    public class GenericHelpers
    {
        private static string _imagePath;
        public static string ConvertImageToBase64String(string imagePath)
        {
            _imagePath = imagePath ?? throw new ArgumentException($"Image Path cannot be null");

            byte[] imageDataArray = File.ReadAllBytes(_imagePath);

            return System.Convert.ToBase64String(imageDataArray);
        }

        public static string GetAuzthorizationToken(string token)
        {
            return string.Format(Constants.AuthorizationToken, token);
        }
    }


}