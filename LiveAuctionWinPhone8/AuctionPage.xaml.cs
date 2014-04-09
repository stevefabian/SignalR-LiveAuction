using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiveAuctionWinPhone8.Models;

namespace LiveAuctionWinPhone8
{
    public partial class AuctionPage : PhoneApplicationPage
    {
        public AuctionPage()
        {
            InitializeComponent();

            if (!App.MainViewModel.IsHubStarted)
            {
                App.MainViewModel.StartSignalRHub();
                App.MainViewModel.LoadItems();
            }

            DataContext = App.MainViewModel;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            App.MainViewModel.SendMessage(TextBoxMessage.Text);
            TextBoxMessage.Text = "";
        }

        private void AuctionItem_Selected(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (lbItems.SelectedIndex == -1)
                return;

            // get the item they touched
            var item = (ItemModel)lbItems.Items[lbItems.SelectedIndex];

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/AuctionItem.xaml?id=" + item.id, UriKind.Relative));
        }
    }
}