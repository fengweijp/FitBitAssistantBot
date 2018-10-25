namespace FitBitAssistantBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class UserBadges
    {

        public List<Badge> badges { get; set; }
    }

    /// <summary>
    /// Class representing individual badge
    /// </summary>
    public class Badge
    {
        public string badgeGradientEndColor { get; set; }
        public string badgeGradientStartColor { get; set; }
        public string badgeType { get; set; }
        public string category { get; set; }
        public List<object> cheers { get; set; }
        public string dateTime { get; set; }
        public string description { get; set; }
        public string earnedMessage { get; set; }
        public string encodedId { get; set; }
        public string image100px { get; set; }
        public string image125px { get; set; }
        public string image300px { get; set; }
        public string image50px { get; set; }
        public string image75px { get; set; }
        public string marketingDescription { get; set; }
        public string mobileDescription { get; set; }
        public string name { get; set; }
        public string shareImage640px { get; set; }
        public string shareText { get; set; }
        public string shortDescription { get; set; }
        public string shortName { get; set; }
        public int timesAchieved { get; set; }
        public int value { get; set; }
        public string unit { get; set; }
    }


}
