using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SewerScavenger.Models;
using SewerScavenger.Services;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BattlePage : ContentPage
    {
        // Displayed onto the screen, contains all the characters and monsters who are fighting
        private List<List<BattleTile>> theGrid;

        // State of the battle. When onAttack is true, red tiles are displayed and the character may attack
        // When onAttack is false, blue tiles are displayed and the character may move
        // Necessary for the view because tile colors need to be refreshed
        private bool onAttack;

        // Hold when the page is currently loading
        private bool loading = false;

        // Holds the battle text that is displayed at the bottom. Accumulates from the battleSystem
        private List<BattleRecordsItem> records = new List<BattleRecordsItem>();

        // Holds the Business logic for the battle.
        private BattleSystem battleSystem;

        // Hackathon potion display
        private int healingPotionsLeft = 0;

        // Constructor for the battle page
        public BattlePage()
        {
            InitializeComponent();
            onAttack = false;

            // Initialized the potions depending on the debug toggle
            if (BattleSystemViewModel.Instance.HealthPotions)
            {
                HealthPotion.Text = "Use Potion: 6 left";
                healingPotionsLeft = 6;
            }
            else
            {
                HealthPotion.Text = "Use Potion: 0 left";
                healingPotionsLeft = 0;
            }

            // Grabs the current state of the battle
            battleSystem = BattleSystemViewModel.Instance.GetBattleSystem();

            // Wait for the battle computation to end
            while (BattleSystemViewModel.Instance.running)
            {
            }

            // Begin the batte by enabling the AI to move first. 
            ConsolidateAI();
        }

        // Allows the AI to act then prepares the grid for the current state of the game.
        async void ConsolidateAI()
        {
            // Calls for the AI to check if it is their turn and allows them to take it if it is
            // Results are added to the records
            FeedRecords(battleSystem.EnemyTurn());

            // Checks if the game has ended to push the BattleRestPage
            if (battleSystem.IsBattleOver())
            {
                BattleSystemViewModel.Instance.SetState("BattlePage");
                await Navigation.PushModalAsync(new BattleRest(battleSystem.BattleOver(), battleSystem.ItemPool, battleSystem.TotalScore, battleSystem.BattleCount));
            }

            // Checks if the game has ended to push the score page
            else if (battleSystem.IsGameOver())
            {
                Score score = battleSystem.GameOver();
                await SQLDataStore.Instance.AddAsync_Score(score);
                ScoresViewModel.Instance.Dataset.Add(score);
                ScoresViewModel.Instance.SetNeedsRefresh(true);
                await Navigation.PushModalAsync(new ScoreResults(new ScoreDetailViewModel(score)));
            }

            // Updates the display based on the results of the AI attack. 
            else
            {
                LoadTheGrid();
            }
        }

        // Called by all 6 of the ListView items when clicked. 
        // Basically clicking in the grid is passed here. Includes the tile that was clicked
        async void GridListTapped(BattleTile tile)
        {
            // If the state of the game is attack phase
            if (onAttack)
            {
                // Checks if the tile is a valid attack target 
                if ((battleSystem.TurnList[0].Row == tile.Row && battleSystem.TurnList[0].Column == tile.Column) || tile.Highlight == "Red")
                {
                    // Compute the damage of the attack, then feed the results to the records
                    FeedRecords(battleSystem.CharacterAttacks(tile));

                    // If the battle has ended as a result of the character attacking
                    if (battleSystem.IsBattleOver())
                    {
                        // Push a new BattleRest page so they can equip stuff
                        loading = false;
                        BattleSystemViewModel.Instance.SetState("BattlePage");
                        await Navigation.PushModalAsync(new BattleRest(battleSystem.BattleOver(), battleSystem.ItemPool, battleSystem.TotalScore, battleSystem.BattleCount));
                    }

                    // If the game has ended(since AI can attack after the character attacks)
                    else if (battleSystem.IsGameOver())
                    {
                        // Push a new ScorePage to display the results of the game
                        Score score = battleSystem.GameOver();
                        await SQLDataStore.Instance.AddAsync_Score(score);
                        ScoresViewModel.Instance.Dataset.Add(score);
                        ScoresViewModel.Instance.SetNeedsRefresh(true);
                        loading = false;
                        await Navigation.PushModalAsync(new ScoreResults(new ScoreDetailViewModel(score)));
                    }

                    // Otherwise transition from attack mode to move mode then refresh the grid.
                    else
                    {
                        LoadTheGrid();
                        onAttack = false;
                    }
                }

                // If the selected tile is not an attack target, display information about the tile to the user.
                else
                {
                    await DisplayAlert(tile.GetTitleInfo(), tile.GetAllInfo(), "Okay");
                }
            }

            // If the state of the game is in move phase
            else
            {
                // Checks if the tile is a valid move location
                if ((battleSystem.TurnList[0].Row == tile.Row && battleSystem.TurnList[0].Column == tile.Column) || tile.Highlight == "Blue")
                {
                    // Executes the move, change from move to attack phase, then update the grid.
                    battleSystem.MoveCharacter(tile);
                    onAttack = true;
                    LoadTheGrid();
                }

                // If it is not possible to move here, then the information about the tile is displayed instead
                else
                {
                    await DisplayAlert(tile.GetTitleInfo(), tile.GetAllInfo(), "Okay");
                }
            }
            loading = false;
        }

        // Page refresh to update the turnList and the entire grid. All 6 columns.
        public void LoadTheGrid()
        {
            if (BattleSystemViewModel.Instance.UseFocusedAttack)
            {
                FocusedAttack.Text = "Focused Attack";
            }
            else
            {
                FocusedAttack.Text = "Normal Attack";
            }
            TurnListView.ItemsSource = null;
            GridList1.ItemsSource = null;
            GridList2.ItemsSource = null;
            GridList3.ItemsSource = null;
            GridList4.ItemsSource = null;
            GridList5.ItemsSource = null;
            TurnListView.ItemsSource = battleSystem.TurnList;
            theGrid = battleSystem.GetGrid();
            GridList1.ItemsSource = theGrid[0];
            GridList2.ItemsSource = theGrid[1];
            GridList3.ItemsSource = theGrid[2];
            GridList4.ItemsSource = theGrid[3];
            GridList5.ItemsSource = theGrid[4];
        }

        // If the turn list was clicked, information regarding the monster or character is displayed
        async void TurnListTapped(object sender, ItemTappedEventArgs e)
        {
            var data = e.Item as BattleTile;
            if (data == null)
                return;

            TurnListView.SelectedItem = null;
            await DisplayAlert(data.GetTitleInfo(), data.GetAllInfo(), "Okay");
        }
        
        // Handler for a column on the grid.
        public void GridListTapped1(object sender, ItemTappedEventArgs e)
        {
            while (loading)
            {
            }
            loading = true;
            var data = e.Item as BattleTile;
            if (data == null)
                return;
            GridList1.SelectedItem = null;
            GridListTapped(data);
        }

        // Handler for a column on the grid.
        public void GridListTapped2(object sender, ItemTappedEventArgs e)
        {
            while (loading)
            {
            }
            loading = true;
            var data = e.Item as BattleTile;
            if (data == null)
                return;
            GridList2.SelectedItem = null;
            GridListTapped(data);
        }

        // Handler for a column on the grid.
        public void GridListTapped3(object sender, ItemTappedEventArgs e)
        {
            while (loading)
            {
            }
            loading = true;
            var data = e.Item as BattleTile;
            if (data == null)
                return;
            GridList3.SelectedItem = null;
            GridListTapped(data);
        }

        // Handler for a column on the grid.
        public void GridListTapped4(object sender, ItemTappedEventArgs e)
        {
            while (loading)
            {
            }
            loading = true;
            var data = e.Item as BattleTile;
            if (data == null)
                return;
            GridList4.SelectedItem = null;
            GridListTapped(data);
        }

        // Handler for a column on the grid.
        public void GridListTapped5(object sender, ItemTappedEventArgs e)
        {
            while (loading)
            {
            }
            loading = true;
            var data = e.Item as BattleTile;
            if (data == null)
                return;
            GridList5.SelectedItem = null;
            GridListTapped(data);
        }

        // Heal character button clicked for the hackathon,
        // Always visible but not necessarily usable
        public async void HealCharacter_Command(object sender, EventArgs e)
        {
            // Holds while the battle system is loading
            while (loading)
            {
            }
            loading = true;

            // Ensures that the phase is currently for attack and not move
            if (onAttack)
            {
                //Call the heal command in OnAttack
                if (healingPotionsLeft > 0)
                {
                    //Decrease the value of the number of potions left
                    healingPotionsLeft--;
                    HealthPotion.Text = "Use Potion :" + healingPotionsLeft + " left";

                    // Heal the hero
                    Character hero = battleSystem.TheGrid.Grid[battleSystem.TurnList[0].Column][battleSystem.TurnList[0].Row].Hero;
                    hero.HealthCurr = hero.Health;
                    battleSystem.TurnList[0].Life = "life" + (int)(((double)(battleSystem.TurnList[0].Hero.HealthCurr + 1) / (double)(battleSystem.TurnList[0].Hero.Health + 1)) * 10) + ".png";

                    // Spend hero's turn
                    battleSystem.TurnList.RemoveAt(0);
                    battleSystem.RemovePathing();
                    onAttack = false;

                    // Update the page
                    FeedRecords(hero.Name + " uses a health potion and is now fully healed.\n" + battleSystem.EnemyTurn());
                    LoadTheGrid();
                }
                else
                {
                    // No potions left
                    await DisplayAlert("Health Potions", "You do not have any health potions left to use", "Ok");
                }
            }
            else
            {
                // Not in attack phase
                await DisplayAlert("Health Potions", "Health Potions usable during Attack Phase", "Ok");
            }
            loading = false;
        }

        // Focus attack button clicked
        private async void FocusedAttackClicked(object sender, EventArgs e)
        {
            // If focus battle is allowed for this game
            if (BattleSystemViewModel.Instance.FocusedAttack)
            {
                // If the character is currently using focused attack
                if (BattleSystemViewModel.Instance.UseFocusedAttack)
                {
                    // Set text to say normal mode is currently being used
                    FocusedAttack.Text = "Normal Attack";
                    BattleSystemViewModel.Instance.SetUseFocusedAttack(false);
                    await DisplayAlert("Focused Attack", "Normal Attack Activated!", "Ok");
                }

                // If the character is currently using normal attack
                else
                {
                    // Check if the character can still use a focused attack this turn
                    if (battleSystem.TurnList[0].FocusedAttack)
                    {
                        // Change the button to a Focused attack
                        FocusedAttack.Text = "Focused Attack";
                        BattleSystemViewModel.Instance.SetUseFocusedAttack(true);
                        await DisplayAlert("Focused Attack", "Focused Attack Activated!", "Ok");
                    }

                    // Notify user that the focused attack cannot be used 
                    else
                    {
                        await DisplayAlert("Focused Attack", "Character is incappable of a Focused Attack.", "Ok");
                    }
                }
            }

            // Notify user that the focused attack cannot be used
            else
            {
                await DisplayAlert("Focused Attack", "Character is incappable of a Focused Attack.", "Ok");
            }
        }

        // If he user clicks on any of the text on the battle text being displayed at the bottom of the BattlePage
        // The text is displayed in full as a DisplayAlter
        async void ReadTextRequested(object sender, ItemTappedEventArgs e)
        {
            var data = e.Item as BattleRecordsItem;
            if (data == null)
                return;
            BattleRecord.SelectedItem = null;
            await DisplayAlert("Battle Text", data.Records, "Okay");
        }

        // Feeds new battle records to the display on the bottom.
        // Ensures the list only contains a maximum of 100 lines so as not to slow down the game 
        public void FeedRecords(string record)
        {
            string tempRec = "";

            // Add the text separated by a new line symbol
            for (var i = 0; i < record.Count(); i++)
            {
                // Add the line to the records
                if (record[i] == '\n')
                {
                    records.Insert(0, new BattleRecordsItem(tempRec));
                    tempRec = "";
                }

                // Collect text for the new line
                else
                {
                    tempRec += record[i];
                }
            }

            // Ensures that the records do not contain too much informations so as not to slow down the game
            while(records.Count > 100)
            {
                records.RemoveAt(records.Count-1);
            }

            // Updates the display
            BattleRecord.ItemsSource = null;
            BattleRecord.ItemsSource = new List<BattleRecordsItem>(records);
        }

        // Used to ensure that return from the Battle Rest page with the correct state will continue the game.
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // If the state of the game is invalid. This page is popped
            if(BattleSystemViewModel.Instance.GetState() != "BattlePage")
            {
                BattleSystemViewModel.Instance.SetState("To Root");
                Navigation.PopModalAsync();

            // If the state of the game s valid. This page creates a new battle then continues
            }else
            {
                // New battle initialization
                InitializeComponent();
                onAttack = false;
                if (BattleSystemViewModel.Instance.HealthPotions)
                {
                    HealthPotion.Text = "Use Potion: 6 left";
                    healingPotionsLeft = 6;
                }
                else
                {
                    HealthPotion.Text = "Use Potion: 0 left";
                    healingPotionsLeft = 0;
                }

                // Grabs the latest copy
                battleSystem = BattleSystemViewModel.Instance.GetBattleSystem();

                // Waits for it to finish setting up
                while (BattleSystemViewModel.Instance.running)
                {

                }

                // Gives AI the turn
                ConsolidateAI();
                loading = false;
            }

            // If the user uses the back button at any time, the game will stop and be brought back to the pick character page
            BattleSystemViewModel.Instance.SetState("To Root");
        }
    }
}