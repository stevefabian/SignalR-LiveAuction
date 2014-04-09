using System;

namespace LiveAuctionWinPhone8.Models
{
    public class ItemModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public decimal reserve { get; set; }
        public decimal price { get; set; }
        public string bidder { get; set; }
        public string status { get; set; }
        public int duration { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
}