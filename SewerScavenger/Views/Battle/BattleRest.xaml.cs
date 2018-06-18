using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;
using SewerScavenger.Controllers;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BattleRest : ContentPage
    {
        private List<Character> PartyListCopy;  // List of surviving party members
        private List<Item> DropListCopy;        // List of items accumulated from items dropped during the previous battle
        private Score ScoreItem;                // Holds the score from the previous battle
        private int Battles;                    // Holds the curent number of battles so far
        private BattleRestViewModel _viewModel; // Displays a string that is the current number of battles

        // Constructor that requires the survivors list, items dropped list, updated score, and the current battle count.
        public BattleRest(List<Character> pList, List<Item> iList, Score score, int bCount)
        {
            // Initialize everything
            InitializeComponent();
            PartyListCopy = new List<Character>(pList);
            DropListCopy = new List<Item>(iList);
            ScoreItem = score;
            Battles = bCount;

            // Initialize the display
            _viewModel = new BattleRestViewModel("Battle Count: " + bCount);
            BindingContext = _viewModel;
            PartyListView.ItemsSource = PartyListCopy;
            InventoryListView.ItemsSource = DropListCopy;
        }

        // Called when the user request the state of a new battle.
        async void StartGameButtonClick(object sender, EventArgs e)
        {
            // Generates a new battle to continue the game
            BattleSystemViewModel.Instance.ContinueBattle(PartyListCopy, ScoreItem, Battles);
            // Pop because the BattlePage will continue the game OnAppearing
            await Navigation.PopModalAsync();
        }

        // Called when te user selects a surviving character
        public async void CharacterSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Character;
            if (data == null)
            {
                return;
            }

            // This page allows the users to equip their characters
            await Navigation.PushModalAsync(new CharacterEquipPage(new CharacterDetailViewModel(data), DropListCopy));

            // Manually deselect item.
            PartyListView.SelectedItem = null;
        }

        // Called when the user selects an item from the dropped list 
        private async void ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            // Just displays more details about the items 
            await Navigation.PushModalAsync(new InventoryDetailsPage(new ItemDetailViewModel(data)));

            // Manually deselect item.
            InventoryListView.SelectedItem = null;
        }

        // Overriden to ensure continuous poping all the way back to the PickCharacter page
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Continue poping till the PickCharacter page is at the top
            if (BattleSystemViewModel.Instance.GetState() == "To Root")
            {
                Navigation.PopModalAsync();
            }
            
            // If the state is valid, meaing the user just returned from the character equip or item view pages 
            else
            {
                BindingContext = null;
                PartyListView.ItemsSource = null;
                InventoryListView.ItemsSource = null;

                InitializeComponent();

                // Update the display
                _viewModel = new BattleRestViewModel("Battle Count: " + Battles);
                BindingContext = _viewModel;
                PartyListView.ItemsSource = PartyListCopy;
                InventoryListView.ItemsSource = DropListCopy;
            }
        }

        // Catches the back button to mean that the game will now go back to the PickCharacter page
        protected override bool OnBackButtonPressed()
        {
            BattleSystemViewModel.Instance.SetState("To Root");
            Navigation.PopModalAsync();
            return true;
        }
    }
}