using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiveAuctionWinPhone8
{
    public partial class AuctionItem : PhoneApplicationPage
    {
        private static int _id;

        public AuctionItem()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";

            if (NavigationContext.QueryString.TryGetValue("id", out selectedIndex))
            {
                _id = int.Parse(selectedIndex);
                LoadItem();
            }
        }

        private void LoadItem()
        {
            var item = App.MainViewModel.Items.Where(x => x.id == _id).SingleOrDefault();
            if (item != null)
            {
                DataContext = item;
            }
        }

        private void PlaceBid_Click(object sender, RoutedEventArgs e)
        {
            App.MainViewModel.PlaceBid(txtBid.Text, _id);
            NavigationService.Navigate(new Uri("/AuctionPage.xaml", UriKind.Relative));
        }
    }
}