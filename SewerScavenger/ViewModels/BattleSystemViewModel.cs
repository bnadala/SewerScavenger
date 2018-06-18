using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SewerScavenger.Models;
using SewerScavenger.Controllers;

namespace SewerScavenger.ViewModels
{
    public class BattleSystemViewModel : BaseViewModel
    {
        // Ensures one instance of the BattleSystemViewModel is passed along the different battle pages
        private static BattleSystemViewModel _instance;
        public static BattleSystemViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BattleSystemViewModel();
                }
                return _instance;
            }
        }
        
        // Hackathon settings
        public bool DisableRNG = false;
        public int ToHit = 0;
        public bool Miss1 = true;
        public bool Hit20 = true;
        public bool CriticalMiss1 = false;
        public bool Critical20 = false;
        public bool MassVolcano = false;
        public bool HealthPotions = false;
        public bool FocusedAttack = false;
        public bool UseFocusedAttack = false;
        public bool MostlyDead = false; 
        public bool Auto = false;

        // Game related resources
        private BattleSystem battleSystem;                              // Holds the main battle system
        private List<Item> DropList = new List<Item>();                 // List of drops to feed into the next battle
        private List<Monster> MonsterList = new List<Monster>();        // Monster list to be used to generate new enemies
        private List<Character> CharacterList = new List<Character>();  // List of characters to be used for the battles
        public Score SaveScore = new Score();                           // Score accumulated throughout the battles
        private int BattleCount = 0;                                    // Number of battles
        private string State = "";                                      // Used to pop and push into the right page
        public bool running = false;                                    // Used to hold asynchronous threads. 

        // Basic constructor. The functions are the important points. 
        public BattleSystemViewModel()
        {
        }

        // Creates a new game with the characters and monsters from the PickCharacter page
        public void NewBattle(List<Character> cList, List<Monster> mList)
        {
            running = true;

            MonsterList = mList;
            SaveScore = new Score();
            battleSystem = new BattleSystem(cList, mList, SaveScore);
            State = "BattlePage";

            running = false;
        }

        // Continues a previous battle from the BattleRest page
        public void ContinueBattle(List<Character> cList, Score score, int bCount)
        {
            running = true;
            
            BattleCount = bCount;
            SaveScore = score;
            battleSystem = new BattleSystem(cList, MonsterList, SaveScore, bCount);
            State = "BattlePage";

            running = false; 
        }

        // Begins new auto battle. 
        public async Task<int> NewAutoBattle(List<Character> cList, List<Monster> mList)
        {
            running = true;
            
            MonsterList = mList;
            SaveScore = new Score();
            battleSystem = new BattleSystem(cList, mList, true, SaveScore);
            await battleSystem.AutoRun();
            State = "Score Page";

            running = false;
            return 0;
        }

        // Deprecated but still kept just in case.
        // This function has been moved to inside of the BattleSystem to allow for automatic refresh during autobattle.
        public async void GrabFreshDrops()
        {
            var number = 6; // Want to get 10 items from the server or however many items you want from the server
            var level = 1;
            var attribute = AttributeEnum.Unknown;
            var location = ItemLocationEnum.Unknown;
            var random = true;
            var updateDataBase = false;

            var drop = await ItemsController.Instance.GetItemsFromServerPost(number, level, attribute, location, random, updateDataBase);

            for (int i = 0; i < 6; i++)
            {
                Random rng = new Random();
                DropList.Add(ItemsViewModel.Instance.Dataset[rng.Next(ItemsViewModel.Instance.Dataset.Count)]);
            }
        }

        // Returns the battle system 
        public BattleSystem GetBattleSystem()
        {
            return battleSystem;
        }

        // Returns the turnlist from the battle system
        public List<BattleTile> GetTurnList()
        {
            return battleSystem.TurnList;
        }

        // Returns the current score on the battle system
        public Score GetScore()
        {
            return battleSystem.GameOver();
        }

        // Returns the current state of the game
        public string GetState()
        {
            return State;
        }

        // Sets the current state of the game
        public void SetState(string s)
        {
            State = s;
        }

        // Settings Setters when a toggle is pressed on the debug 
        public void SetDisableRNG(bool settings)
        {
            DisableRNG = settings;
        }

        public void SetMiss1(bool settings)
        {
            Miss1 = settings;
        }

        public void SetHit20(bool settings)
        {
            Hit20 = settings;
        }

        public void SetCritical20(bool settings)
        {
            Critical20 = settings;
        }

        public void SetToHit(int settings)
        {
            ToHit = settings;
        }

        public void SetCriticalMiss1(bool settings)
        {
            CriticalMiss1 = settings;
        }

        public void SetMassVolcano(bool settings)
        {
            MassVolcano = settings;
        }

        public void SetHealthPotions(bool settings)
        {
            HealthPotions = settings;
        }

        public void SetFocusedAttack(bool settings)
        {
            FocusedAttack = settings;
        }

        public void SetUseFocusedAttack(bool settings)
        {
            UseFocusedAttack = settings;
        }

        // Function to add text to the battle text. Only adds text when the debug option is turned on
        // Otherwise the auto battle and normal battles will take too long accumulate and display all the text.
        // Purely for debug purposes. 
        public void AddTxt(StringWrapper s)
        {
            if (Auto)
            {
                SaveScore.BattleText.Add(s);
            }
        }
    }
}