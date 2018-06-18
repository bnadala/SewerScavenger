using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;
using SewerScavenger.Services;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickCharacter : ContentPage
    {
        // Contains the list of playable characters
        private CharactersViewModel _viewModel;

        // Contains the list of possible monsters
        private MonstersViewModel _monsterViewModel;

        // List of characters the user will play with
        private List<Character> partyList = new List<Character>();

        // Constructor loads all the characters for display
        public PickCharacter()
        {
            InitializeComponent();
            partyList = new List<Character>();
            PartyList.ItemsSource = partyList;
            BindingContext = _viewModel = CharactersViewModel.Instance;

            // Grabs a copy of the monsters
            _monsterViewModel = MonstersViewModel.Instance;
            if (_monsterViewModel.Dataset.Count == 0)
            {
                _monsterViewModel.LoadDataCommand.Execute(null);
            }
            else if (_monsterViewModel.NeedsRefresh())
            {
                _monsterViewModel.LoadDataCommand.Execute(null);
            }
        }

        // If the user selects a character from the chracter list then it is added to the party
        async void AddCharacterToParty(object sender, ItemTappedEventArgs e)
        {
            var data = e.Item as Character;
            if (data == null)
                return;

            // Check if partyList is full
            if (partyList.Count < 6)
            {
                // Reset Contents of ItemsSource
                PartyList.ItemsSource = null;

                // Append item to the list if party is not full
                partyList.Add(data);
                PartyList.ItemsSource = partyList;
            }
            else
            {
                // Warn player party is full
                await DisplayAlert("Party Full", "Cannot have more than six party members", "OK");
            }

            //Deselect Item
            MyListView.SelectedItem = null;
        }

        // If the user selects a character from the party list then it is removed
        public void RemoveCharacterFromParty(object sender, ItemTappedEventArgs e)
        {
            var data = e.Item as Character;
            if (data == null)
                return;

            // Reset Contents of ItemsSource
            PartyList.ItemsSource = null;

            // Remove selected character from partyList
            partyList.Remove(data);

            // Update contents of ItemsSource
            PartyList.ItemsSource = partyList;

            //Deselect Item
            PartyList.SelectedItem = null;
        }

        // If the player starts a normal game
        async void StartGameButtonClick(object sender, EventArgs e)
        {
            // If no characters in the party list, then fill it from the character list
            if (partyList.Count == 0)
            {
                // If no playable characters tell the user to go make some
                if(_viewModel.Dataset.Count == 0)
                {
                    // If Character List is empty warn player
                    await DisplayAlert("No Characters", "Cannot have a party with no characters", "OK");
                    return;
                }
                else
                {
                    // Fills the party list with characters from the character list till full at 6
                    partyList = new List<Character>();
                    foreach (Character character in _viewModel.Dataset)
                    {
                        if(partyList.Count < 6)
                        {
                            partyList.Add(new Character(character));
                        }
                    }
                }
            }

            // Pass in a new list of monsters
            List<Monster> monsterList = new List<Monster>();
            foreach (Monster m in _monsterViewModel.Dataset)
            {
                monsterList.Add(new Monster(m));
            }

            // If no monsters available for the battle then tell the user to go make more
            if (monsterList.Count == 0)
            {
                await DisplayAlert("No Enemies to Fight", "Cannot have a battle with no enemies.", "OK");
                return;
            }

            // Prepare for battle
            BattleSystemViewModel.Instance.SetState("BattlePage");
            BattleSystemViewModel.Instance.NewBattle(partyList, monsterList);
                
            // Load new Battle Page with the selected party members
            await Navigation.PushModalAsync(new BattlePage());
        }

        // If the player starts a normal game
        async void AutoPlayButtonClick(object sender, EventArgs e)
        {
            // If no characters in the party list then it is filled with characters from the character list
            if (partyList.Count == 0)
            {
                // If no characters in the character list then tell the user to go make some
                if (_viewModel.Dataset.Count == 0)
                {
                    // If Character List is empty warn player
                    await DisplayAlert("No Characters", "Cannot have a party with no characters", "OK");
                    return;
                }
                else
                {
                    // Party list is fill with characters from the character list up to max. 6
                    partyList = new List<Character>();
                    foreach (Character character in _viewModel.Dataset)
                    {
                        if (partyList.Count < 6)
                        {
                            partyList.Add(new Character(character));
                        }
                    }
                }
            }

            // Pass in new list of monsters
            List<Monster> monsterList = new List<Monster>();
            foreach (Monster m in _monsterViewModel.Dataset)
            {
                monsterList.Add(new Monster(m));
            }

            // If no monsters then tell the user to go make some
            if (monsterList.Count == 0)
            {
                await DisplayAlert("No Enemies to Fight", "Cannot have a battle with no enemies.", "OK");
                return;
            }

            // Create new instance of each character for the game
            List<Character> pList = new List<Character>();
            foreach (Character c in partyList)
            {
                pList.Add(new Character(c));
            }
            
            // Start a new autobattle and wait for results
            // Await because the items are grabbed from the post call 
            await BattleSystemViewModel.Instance.NewAutoBattle(pList, monsterList);
            
            // Score is retrieved from the system after the battle
            Score scores = BattleSystemViewModel.Instance.GetScore();

            // Add to the score list
            await SQLDataStore.Instance.AddAsync_Score(scores);
            ScoresViewModel.Instance.Dataset.Add(scores);
            ScoresViewModel.Instance.SetNeedsRefresh(true);

            // Load new Battle Page with the selected party members
            await Navigation.PushModalAsync(new ScoreResults(new ScoreDetailViewModel(scores)));
        }

        // When this page appears refresh the displays
        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = null;

            if (ToolbarItems.Count > 0)
            {
                ToolbarItems.RemoveAt(0);
            }

            InitializeComponent();

            if (_viewModel.Dataset.Count == 0)
            {
                _viewModel.LoadDataCommand.Execute(null);
            }
            else if (_viewModel.NeedsRefresh())
            {
                _viewModel.LoadDataCommand.Execute(null);
            }

            BindingContext = _viewModel;

            PartyList.ItemsSource = null;
            PartyList.ItemsSource = partyList;
        }
    }
}