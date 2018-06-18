
using System;
using SewerScavenger.Services;
using SewerScavenger.Controllers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        // Displays a short story ABOUT the game then allows for editting options. 
        public AboutPage()
        {
            InitializeComponent();
            var dateLabel = new Label
            {
                Text = DateTime.Now.ToShortDateString(),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontFamily = "Helvetica Neue",
                FontAttributes = FontAttributes.Bold,
                FontSize = 12,
                TextColor = Color.White,
            };
            DateRoot.Children.Add(dateLabel);
            UseMockDataSource.IsToggled = false;
            SetDataSource(UseMockDataSource.IsToggled);
        }

        // Opens the debug panel by moving to a new page with the debug panel. hehehe
        async void DebugPanelClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DebugControls());
        }

        // Clear command for the database
        private async void ClearDatabase_Command(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete", "Sure you want to Delete All Data, and start over?", "Yes", "No");
            if (answer)
            {
                // Call to the SQL DataStore and have it clear the tables.
                SQLDataStore.Instance.InitializeDatabaseNewTables();
            }
        }
        
        // Get items from the server. All hundred command
        private async void GetItems_Command(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Get", "Sure you want to Get Items from the Server?", "Yes", "No");
            if (answer)
            {
                // Call to the Item Service and have it Get the Items
                ItemsController.Instance.GetItemsFromServer();
            }
        }

        // Update all the things!
        private void SetDataSource(bool isMock)
        {
            if (isMock == true)
            {
                ItemsViewModel.Instance.SetDataStore(BaseViewModel.DataStoreEnum.Mock);
                MonstersViewModel.Instance.SetDataStore(BaseViewModel.DataStoreEnum.Mock);
                CharactersViewModel.Instance.SetDataStore(BaseViewModel.DataStoreEnum.Mock);
                ScoresViewModel.Instance.SetDataStore(BaseViewModel.DataStoreEnum.Mock);
            }
            else
            {
                ItemsViewModel.Instance.SetDataStore(BaseViewModel.DataStoreEnum.Sql);
                MonstersViewModel.Instance.SetDataStore(BaseViewModel.DataStoreEnum.Sql);
                CharactersViewModel.Instance.SetDataStore(BaseViewModel.DataStoreEnum.Sql);
                ScoresViewModel.Instance.SetDataStore(BaseViewModel.DataStoreEnum.Sql);
            }

            // Have data refresh...
            ItemsViewModel.Instance.SetNeedsRefresh(true);
            MonstersViewModel.Instance.SetNeedsRefresh(true);
            CharactersViewModel.Instance.SetNeedsRefresh(true);
            ScoresViewModel.Instance.SetNeedsRefresh(true);
        }

        private void UseMockDataSourceSwitch_OnToggled(object sender, ToggledEventArgs e)
        {
            // This will change out the DataStore to be the Mock Store if toggled on, or the SQL if off.
            SetDataSource(e.Value);
        }
    }
}