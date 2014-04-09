using LiveAuctionWin8.Models;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveAuctionWin8.ViewModels
{
    public class ViewModel
    {
        private HubConnection _hubConnection;
        private IHubProxy _hubProxy;
        public SynchronizationContext Context { get; set; }
        public string UserName { get; set; }
        public bool IsHubStarted { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public ObservableCollection<Models.ItemModel> Items { get; set; }

        public ViewModel()
        {
            Messages = new ObservableCollection<string>();
            Items = new ObservableCollection<Models.ItemModel>();
            UserName = "not set";
            IsHubStarted = false;
        }

        public async void StartSignalRHub()
        {
            _hubConnection = new HubConnection("http://192.168.0.30:55720/");
            _hubProxy = _hubConnection.CreateHubProxy("MyHub");
            Context = SynchronizationContext.Current;

            await _hubConnection.Start();
            IsHubStarted = true;
            if (this.UserName == null)
            {
                this.UserName = "not set";
            }
            await _hubProxy.Invoke("Send", new object[] { this.UserName, "Has joined the Auction" });

            _hubProxy.On("Reload", () => 
                Context.Post(delegate {LoadItems();}, null));

            _hubProxy.On<string>("Message", (msg) =>
                Context.Post(delegate {
                    this.Messages.Add(msg);
                }, null));
        }

        public async void SendMessage(string msg)
        {
            await _hubProxy.Invoke("Send", this.UserName, msg);
        }

        public async void LoadItems()
        {
            HttpClient webClient = new HttpClient();
            webClient.BaseAddress = new Uri(@"http://192.168.0.30:55720/");
            HttpResponseMessage response = await webClient.GetAsync("api/items/started");
            var resp = await response.Content.ReadAsStringAsync();

            this.Items.Clear();
            var auctionItems = JsonConvert.DeserializeObject<ItemModel[]>(resp);
            foreach (ItemModel auctionItem in auctionItems)
            {
                this.Items.Add(new ItemModel()
                {
                    id = auctionItem.id,
                    name = auctionItem.name,
                    desc = auctionItem.desc,
                    price = auctionItem.price,
                    status = auctionItem.status,
                    bidder = auctionItem.bidder,
                    reserve = auctionItem.reserve,
                    start = auctionItem.start,
                    end = auctionItem.end,
                    duration = auctionItem.duration
                });
            }
        }

        public async void PlaceBid(string bid, int id)
        {
            await _hubProxy.Invoke("Bid", UserName, id, bid);
        }
    }
}
