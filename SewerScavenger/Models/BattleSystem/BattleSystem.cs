using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SewerScavenger.Controllers;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Models
{
    public class BattleSystem
    {
        // Some in game constants. Should not be changed due to lack of support.
        public int EnemyCount = 6;          // Number of enemies
        public int Rows = 6;                // Number of rows on the grid
        public int Columns = 5;             // Number of columns on the grid

        // Lists of things
        public List<Monster> MonsterList;                   // List of Monsters. Does not change.
        public List<Character> PartyList;                   // List of Characters. Changes when they die.
        public List<Monster> EnemyList;                     // List of Monsters. Changes when they die.
        public List<Item> DropList;                         // List of items as rewards. Dropped by monsters but is not tied to a specific monster. Periodically refreshes.
        public int itemsDropped = 0;                        // Counter that increments when an item has been dropped
        public List<BattleTile> TurnList;                   // List of Monsters and Characters in tiles on the grid
        public List<Item> ItemPool = new List<Item>();      // List of items dropped and available for equipping after the battle.

        // Grid, Score, and number of battles
        public BattleGrid TheGrid;                          // The grid contains pathing functions and the actual grid itself.
        public int BattleCount;                             // The current number of battles. Starts at 1
        public Score TotalScore;                            // The accumulated score. Only one score till game ends. Updated per battle.

        // Hackathon options
        public bool MassVolcano = false;                    // Volcano event toggle
        public bool HealthPotions = false;                  // Health potions toggle
        public string OpeningText = "";                     // First text displayed per battle. Used by Volcano Event.

        // Save settings from BattleSystemViewModel for Hackathon options. Called by Normal Battle Constructor
        private void Settings()
        {
            MassVolcano = BattleSystemViewModel.Instance.MassVolcano;
            HealthPotions = BattleSystemViewModel.Instance.HealthPotions;
        }

        // Constructor for a new Normal game and subsequent battles.
        public BattleSystem(List<Character> pList, List<Monster> mList, Score score, int bCount = 0)
        {
            PostNewItems();                             // Updates the DropList Asynchronously with new items from the Web. Will break when no internet available.
            Settings();                                 // Update toggle for Hackathon options
            
            MonsterList = new List<Monster>(mList);     // Create new monster list from the CRUDI monster list.
            PartyList = new List<Character>(pList);     // Create new party list from survivors

            BattleCount = bCount + 1;                   // Update battle count
            TotalScore = score;                         // Update the current score

            // Creates a new score if it is the first battle. 
            if (bCount == 0)
            {
                TotalScore.Id = Guid.NewGuid().ToString();
            }

            EnemyList = GenerateEnemies();              // Generate enemies and level them to the current batte count. 
            TheGrid = PositionCharacters();             // Place both Characters and monsters on the grid.
            TurnList = TheGrid.ComputeTurnList();       // Generate the first turn list.
            TotalScore.TurnNumber += 1;                 // Update the number of turns
        }

        // Constructor for Auto battle mode
        public BattleSystem(List<Character> pList, List<Monster> mList, bool isAuto, Score score)
        {
            Settings();                                 // Update toggle for Hackathon options

            MonsterList = new List<Monster>(mList);     // Create new monster list from the CRUDI monster list.
            PartyList = new List<Character>(pList);     // Create new party list from survivors

            // Initialize the battle count and the score for auto mode.
            BattleCount = 1;
            TotalScore = score;
            TotalScore.Id = Guid.NewGuid().ToString();
            TotalScore.AutoBattle = true;

            // Initialize the grid to reuse most of the computing functions. Does not actually use the grid for auto battle.
            TurnList = new List<BattleTile>();
            EnemyList = GenerateEnemies();
            TheGrid = PositionCharacters();
        }

        // Immediately run to consolidate auto run. 
        // Handles the whole auto run sequence. 
        public async Task<int> AutoRun()
        {
            // Initialize battle state
            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Auto Battle Begins!"));
            await PostNewItems();                           // Update DropList with Web items
            List<BattleTile> HeroList = CollectHeroes();    // Contains all the Characters till they all die.
            List<Item> Inventory = new List<Item>();        // Contains all the items dropped in between battles.
            List<BattleTile> Enemies = CollectEnemies();    // Contains a refreshable list of monsters. 
            TurnList = TheGrid.ComputeTurnList();           // Generate turnlist based on the characters on theGrid.

            // Potions for Hackathon. Updated when new battle begins
            int Potions = 0;

            // Continues to run until all heroes are dead.
            while (HeroList.Count > 0)
            {
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Battle #"+BattleCount));
                // If all enemies have been defeated, generate new battle.
                if (Enemies.Count <= 0)
                {
                    // Generate copy of heroes just in case.
                    PartyList = new List<Character>();
                    foreach (BattleTile bt in HeroList)
                    {
                        PartyList.Add(bt.Hero);
                    }

                    // Generate enemies, heroes, on and off the grid.
                    EnemyList = GenerateEnemies();
                    TheGrid = PositionCharacters();
                    HeroList = CollectHeroes();
                    Enemies = CollectEnemies();
                }

                // If Hackathon option for potions is turned on it refreshes the number of potions in between battles. 
                if (HealthPotions)
                {
                    Potions = 6;
                }

                // Continues till ethier all enemies die and a new battle begins or till all Characters have died and the game ends. 
                while (Enemies.Count > 0 && HeroList.Count > 0)
                {
                    // TurnList is either empty or its a new battle.
                    TurnList = TheGrid.ComputeTurnList();
                    TotalScore.TurnNumber += 1;
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Turn #" + TotalScore.TurnNumber));

                    // Continue till all Fighters have had a turn or when all Characters and Monsters have died. 
                    while (TurnList.Count > 0 && Enemies.Count > 0 && HeroList.Count > 0)
                    {

                        // Random to determine current Fighter's opponent. 
                        Random rng = new Random();

                        // If it is a monster's turn
                        if (TurnList[0].Type == "Monster")
                        {
                            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Monster "+TurnList[0].Enemy.Name + "'s Turn."));
                            // Determine opponent
                            int t = rng.Next(HeroList.Count);

                            // Attach opponent
                            int damage = HeroList[t].DealtDamage(TurnList[0]);
                            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Monster " + TurnList[0].Enemy.Name + " deals "+damage+" damage to "+HeroList[t].Name));

                            // If result is a critical miss
                            if (HeroList[t].DamageStatus == "Critical Miss")
                            {
                                // Since monster missed, automatically drop random item and place in the inventory. Then refresh the DropList.
                                TotalScore.ItemList.Add(new Item(DropList[itemsDropped]));
                                Inventory.Add(DropList[itemsDropped]);
                                await PostNewItems();
                            }
                            HeroList[t].DamageStatus = "";

                            // If the Character dies
                            if (HeroList[t].GetHealthCurr() <= 0)
                            {
                                // Resurrect if the Hackathon toggle is on and if the hero has not resurrected yet this battle.
                                if (BattleSystemViewModel.Instance.MostlyDead && HeroList[t].MostlyDead)
                                {
                                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper(HeroList[t].Name + " comes back to life"));
                                    HeroList[t].MostlyDead = false;
                                    HeroList[t].Hero.HealthCurr = HeroList[t].Hero.Health;
                                }

                                // Hero dies the normal way.
                                else
                                {
                                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Hero dies, " + HeroList[t].Name));
                                    // Update the Score
                                    if (TotalScore.CharacterAtDeathList == "")
                                    {
                                        TotalScore.CharacterAtDeathList += HeroList[t].Hero.ConvertToString();
                                    }
                                    else
                                    {
                                        TotalScore.CharacterAtDeathList += "," + HeroList[t].Hero.ConvertToString();
                                    }
                                    TotalScore.CharacterList.Add(new Character(HeroList[t].Hero));

                                    // Drop all the items the Character has and store into the Inventory for equipping after the battle.
                                    var held = HeroList[t].Hero.UnequipAllItems();
                                    foreach (Item i in held)
                                    {
                                        Inventory.Add(i);
                                    }

                                    // Remove dead Character from the TurnList if he has not taken a turn yet.
                                    for (int i = 0; i < TurnList.Count; i++)
                                    {
                                        if (TurnList[i].Row == HeroList[t].Row && TurnList[i].Column == HeroList[t].Column && TurnList[i].Name == HeroList[t].Name)
                                        {
                                            TurnList.RemoveAt(i);
                                            i = TurnList.Count;
                                        }
                                    }

                                    // Remove from theGrid and the party
                                    TheGrid.Grid[HeroList[t].Column][HeroList[t].Row] = new BattleTile();
                                    HeroList.RemoveAt(t);
                                    PartyList.RemoveAt(t);
                                }
                            }
                        }

                        // If it is a Character's turn
                        else
                        {
                            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Hero's turn, " + TurnList[0].Hero.Name));
                            // Character uses a potion if health is less than 20%
                            if (Potions > 0 && TurnList[0].Hero.HealthCurr <= (int)(TurnList[0].Hero.Health*.2))
                            {
                                TurnList[0].Hero.HealthCurr = TurnList[0].Hero.Health;
                                Potions--;
                                BattleSystemViewModel.Instance.AddTxt(new StringWrapper(TurnList[0].Hero.Name + " uses Potion"));
                            }

                            // Otherwise continue
                            else
                            {
                                // Randomly select enemy to fight.
                                int t = rng.Next(Enemies.Count);
                                if (t == Enemies.Count)
                                {
                                    t -= 1;
                                }
                                int previousHealth = Enemies[t].GetHealthCurr();

                                // If Focused Attack toggle is on use it if enemy's health is x5 greater than current possible damage.
                                if(BattleSystemViewModel.Instance.FocusedAttack && TurnList[0].FocusedAttack)
                                {
                                    if (previousHealth > 5 * (TurnList[0].GetAttack() + TurnList[0].GetDamage() + (int)(TurnList[0].GetLevel() / 4)))
                                    {
                                        if (TurnList[0].Hero.UnequipAllItems().Count > 0)
                                        {
                                            BattleSystemViewModel.Instance.UseFocusedAttack = true;
                                            TurnList[0].FocusedAttack = false;
                                            BattleSystemViewModel.Instance.AddTxt(new StringWrapper(TurnList[0].Hero.Name + " uses Focused Attack"));
                                        }
                                    }
                                }
                                
                                // Deal damage based on Attack type
                                int damage = Enemies[t].DealtDamage(TurnList[0]);
                                BattleSystemViewModel.Instance.AddTxt(new StringWrapper(TurnList[0].Hero.Name + " dealt " + damage + " damage to " + Enemies[t].Name));
                                string status = Enemies[t].DamageStatus;

                                // If result is a critical miss, compute and execute
                                if (Enemies[t].DamageStatus == "Critical Miss")
                                {
                                    CharacterCriticalMiss(Inventory);
                                }
                                Enemies[t].DamageStatus = "";

                                // If result is a focused attack, remove an item from the character
                                if (status == "Focused Attack")
                                {
                                    // Peak at all items equipped and pick the weakest
                                    List<Item> items = TurnList[0].Hero.UnequipAllItems();
                                    int weakest = 0;
                                    for (int i = 0; i < items.Count; i++)
                                    {
                                        if (items[weakest].Value > items[i].Value)
                                        {
                                            weakest = i;
                                        }
                                    }

                                    // Take it off
                                    Item dropped = items[weakest];
                                    TurnList[0].Hero.UnEquip(items[weakest]);
                                    if (dropped.Location == ItemLocationEnum.Head)
                                        TurnList[0].Hero.Head = new Item();
                                    else if (dropped.Location == ItemLocationEnum.Necklass)
                                        TurnList[0].Hero.Necklass = new Item();
                                    else if (dropped.Location == ItemLocationEnum.Feet)
                                        TurnList[0].Hero.Feet = new Item();
                                    else if (dropped.Location == ItemLocationEnum.PrimaryHand)
                                        TurnList[0].Hero.PrimaryHand = new Item();
                                    else if (dropped.Location == ItemLocationEnum.OffHand)
                                        TurnList[0].Hero.OffHand = new Item();
                                    else if (dropped.Location == ItemLocationEnum.Finger)
                                    {
                                        if (TurnList[0].Hero.LeftFinger.Guid == dropped.Guid)
                                            TurnList[0].Hero.LeftFinger = new Item();
                                        else
                                            TurnList[0].Hero.RightFinger = new Item();
                                    }
                                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper(TurnList[0].Hero.Name + " ddrops " + dropped.Name + " from Focused Attack"));
                                }

                                // Compute health taken from Monster
                                double percentTaken = 0;
                                if (damage > previousHealth)
                                {
                                    percentTaken = (double)previousHealth / (double)Enemies[t].Enemy.Health;
                                }
                                else
                                {
                                    double previousPercent = (double)previousHealth / (double)Enemies[t].Enemy.Health;
                                    percentTaken = previousPercent - ((double)Enemies[t].GetHealthCurr() / (double)Enemies[t].Enemy.Health);
                                }

                                // If result is anything other than a focused attack, gain experience and update the score as well
                                if (status != "Focused Attack")
                                {
                                    int expGained = (int)(Enemies[t].Enemy.ExpToGive * percentTaken);
                                    Enemies[t].Enemy.ExpToGive -= expGained;
                                    TotalScore.ExperienceGainedTotal += expGained;
                                    TotalScore.ScoreTotal += expGained;
                                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper(TurnList[0].Hero.Name + " earns " + expGained + " experience."));
                                    for (int i = 0; i < HeroList.Count; i++)
                                    {
                                        if (HeroList[i].Row == TurnList[0].Row && TurnList[0].Column == HeroList[i].Column && TurnList[0].Name == HeroList[i].Name)
                                        {
                                            HeroList[i].Hero.AddExperience(expGained);
                                            i = HeroList.Count;
                                        }
                                    }
                                }

                                // If the monster died
                                if (Enemies[t].GetHealthCurr() <= 0)
                                {
                                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper(Enemies[t].Name + " dies."));
                                    // Update the Score
                                    if (TotalScore.MonstersKilledList == "")
                                    {
                                        TotalScore.MonstersKilledList += Enemies[t].Enemy.ConvertToString();
                                    }
                                    else
                                    {
                                        TotalScore.MonstersKilledList += "," + Enemies[t].Enemy.ConvertToString();
                                    }
                                    TotalScore.MonsterSlainNumber += 1;
                                    TotalScore.MonsterList.Add(Enemies[t].Enemy);

                                    // If result is anything other than a focused attack, Monster drops an item and is placed in the ItemPool for equipping after the battle
                                    if (status != "Focused Attack")
                                    {
                                        if (TotalScore.ItemsDroppedList == "")
                                        {
                                            TotalScore.ItemsDroppedList += DropList[itemsDropped].ConvertToString();
                                        }
                                        else
                                        {
                                            if (itemsDropped >= DropList.Count)
                                            {
                                                itemsDropped = 0;
                                            }
                                            TotalScore.ItemsDroppedList += "," + DropList[itemsDropped].ConvertToString();
                                        }
                                        TotalScore.ItemList.Add(new Item(DropList[itemsDropped]));
                                        Inventory.Add(DropList[itemsDropped]);
                                        BattleSystemViewModel.Instance.AddTxt(new StringWrapper(Enemies[t].Name + " drops " + DropList[itemsDropped]));
                                        itemsDropped++;
                                    }
                                    
                                    //  Remove dead monster from the TurnList is he has not taken a turn yet.
                                    for (int i = 0; i < TurnList.Count; i++)
                                    {
                                        if (TurnList[i].Row == Enemies[t].Row && TurnList[i].Column == Enemies[t].Column && TurnList[i].Name == Enemies[t].Name)
                                        {
                                            TurnList.RemoveAt(i);
                                            i = TurnList.Count;
                                        }
                                    }

                                    // Remove from the grid as well as the enemies list.
                                    TheGrid.Grid[Enemies[t].Column][Enemies[t].Row] = new BattleTile();
                                    Enemies.RemoveAt(t);
                                }
                            }
                        }

                        // Fighter's turn has ended. Loops to the next Fighter's turn.
                        TurnList.RemoveAt(0);
                    }
                }

                // Battle has ended, Characters can now equip items that were dropped. 
                if (HeroList.Count > 0)
                {
                    BattleCount++;
                    foreach (BattleTile bt in HeroList)
                    {
                        Inventory = bt.Hero.LoadUp(Inventory);
                    }

                    // Grabs new list of items from the Web
                    await PostNewItems();
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Mass Equipping"));
                }
            }

            // Allows this function to be asynchronous. Im sorry im not handling this properly. 
            return 0;
        }

        // Collect all characters scattered on the grid.
        public List<BattleTile> CollectHeroes()
        {
            // Update PartyList
            List<BattleTile> hList = new List<BattleTile>();
            for (int j = 0; j < TheGrid.Grid.Count; j++)
            {
                for (int k = 0; k < TheGrid.Grid[j].Count; k++)
                {
                    if (TheGrid.Grid[j][k].Type == "Character" && TheGrid.Grid[j][k].GetHealthCurr() > 0)
                    {
                        hList.Add(TheGrid.Grid[j][k]);
                    }
                }
            }
            return hList;
        }

        // Collect all Monsters scattered on the grid.
        public List<BattleTile> CollectEnemies()
        {
            // Update PartyList
            List<BattleTile> hList = new List<BattleTile>();
            for (int j = 0; j < TheGrid.Grid.Count; j++)
            {
                for (int k = 0; k < TheGrid.Grid[j].Count; k++)
                {
                    if (TheGrid.Grid[j][k].Type == "Monster" && TheGrid.Grid[j][k].GetHealthCurr() > 0)
                    {
                        hList.Add(TheGrid.Grid[j][k]);
                    }
                }
            }
            return hList;
        }

        // Grabs items from the Web Post. Will break if there is no internet. 
        public async Task<int> PostNewItems()
        {
            var number = 6; // Want to get 6 items from the server
            var level = BattleCount; // With updated levels for better equipment per battle
            var attribute = AttributeEnum.Unknown;
            var location = ItemLocationEnum.Unknown;
            var random = true;
            var updateDataBase = false;

            var dropList = await ItemsController.Instance.GetItemsFromServerPost(number, level, attribute, location, random, updateDataBase);
            
            DropList = new List<Item>(dropList);
            itemsDropped = 0;

            return 0;
        }

        // Normal Battle function. A Player has choosen a tile to attack.
        public string CharacterAttacks(BattleTile tile)
        {
            string battleRecord = "";
            if (tile.Type == "Tile" || (tile.Row == TurnList[0].Row && tile.Column == TurnList[0].Column))
            {
                // Immediately returns because the player has choosen not to attack a monster.
                battleRecord += TurnList[0].Name + " does not attack.\n";
            }

            // If the player has selected a monster to attack,
            else if (tile.Type == "Monster")
            {
                // Monster is dealt damage
                int previousHealth = TheGrid.Grid[tile.Column][tile.Row].GetHealthCurr();
                int damage = TheGrid.Grid[tile.Column][tile.Row].DealtDamage(TurnList[0]);

                // If the attack misses
                string status = TheGrid.Grid[tile.Column][tile.Row].DamageStatus;
                if (status == "Miss")
                {
                    battleRecord += TurnList[0].Name + " misses attack against " + TheGrid.Grid[tile.Column][tile.Row].Name + ".\n";
                }

                // If the attack critically misses with the toggle turned on.
                else if (status == "Critical Miss")
                {
                    battleRecord += TurnList[0].Name + " CRTITICALLY misses attack against " + TheGrid.Grid[tile.Column][tile.Row].Name + ".\n";
                    battleRecord += CharacterCriticalMiss(ItemPool);
                }

                // Some damage successfully dealt.
                else
                {
                    // If the attack was a focused attack
                    if(status == "Focused Attack")
                    {
                        // Peak at equipped items and select the weakest.
                        battleRecord += TurnList[0].Name + " deals " + damage + " Focused damage to " + TheGrid.Grid[tile.Column][tile.Row].Name + ".\n";
                        List <Item> items= TurnList[0].Hero.UnequipAllItems();
                        int weakest = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[weakest].Value > items[i].Value)
                            {
                                weakest = i;
                            }
                        }

                        // Drops it
                        Item dropped = items[weakest];
                        battleRecord += TurnList[0].Name + " loses " + dropped.Name + " due to carelessness during the Focused Attack.\n";
                        TurnList[0].Hero.UnEquip(items[weakest]);
                        if (dropped.Location == ItemLocationEnum.Head)
                            TurnList[0].Hero.Head = new Item();
                        else if (dropped.Location == ItemLocationEnum.Necklass)
                            TurnList[0].Hero.Necklass = new Item();
                        else if (dropped.Location == ItemLocationEnum.Feet)
                            TurnList[0].Hero.Feet = new Item();
                        else if (dropped.Location == ItemLocationEnum.PrimaryHand)
                            TurnList[0].Hero.PrimaryHand = new Item();
                        else if (dropped.Location == ItemLocationEnum.OffHand)
                            TurnList[0].Hero.OffHand = new Item();
                        else if (dropped.Location == ItemLocationEnum.Finger)
                        {
                            if (TurnList[0].Hero.LeftFinger.Guid == dropped.Guid)
                                TurnList[0].Hero.LeftFinger = new Item();
                            else
                                TurnList[0].Hero.RightFinger = new Item();
                        }
                    }

                    // If the attack was a critical hit
                    else if(status == "Critical Hit")
                    {
                        battleRecord += TurnList[0].Name + " deals " + damage + " CRITICAL damage to " + TheGrid.Grid[tile.Column][tile.Row].Name + ".\n";
                    }

                    // Just normal damage this time
                    else
                    {
                        battleRecord += TurnList[0].Name + " deals " + damage + " damage to " + TheGrid.Grid[tile.Column][tile.Row].Name + ".\n";
                    }

                    // If it was not a focused attack, experience may be gained.
                    if (status != "Focused Attack")
                    {
                        // Compute damage taken from the monster.
                        double percentTaken = 0;
                        if (damage > previousHealth)
                        {
                            percentTaken = (double)previousHealth / (double)TheGrid.Grid[tile.Column][tile.Row].Enemy.Health;
                        }
                        else
                        {
                            double previousPercent = (double)previousHealth / (double)TheGrid.Grid[tile.Column][tile.Row].Enemy.Health;
                            percentTaken = previousPercent - ((double)TheGrid.Grid[tile.Column][tile.Row].GetHealthCurr() / (double)TheGrid.Grid[tile.Column][tile.Row].Enemy.Health);
                        }
                        int expGained = (int)(TheGrid.Grid[tile.Column][tile.Row].Enemy.ExpToGive * percentTaken);
                        TheGrid.Grid[tile.Column][tile.Row].Enemy.ExpToGive -= expGained;
                        battleRecord += TurnList[0].Name + " gains " + expGained + " experience.\n";

                        // Updates the Score
                        TotalScore.ExperienceGainedTotal += expGained;
                        TotalScore.ScoreTotal += expGained;
                        int prevLevel = TurnList[0].GetLevel();
                        TurnList[0].Hero.AddExperience(expGained);

                        // If Character levels up the healh/life is recomputed for the Life image selection
                        if (prevLevel < TurnList[0].GetLevel())
                        {
                            battleRecord += TurnList[0].Name + " has leveled up. New level is " + TurnList[0].GetLevel() + ".\n";
                            if (TurnList[0].Hero.HealthCurr < 0)
                            {
                                TurnList[0].Hero.HealthCurr = 0;
                            }
                            // Formula for computing percentage of health left by tens. Used to select which remaining health to display
                            TurnList[0].Life = "life" + (int)(((double)(TurnList[0].Hero.HealthCurr + 1) / (double)(TurnList[0].Hero.Health + 1)) * 10) + ".png";
                        }
                    }
                    else
                    {
                        // Focused attack so no experience was given.
                        battleRecord += TurnList[0].Name + " gains no experience. That fight was too easy.\n";
                    }

                    // If the Monster died from the attack.
                    if (TheGrid.Grid[tile.Column][tile.Row].GetHealthCurr() <= 0)
                    {
                        // Updates the Score
                        battleRecord += TheGrid.Grid[tile.Column][tile.Row].Name + " dies from the damage.\n";
                        TotalScore.MonsterSlainNumber += 1;
                        if(TotalScore.MonstersKilledList == "")
                        {
                            TotalScore.MonstersKilledList += TheGrid.Grid[tile.Column][tile.Row].Enemy.ConvertToString();
                        }
                        else
                        {
                            TotalScore.MonstersKilledList += "," + TheGrid.Grid[tile.Column][tile.Row].Enemy.ConvertToString();
                        }
                        TotalScore.MonsterList.Add(TheGrid.Grid[tile.Column][tile.Row].Enemy);

                        // If the attack was a focused attack, the monster drops nothing.
                        if (status == "Focused Attack")
                        {
                            battleRecord += TheGrid.Grid[tile.Column][tile.Row].Name + " drops a broken item. That focused attack really hit hard. \n";
                            BattleSystemViewModel.Instance.SetUseFocusedAttack(false);
                            TurnList[0].FocusedAttack = false;
                        }
                        else
                        {
                            // Otherwise the monster just drops a random item from the DropList.
                            // Actually the item dropped is sequential BUT the DropList contains random items so technically the drop is random.
                            // Continues to update the Score as well as placing the item into the ItemPool
                            if (itemsDropped >= DropList.Count)
                            {
                                itemsDropped = 0;
                            }
                            battleRecord += TheGrid.Grid[tile.Column][tile.Row].Name + " drops an item. " + DropList[itemsDropped].FormatOutput() + ". \n";
                            if (TotalScore.ItemsDroppedList == "")
                            {
                                TotalScore.ItemsDroppedList += DropList[itemsDropped].ConvertToString();
                            }
                            else
                            {
                                TotalScore.ItemsDroppedList += "," + DropList[itemsDropped].ConvertToString();
                            }
                            TotalScore.ItemList.Add(DropList[itemsDropped]);
                            ItemPool.Add(DropList[itemsDropped]);
                            itemsDropped++;
                        }
                        // Remove dead monster from the grid as well as the turnList
                        TheGrid.Grid[tile.Column][tile.Row] = new BattleTile(tile.Row, tile.Column);
                        for (int i = 0; i < TurnList.Count; i++)
                        {
                            if(TurnList[i].Row == tile.Row && TurnList[i].Column == tile.Column)
                            {
                                TurnList.RemoveAt(i);
                                i = TurnList.Count;
                            }
                        }
                    }
                }
                TheGrid.Grid[tile.Column][tile.Row].DamageStatus = "";
            }

            // Character's turn has ended so he is removed from the TurnList and the Attack(Red) pathing has been removed. 
            TurnList.RemoveAt(0);
            RemovePathing();

            // Returns the string records from this attack as well as giving the AI the chance to attack if it is their turn.
            return battleRecord + EnemyTurn();
        }

        // Removes the attack(Red) pathing from the Grid. 
        public void RemovePathing()
        {
            for (int i = 0; i < TheGrid.Grid.Count; i++)
            {
                for (int j = 0; j < TheGrid.Grid[i].Count; j++)
                {
                    if (TheGrid.Grid[i][j].Highlight == "Red")
                    {
                        if (TheGrid.Grid[i][j].Type == "Tile")
                        {
                            TheGrid.Grid[i][j].Highlight = "DarkGreen";
                        }
                        else if (TheGrid.Grid[i][j].Type == "Monster")
                        {
                            TheGrid.Grid[i][j].Highlight = "Yellow";
                        }
                        else if (TheGrid.Grid[i][j].Type == "Character")
                        {
                            TheGrid.Grid[i][j].Highlight = "Green";
                        }

                    }
                }
            }
        }

        // If the player selects a tile to move into. 
        public void MoveCharacter(BattleTile tile)
        {
            // Current location turns into a normal tile.
            TheGrid.Grid[TurnList[0].Column][TurnList[0].Row] = new BattleTile(TurnList[0].Row, TurnList[0].Column);
            TurnList[0].Row = tile.Row;
            TurnList[0].Column = tile.Column;

            // Remove Move(Blue) pathing from the grid
            for (int i = 0; i < TheGrid.Grid.Count; i++)
            {
                for (int j = 0; j < TheGrid.Grid[i].Count; j++)
                {
                    if (TheGrid.Grid[i][j].Highlight == "Blue")
                    {
                        if (TheGrid.Grid[i][j].Type == "Character")
                        {
                            TheGrid.Grid[i][j].Highlight = "Green";
                        }
                        else
                        {
                            TheGrid.Grid[i][j].Highlight = "DarkGreen";
                        }
                    }
                }
            }

            // Move character to the new location
            TheGrid.Grid[tile.Column][tile.Row] = TurnList[0];

            // Paint tiles the character may attack(Red)
            TheGrid.PaintWarPath(new BattleTile(TurnList[0]), 0, 0, TurnList[0].GetRange());
        }

        // Gives the Monster AI a chance to attack if it is their turn.
        public string EnemyTurn()
        {
            string battleText = OpeningText;
            OpeningText = "";

            // If all characters are dead, game over
            if (TheGrid.NoCharactersLeft())
            {
                battleText += "All party members have died. GAME OVER!\n";
                return battleText;
            }

            // If all monsters are dead, new battle
            if (TheGrid.NoMonstersLeft())
            {
                battleText += "All enemies have died. BATTLE OVER!\n";
                return battleText;
            }

            // If all Fighters have taken a turn, generate new turnList and update Score
            if (TurnList.Count == 0)
            {
                battleText += "Turn has ended.\n";
                TurnList = new List<BattleTile>(TheGrid.ComputeTurnList());
                TotalScore.TurnNumber += 1;
            }

            // Allows all enemies to take turn until it is a Character's turn or the turn is over.
            while (TurnList.Count > 0 && TurnList[0].IsMonster())
            {
                // AI turn in the Grid includes moving towards the closest Character by ManhattanDistance and attacking if within range.
                battleText += TheGrid.EnemyTakeTurn(TurnList[0].Row, TurnList[0].Column);

                // If the attack resulted in a critical miss
                if (TheGrid.DamageStatus == "Critical Miss")
                {
                    // Drop a random item and update the DropList
                    Item dropped = DropList[itemsDropped];
                    battleText += TurnList[0].Name + " accidentally drops " + dropped.Name + ".\n";
                    PostNewItems();
                }
                TheGrid.DamageStatus = "";
                // If the character has died, update the score
                if (TheGrid.recentlyDeceased.Type == "Character")
                {
                    if (TotalScore.CharacterAtDeathList == "")
                    {
                        TotalScore.CharacterAtDeathList += TheGrid.recentlyDeceased.Hero.ConvertToString();
                    }
                    else
                    {
                        TotalScore.CharacterAtDeathList += "," + TheGrid.recentlyDeceased.Hero.ConvertToString();
                    }
                    TotalScore.CharacterList.Add(new Character(TheGrid.recentlyDeceased.Hero));

                    // Drops all the items from the hero into the ItemPool
                    List<Item> heldItems = TheGrid.recentlyDeceased.Hero.UnequipAllItems();
                    foreach (Item i in heldItems)
                    {
                        ItemPool.Add(i);
                    }
                    
                    // Remove dead character from the party list by adding all the characters exept the one that died.
                    PartyList = new List<Character>();
                    for (int j = 0; j < TheGrid.Grid.Count; j++)
                    {
                        for (int k = 0; k < TheGrid.Grid[j].Count; k++)
                        {
                            if (TheGrid.Grid[j][k].Type == "Character" && TheGrid.Grid[j][k].GetHealthCurr() > 0)
                            {
                                PartyList.Add(TheGrid.Grid[j][k].Hero);
                            }
                        }
                    }

                    // Remove the dead Character from the turnlist
                    bool lookingForDeadGuy = true;
                    for (int i = 0; i < TurnList.Count && lookingForDeadGuy; i++)
                    {
                        // Deadguy found in TurnList and removed
                        if (TurnList[i].Row == TheGrid.recentlyDeceased.Row && TurnList[i].Column == TheGrid.recentlyDeceased.Column)
                        {
                            TurnList.RemoveAt(i);
                            // Update Grid
                            lookingForDeadGuy = false; 
                        }
                    }

                    TheGrid.recentlyDeceased = new BattleTile();
                }

                // This Monster's turn is over.
                TurnList.RemoveAt(0);

                // If no characters are left as a result of the attack, the game is over
                if (TheGrid.NoCharactersLeft())
                {
                    battleText += "All party members have died. GAME OVER!\n";
                    return battleText;
                }

                // If for some reason all the monsters died, the battle is over. Maybe counter attack but it was not implemented
                if (TheGrid.NoMonstersLeft())
                {
                    battleText += "All enemies have died. BATTLE OVER!\n";
                    return battleText;
                }

                // If the every fighter has had a turn, the TurnList is regenerated. 
                if (TurnList.Count == 0)
                {
                    battleText += "Turn has ended.\n";
                    TurnList = new List<BattleTile>(TheGrid.ComputeTurnList());
                    TotalScore.TurnNumber += 1;
                }
            }

            // If there are still characters left, paint a new path for the Player to move the next Character in turn.
            if(!TheGrid.NoCharactersLeft())
            {
                TheGrid.PaintPath(new BattleTile(TurnList[0]), 0, 0, TurnList[0].GetMove());
                battleText += TurnList[0].Name + "'s turn.";
            }

            // Return all the battle text generated from the fights. 
            return battleText;
        }

        // Returns the grid at its current state.
        public List<List<BattleTile>> GetGrid()
        {
            return TheGrid.Grid;
        }

        // Place fighters in the grid.
        // Includes leveling up the characters at the first battle to their designated level.
        // Includes leveling up all the new monsters.
        // Includes the Volcano event.
        // Includes initializers for the one Focused attack per battle.
        // Includes initializers for the one Resurrection per battle.
        public BattleGrid PositionCharacters()
        {
            BattleGrid bg = new BattleGrid(Rows, Columns);
            
            // Levels them up to their preset levels
            // Gives them full health
            if (BattleCount <= 1)
            { 
                for (int i = 0; i < PartyList.Count; i++)
                {
                    PartyList[i].SetLevel(PartyList[i].Level);
                    PartyList[i].HealthCurr = PartyList[i].Health;
                }
            }

            bool goHome = false; 

            // MASS VOLCANO EVENT!!!!
            if (MassVolcano)
            {
                // RNG
                Random rng = new Random();
                int roll = 1 + rng.Next(20);
                
                // Characters gain full health
                if (roll == 1)
                {
                    for (int i = 0; i < PartyList.Count; i++)
                    {
                        PartyList[i].HealthCurr = PartyList[i].Health;
                    }
                    OpeningText += "A nearby Volcano massively errupts. Characters have been fully healed due to over preparedness!\n";
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Volcano Erupts Heals Characters"));
                }

                // Characters take 20% damage
                else if (roll == 2)
                {
                    for (int i = 0; i < PartyList.Count; i++)
                    {
                        PartyList[i] = new Character(PartyList[i]);
                    }
                    for (int i = 0; i < PartyList.Count; i++)
                    {
                        PartyList[i].HealthCurr = PartyList[i].HealthCurr - (int)Math.Ceiling(PartyList[i].Health*.2);
                        if (PartyList[i].HealthCurr <= 0)
                        {
                            PartyList[i].HealthCurr = 1;
                        }
                    }
                    OpeningText += "A nearby Volcano massively errupts. Characters suffer from fumes in the sewer!(-20%HP)\n";
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Volcano Errupts Fumes Character"));
                }

                // Monsters take 20% damage
                else if (roll == 3)
                {
                    for (int i = 0; i < EnemyList.Count; i++)
                    {
                        EnemyList[i] = new Monster(EnemyList[i]);
                    }
                    for (int i = 0; i < EnemyList.Count; i++)
                    {
                        EnemyList[i].HealthCurr = EnemyList[i].HealthCurr - (int)Math.Ceiling(EnemyList[i].Health * .2);
                        if (EnemyList[i].HealthCurr <= 0)
                        {
                            EnemyList[i].HealthCurr = 1;
                        }
                    }
                    OpeningText += "A nearby Volcano massively errupts. Monsters suffer from fumes in the sewer!(-20%HP)\n";
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Volcano Errupts Fumes Monsters"));
                }

                // Strongest runs away
                else if (roll == 4)
                {
                    int strongest = 0;
                    int strongestStats = EnemyList[0].Health +
                        EnemyList[0].Attack +
                        EnemyList[0].Defense +
                        EnemyList[0].Speed +
                        EnemyList[0].Move +
                        EnemyList[0].Range +
                        EnemyList[0].Damage;
                    for (int i = 0; i < EnemyList.Count; i++)
                    {
                        int nextStats = EnemyList[i].Health +
                        EnemyList[i].Attack +
                        EnemyList[i].Defense +
                        EnemyList[i].Speed +
                        EnemyList[i].Move +
                        EnemyList[i].Range +
                        EnemyList[i].Damage;

                        if (strongestStats < nextStats)
                        {
                            strongest = i;
                            strongestStats = nextStats;
                        }
                    }

                    Monster strong = EnemyList[strongest];
                    EnemyList.RemoveAt(strongest);
                    goHome = true;
                    OpeningText += "A nearby Volcano massively errupts. The strongest Monster runs home to protect their family!(enemies -1)\n";
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Volcano Errupts Monster runs away"));
                }
            }

            // Places and initializes the characters hard coded style rather than intense math.
            // Top is full party placed and bottom is one party member placed.
            switch (PartyList.Count)
            {
                case 6:
                    bg.Grid[1][5] = (new BattleTile(new Character(PartyList[0]), 5, 1));
                    bg.Grid[2][5] = (new BattleTile(new Character(PartyList[1]), 5, 2));
                    bg.Grid[3][5] = (new BattleTile(new Character(PartyList[2]), 5, 3));
                    bg.Grid[1][4] = (new BattleTile(new Character(PartyList[3]), 4, 1));
                    bg.Grid[2][4] = (new BattleTile(new Character(PartyList[4]), 4, 2));
                    bg.Grid[3][4] = (new BattleTile(new Character(PartyList[5]), 4, 3));

                    if (BattleSystemViewModel.Instance.FocusedAttack)
                    {
                        bg.Grid[1][5].FocusedAttack = true;
                        bg.Grid[2][5].FocusedAttack = true;
                        bg.Grid[3][5].FocusedAttack = true;
                        bg.Grid[1][4].FocusedAttack = true;
                        bg.Grid[2][4].FocusedAttack = true;
                        bg.Grid[3][4].FocusedAttack = true;
                    }
                    if (BattleSystemViewModel.Instance.MostlyDead)
                    {
                        bg.Grid[1][5].MostlyDead = true;
                        bg.Grid[2][5].MostlyDead = true;
                        bg.Grid[3][5].MostlyDead = true;
                        bg.Grid[1][4].MostlyDead = true;
                        bg.Grid[2][4].MostlyDead = true;
                        bg.Grid[3][4].MostlyDead = true;
                    }
                    break;
                case 5:
                    bg.Grid[1][5] = (new BattleTile(new Character(PartyList[0]), 5, 1));
                    bg.Grid[2][5] = (new BattleTile(new Character(PartyList[1]), 5, 2));
                    bg.Grid[3][5] = (new BattleTile(new Character(PartyList[2]), 5, 3));
                    bg.Grid[1][4] = (new BattleTile(new Character(PartyList[3]), 4, 1));
                    bg.Grid[2][4] = (new BattleTile(new Character(PartyList[4]), 4, 2));

                    if (BattleSystemViewModel.Instance.FocusedAttack)
                    {
                        bg.Grid[1][5].FocusedAttack = true;
                        bg.Grid[2][5].FocusedAttack = true;
                        bg.Grid[3][5].FocusedAttack = true;
                        bg.Grid[1][4].FocusedAttack = true;
                        bg.Grid[2][4].FocusedAttack = true;
                    }
                    if (BattleSystemViewModel.Instance.MostlyDead)
                    {
                        bg.Grid[1][5].MostlyDead = true;
                        bg.Grid[2][5].MostlyDead = true;
                        bg.Grid[3][5].MostlyDead = true;
                        bg.Grid[1][4].MostlyDead = true;
                        bg.Grid[2][4].MostlyDead = true;
                    }
                    break;
                case 4:
                    bg.Grid[1][5] = (new BattleTile(new Character(PartyList[0]), 5, 1));
                    bg.Grid[2][5] = (new BattleTile(new Character(PartyList[1]), 5, 2));
                    bg.Grid[3][5] = (new BattleTile(new Character(PartyList[2]), 5, 3));
                    bg.Grid[1][4] = (new BattleTile(new Character(PartyList[3]), 4, 1));

                    if (BattleSystemViewModel.Instance.FocusedAttack)
                    {
                        bg.Grid[1][5].FocusedAttack = true;
                        bg.Grid[2][5].FocusedAttack = true;
                        bg.Grid[3][5].FocusedAttack = true;
                        bg.Grid[1][4].FocusedAttack = true;
                    }

                    if (BattleSystemViewModel.Instance.MostlyDead)
                    {
                        bg.Grid[1][5].MostlyDead = true;
                        bg.Grid[2][5].MostlyDead = true;
                        bg.Grid[3][5].MostlyDead = true;
                        bg.Grid[1][4].MostlyDead = true;
                    }
                    break;
                case 3:
                    bg.Grid[1][5] = (new BattleTile(new Character(PartyList[0]), 5, 1));
                    bg.Grid[2][5] = (new BattleTile(new Character(PartyList[1]), 5, 2));
                    bg.Grid[3][5] = (new BattleTile(new Character(PartyList[2]), 5, 3));

                    if (BattleSystemViewModel.Instance.FocusedAttack)
                    {
                        bg.Grid[1][5].FocusedAttack = true;
                        bg.Grid[2][5].FocusedAttack = true;
                        bg.Grid[3][5].FocusedAttack = true;
                    }

                    if (BattleSystemViewModel.Instance.MostlyDead)
                    {
                        bg.Grid[1][5].MostlyDead = true;
                        bg.Grid[2][5].MostlyDead = true;
                        bg.Grid[3][5].MostlyDead = true;
                    }
                    break;
                case 2:
                    bg.Grid[1][5] = (new BattleTile(new Character(PartyList[0]), 5, 1));
                    bg.Grid[2][5] = (new BattleTile(new Character(PartyList[1]), 5, 2));

                    if (BattleSystemViewModel.Instance.FocusedAttack)
                    {
                        bg.Grid[1][5].FocusedAttack = true;
                        bg.Grid[2][5].FocusedAttack = true;
                    }

                    if (BattleSystemViewModel.Instance.MostlyDead)
                    {
                        bg.Grid[1][5].MostlyDead = true;
                        bg.Grid[2][5].MostlyDead = true;
                    }
                    break;
                case 1:
                    bg.Grid[1][5] = (new BattleTile(new Character(PartyList[0]), 5, 1));

                    if (BattleSystemViewModel.Instance.FocusedAttack)
                    {
                        bg.Grid[1][5].FocusedAttack = true;
                    }

                    if (BattleSystemViewModel.Instance.MostlyDead)
                    {
                        bg.Grid[1][5].MostlyDead = true;
                    }
                    break;
            }

            // Always place 6 new monsters except when the strongest runs away from the volcano.
            bg.Grid[3][0] = (new BattleTile(EnemyList[0], 0, 3));
            bg.Grid[2][0] = (new BattleTile(EnemyList[1], 0, 2));
            bg.Grid[1][0] = (new BattleTile(EnemyList[2], 0, 1));
            bg.Grid[3][1] = (new BattleTile(EnemyList[3], 1, 3));
            bg.Grid[2][1] = (new BattleTile(EnemyList[4], 1, 2));
            if (!goHome)
            {
                bg.Grid[1][1] = (new BattleTile(EnemyList[5], 1, 1));
            }
            return bg;
        }

        // Generate new list of Monsters from the CRUDI list and sets their level to the current number of battles for scaling.
        public List<Monster> GenerateEnemies()
        {
            Random randomizer = new Random();
            List<Monster> enemies = new List<Monster>();
            for (var i = 0; i < EnemyCount; i++)
            {
                Monster enemy = new Monster();
                if (MonsterList.Count > 0)
                {
                    enemy = new Monster(MonsterList[randomizer.Next(MonsterList.Count)]);
                }
                enemy.SetLevel(BattleCount);
                enemies.Add(enemy);
            }
            return enemies;
        }

        // Determines if the game is over
        public bool IsGameOver()
        {
            return TheGrid.NoCharactersLeft();
        }

        // Determines if the battle is over
        public bool IsBattleOver()
        {
            return TheGrid.NoMonstersLeft();
        }

        // Returns the score 
        public Score GameOver()
        {
            TotalScore.GameDate = DateTime.Now;
            TotalScore.Name = "Total Battles: " + BattleCount;
            return TotalScore;
        }

        // Same as CollectHeroes but it returns the characters in the tiles instead of the tile itself. 
        public List<Character> BattleOver()
        {
            PartyList = new List<Character>();
            for (int j = 0; j < TheGrid.Grid.Count; j++)
            {
                for (int k = 0; k < TheGrid.Grid[j].Count; k++)
                {
                    if (TheGrid.Grid[j][k].Type == "Character" && TheGrid.Grid[j][k].GetHealthCurr() > 0)
                    {
                        PartyList.Add(TheGrid.Grid[j][k].Hero);
                    }
                }
            }
            return PartyList;
        }

        // Hackathon Character's CriticalMiss calculation
        public string CharacterCriticalMiss(List<Item> iList)
        {
            string record = "";

            Random rng = new Random();
            int roll = 1 + rng.Next(10);
            // If the Character has to destroy his primary hand item
            if (roll == 1)
            {
                // Determine the primary hand item
                Item dropped = TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.PrimaryHand;
                TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.PrimaryHand = new Item();
                if (dropped.Name != "Unknown")
                {
                    // Had a primary hand item equipped. Loses it and does not go into the ItemPool
                    record += TurnList[0].Name + " loses " + dropped.Name + " forever!\n";
                    TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.UnEquip(dropped);
                }
                else
                {
                    // No primary hand item equipped. Loses nothing.
                    record += TurnList[0].Name + " has no primary hand item. Nothing lost.\n";
                }
            }

            // If the character has to drop his primary hand item
            else if (roll == 2 || roll == 3 || roll == 4)
            {
                //Determine the primary hand item
                Item dropped = TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.PrimaryHand;;
                if (dropped.Name != "Unknown")
                {
                    // Had a primary hand item equipped. Loses it into the item pool or inventory
                    record += TurnList[0].Name + " loses " + dropped.Name + ". It drops back into the item pool!\n";
                    TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.UnEquip(dropped);
                    iList.Add(dropped);
                }
                else
                {
                    // No primary hand item equipped. Loses nothing.
                    record += TurnList[0].Name + " has no primary hand item. Nothing lost.\n";
                }

            // If the Character has to drop a random item 
            }else if (roll == 5 || roll == 6)
            {
                // Get copy of all items equipped
                List<Item> equipment = TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.UnequipAllItems();
                if(equipment.Count > 0)
                {
                    // Randomy select one
                    Random rng2 = new Random();
                    int loc = rng2.Next(equipment.Count);
                    Item dropped = equipment[loc];
                    iList.Add(dropped);
                    equipment.RemoveAt(loc);
                    TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.UnEquip(dropped);

                    // Unequip it and place into inventory/item pool
                    if (dropped.Location == ItemLocationEnum.Head)
                        TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.Head = new Item();
                    else if (dropped.Location == ItemLocationEnum.Necklass)
                        TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.Necklass = new Item();
                    else if (dropped.Location == ItemLocationEnum.Feet)
                        TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.Feet = new Item();
                    else if (dropped.Location == ItemLocationEnum.PrimaryHand)
                        TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.PrimaryHand = new Item();
                    else if (dropped.Location == ItemLocationEnum.OffHand)
                        TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.OffHand = new Item();
                    else if (dropped.Location == ItemLocationEnum.Finger)
                    {
                        if (TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.LeftFinger.Guid == dropped.Guid)
                            TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.LeftFinger = new Item();
                        else
                            TheGrid.Grid[TurnList[0].Column][TurnList[0].Row].Hero.RightFinger = new Item();
                    }
                    record += TurnList[0].Name + " loses " + dropped.Name + ". It drops back into the item pool!\n";
                }

                // Nothing equipped. Loses nothing.
                else
                {
                    record += TurnList[0].Name + " has no items equipped. Nothing lost.\n";
                }
            }

            // If the character had luck
            else
            {
                record += "Nothing bad happens, luck was with " + TurnList[0].Name + ".\n";
            }
            return record;
        }
    }
}