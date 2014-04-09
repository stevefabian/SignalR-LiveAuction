using LiveAuctionWinPhone8.Models;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveAuctionWinPhone8
{
    public class ViewModel : INotifyPropertyChanged
    {
        public string UserName { get; set; }

        private HubConnection _hubConnection;
        private IHubProxy _hubProxy;
        public SynchronizationContext Context { get; set; }
        public bool IsHubStarted { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public ObservableCollection<Models.ItemModel> Items { get; set; }

        public ViewModel()
        {
            Messages = new ObservableCollection<string>();
            Items = new ObservableCollection<ItemModel>();
            UserName = "Anonymous";
            IsHubStarted = false;
        }

        public async void StartSignalRHub()
        {
            _hubConnection = new HubConnection(
                "http://192.168.0.30:55720/");
            _hubProxy = _hubConnection.CreateHubProxy("MyHub");
            Context = SynchronizationContext.Current;

            await _hubConnection.Start();
            IsHubStarted = true;
            await _hubProxy.Invoke("Send", new object[]{
                this.UserName, "Has joined the Auction"});

            _hubProxy.On("Reload", () =>
                Context.Post(delegate { LoadItems(); }, null));

            _hubProxy.On<string>("Message", (msg) =>
                Context.Post(delegate { this.Messages.Add(msg); }, null));
        }

        public async Task Login(string username)
        {
            this.UserName = username;
        }

        public async void SendMessage(string msg)
        {
            await _hubProxy.Invoke("Send", this.UserName, msg);
        }

        public async void PlaceBid(string bid, int id)
        {
            await _hubProxy.Invoke("Bid", this.UserName, id, bid);
        }

        public void LoadItems()
        {
            const string apiUrl = @"http://192.168.0.30:55720/api/items/started";

            WebClient webClient = new WebClient();
            webClient.Headers["Accept"] = "application/json";
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadCatalogCompleted);
            webClient.DownloadStringAsync(new Uri(apiUrl));
        }

        private void webClient_DownloadCatalogCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        Items.Clear();
                        if (e.Result != null)
                        {
                            var auctionItems = JsonConvert.DeserializeObject<ItemModel[]>(e.Result);
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
                    }
                    catch (Exception ex)
                    {
                        this.Items.Add(new ItemModel()
                        {
                            id = 0,
                            name = "An Error Occurred",
                            desc = String.Format("The following exception occured: {0}", ex.Message)
                        });
                    }

                    NotifyPropertyChanged(m => m.Items);
                });        
        }

        #region INotifyPropertyChanged Members

        // Allows you to specify a lambda for notify property changed
        public event PropertyChangedEventHandler PropertyChanged;

        // Defined as virtual so you can override if you wish
        protected virtual void NotifyPropertyChanged<TResult>
            (Expression<Func<ViewModel, TResult>> property)
        {
            // Convert expression to a property name
            string propertyName = ((MemberExpression)property.
                Body).Member.Name;

            // Fire notify property changed event
            InternalNotifyPropertyChanged(propertyName);
        }

        protected void InternalNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
