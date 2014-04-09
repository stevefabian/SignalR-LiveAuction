/*
 * In the Application_Start method, or somewhere in app's start
 * you'll need to add the namespace inclusion:
 * 
 * using Microsoft.AspNet.SignalR;
 * 
 * and the code:
 * 
 * RouteTable.Routes.MapHubs();
 * 
 * */

using LiveAuction.APIs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Globalization;

namespace LiveAuction.SignalR
{
    [HubName("myHub")]
    public class AuctionHub : Hub
    {
        public void Send(string user, string msg)
        {
            Clients.All.Message(string.Format("{0}: {1}", user, msg));
        }

        public void Bid(string user, int itemid, string amount)
        {
            var ic = new ItemsController();
            var item = ic.GetItems("all").Find(x => x.id == itemid);
            if (item != null)
            {
                decimal value;
                if (decimal.TryParse(amount, NumberStyles.Currency, 
                    CultureInfo.CurrentCulture.NumberFormat, out value))
                {
                    item.price = value;
                    item.bidder = user;
                    var msg = string.Format("bid {0} for {1}", amount, item.name);
                    Send(user, msg);
                    Clients.All.Reload();
                }

            }
        }
    }
}