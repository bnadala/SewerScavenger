using System;
using SewerScavenger.Controllers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SewerScavenger.ViewModels;
using SewerScavenger.Models;

namespace SewerScavenger.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DebugControls : ContentPage
	{
        // About page with all the hackathon debug options
		public DebugControls ()
		{
            // Controllers setup
            InitializeComponent();
            UseMockDataSource.IsToggled = false;
            SetDataSource(UseMockDataSource.IsToggled);
            BattleSystemViewModel.Instance.SetDisableRNG(DisableRNG.IsToggled);
            BattleSystemViewModel.Instance.SetMiss1(Miss1.IsToggled);
            BattleSystemViewModel.Instance.SetHit20(Hit20.IsToggled);
            BattleSystemViewModel.Instance.SetToHit(Int32.Parse(ToHit.Text));
            BattleSystemViewModel.Instance.SetCriticalMiss1(CriticalMiss1.IsToggled);
            BattleSystemViewModel.Instance.SetCritical20(Critical20.IsToggled);
            BattleSystemViewModel.Instance.SetMassVolcano(MassVolcano.IsToggled);
            BattleSystemViewModel.Instance.SetHealthPotions(HealthPotions.IsToggled);
            BattleSystemViewModel.Instance.SetFocusedAttack(FocusedAttack.IsToggled);

            // Example of how to add an view to an existing set of XAML. 
            // Give the Xaml layout you want to add the data to a good x:Name, so you can access it.  Here "DateRoot" is what I am using.
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

        // Toggle mock data store
        private void UseMockDataSourceSwitch_OnToggled(object sender, ToggledEventArgs e)
        {
            // This will change out the DataStore to be the Mock Store if toggled on, or the SQL if off.
            SetDataSource(e.Value);
        }

        // Get 10 random items from the web
        private async void GetItemsPost_Command(object sender, EventArgs e)
        {
            var number = 10; // Want to get 10 items from the server or however many items you want from the server
            var level = 0;
            var attribute = AttributeEnum.Unknown;
            var location = ItemLocationEnum.Unknown;
            var random = true;
            var updateDataBase = true;

            var answer = await DisplayAlert("Post", "Sure you want to Post Items from the Server?", "Yes", "No");
            if (answer)
            {
                var myDataList = await ItemsController.Instance.GetItemsFromServerPost(number, level, attribute, location, random, updateDataBase);

                var myOutput = string.Empty;
                foreach (var item in myDataList)
                {
                    // Build up the output list by appending.
                    // use "\n"; to add a line seperator at the end of each item
                    myOutput += item.FormatOutput() + "\n";
                }
                await DisplayAlert("Returned List", myOutput, "Okay");
            }
        }

        // Disables RNG on the game to modify attack
        private void DisableRNG_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.SetDisableRNG(e.Value);
        }

        // Toggle to allow missing
        private void Miss1_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.SetMiss1(e.Value);
            if (e.Value)
            {
                CriticalMiss1.IsToggled = false;
            }
        }

        // Toggle to allow guaranteed hit
        private void Hit20_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.SetHit20(e.Value);
            if (e.Value)
            {
                Critical20.IsToggled = false;
            }
        }

        // Toggle to allow Critical at 20 instead of hit
        private void Critical20_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.SetCritical20(e.Value);
            if (e.Value)
            {
                Hit20.IsToggled = false;
            }
        }

        // Set the "Random" roll
        private void ToHitClicked(object sender, EventArgs e)
        {
            if (Int32.TryParse(ToHit.Text, out var i)){
                BattleSystemViewModel.Instance.SetToHit(i);
            }
        }

        // Toggle to allow critical miss instead of normal miss at roll 1
        private void CriticalMiss1_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.SetCriticalMiss1(e.Value);
            if (e.Value)
            {
                Miss1.IsToggled = false;
            }
        }

        // Toggle to allow the mass volcano event
        private void MassVolcano_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.SetMassVolcano(e.Value);
        }

        // Toggle to allow health potions
        private void HealthPotions_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.SetHealthPotions(e.Value);
        }

        // Toggle to allow focused attack
        private void FocusedAttack_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.SetFocusedAttack(e.Value);
        }

        // Toggle to allow character resurrection at first death per battle
        private void MostlyDead_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.MostlyDead = e.Value;
        }

        // Toggle Debug auto battle text
        private void Auto_Toggled(object sender, ToggledEventArgs e)
        {
            BattleSystemViewModel.Instance.Auto = e.Value;
        }

        // Button to close the debug panel or pop current page back to actual about page. hehehe
        async void DebugPanelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}