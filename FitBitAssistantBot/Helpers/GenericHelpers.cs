namespace FitBitAssistantBot
{
    #region References
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    using System.Text.RegularExpressions;
    #endregion

    public class GenericHelpers
    {
        private static string _filePath;

        /// <summary>
        /// Convert the resource located at inout path to a base 64 string
        /// </summary>
        /// <param name="filePath">Path to the resource</param>
        /// <returns></returns>
        public static string ConvertResourceToBase64String(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentException($"Image Path cannot be null");

            byte[] imageDataArray = File.ReadAllBytes(_filePath);

            return System.Convert.ToBase64String(imageDataArray);
        }


        /// <summary>
        /// Format the Authorization Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetAuzthorizationToken(string token)
        {
            return string.Format(Constants.AuthorizationToken, token);
        }


        /// <summary>
        /// Check if the command sent to the bot is a magic code.
        /// </summary>
        /// <param name="commandState">Value of the command sent with the activity</param>
        /// <returns></returns>
        public static bool IsMagicCode(string commandState)
        {
            return Regex.IsMatch(commandState, @"(\d{6})");
        }

        public static string ParseCommand(string command)
        {
            if (IsMagicCode(command))
                return "magiccode";
            else
                return command;

        }
    }


}