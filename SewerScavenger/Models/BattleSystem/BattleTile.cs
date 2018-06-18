using System;
using System.Collections.Generic;
using System.Text;

using SewerScavenger.ViewModels;

namespace SewerScavenger.Models
{
    public class BattleTile
    {
        // Monster or Character stats for display and computation
        public Monster Enemy { get; set; }          // If tile contains a monster then it is saved here
        public Character Hero { get; set; }         // If tile contains a character then it is saved here
        public int Row;                             // Current row of this tile
        public int Column;                          // Current column of this tile
        public string Highlight { get; set; }       // Background color for this tile
        public string Image { get; set; }           // Image associated with this tile
        public string Type { get; set; }            // Type of tile (Monster, Character, Empty)
        public string Life { get; set; }            // Life bar image based on current health percentage of Monster or Character
        public string Name { get; set; }            // Name of the Character or monster in this tile. 

        // Hackathon options
        public bool DisableRNG = false;
        public int ToHit = 0;
        public bool Miss1 = true;
        public bool Hit20 = true;
        public bool CriticalMiss1 = false;
        public bool Critical20 = false;
        public bool FocusedAttack = false;
        public bool MostlyDead = false;

        // State return for access after DealtDamage function.
        public string DamageStatus = "";

        // Automatic setup with a new tile to implement the changes in the debug options
        public void Settings()
        {
            DisableRNG = BattleSystemViewModel.Instance.DisableRNG;
            ToHit = BattleSystemViewModel.Instance.ToHit;
            Miss1 = BattleSystemViewModel.Instance.Miss1;
            Hit20 = BattleSystemViewModel.Instance.Hit20;
            CriticalMiss1 = BattleSystemViewModel.Instance.CriticalMiss1;
            Critical20 = BattleSystemViewModel.Instance.Critical20;
        }

        // Constructor for a new tile. May exist without location on the grid.
        public BattleTile(int r=-1, int c=-1)
        {
            Image = "";
            Row = r;
            Column = c;
            Highlight = "DarkGreen";
            Life = "";
            Type = "Tile";
            Name = "Tile";
            Settings();
        }

        // Constructor for a new tile containing a Monster
        public BattleTile(Monster m, int r=-1, int c=-1)
        {
            Enemy = m;
            Image = Enemy.Image;
            Highlight = "Yellow";
            Row = r;
            Column = c;
            if (m.HealthCurr < 0)
            {
                m.HealthCurr = 0;
            }
            Life = "life" + (int)(((double)(m.HealthCurr + 1) / (double)(m.Health + 1)) * 10) + ".png";

            Type = "Monster";
            Name = Enemy.Name;
            Settings();
        }

        // Constructor for a new tile containing a Character
        public BattleTile(Character ch, int r=-1, int c=-1)
        {
            Hero = ch;
            Image = Hero.Image;
            Highlight = "Green";
            Row = r;
            Column = c;
            if (ch.HealthCurr < 0)
            {
                ch.HealthCurr = 0;
            }
            Life = "life" + (int)(((double)(ch.HealthCurr + 1) / (double)(ch.Health + 1)) * 10) + ".png";
            Type = "Character";
            Name = Hero.Name;
            Settings();
        }

        // Constructor to make a new instance of a tile
        public BattleTile(BattleTile bt)
        {
            Type = bt.Type;
            if (bt.Type == "Monster")
            {
                Enemy = bt.Enemy;
            }
            if (bt.Type == "Character")
            {
                Hero = bt.Hero;
            }
            Row = bt.Row;
            Column = bt.Column;
            Highlight = bt.Highlight;
            Image = bt.Image;
            Life = bt.Life;
            Name = bt.Name;
            Settings();
        }

        // Damage dealing function that works with either monster or character
        public int DealtDamage(BattleTile attacker)
        {
            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Damage Calculation Begins"));
            // Roll 20 for attack type.
            Random rng = new Random();
            int roll = 1+rng.Next(20);
            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Rolled: " + roll));

            // Applies to hit Rule
            if (ToHit > 0)
            {
                roll = ToHit;
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("ToHit Applied: " + ToHit));
            }

            // Applies Miss at roll/ToHit 1 Rule
            if (Miss1 && roll == 1)
            {
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Missed"));
                DamageStatus = "Miss";
                return 0;
            // Applies Critical Miss at 1 rule
            }else if (CriticalMiss1 && roll == 1)
            {
                DamageStatus = "Critical Miss";
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Critical Miss"));
                return 0;
            }

            // Applies Hit at roll/ToHit 20 Rule
            bool hit = false;
            bool criticalHit = false;
            bool focusedAttack = false;
            if (Hit20 && roll == 20)
            {
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Hit20"));
                DamageStatus = "Hit";
                hit = true;
            
            // Applies critical hit rule at 20
            }else if (Critical20 && roll == 20)
            {
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Critical Hit"));
                DamageStatus = "Critical Hit";
                criticalHit = true;

            // Applies focused attack rule
            }else if (BattleSystemViewModel.Instance.FocusedAttack && BattleSystemViewModel.Instance.UseFocusedAttack)
            {
                if (attacker.Type == "Character" && attacker.Hero.UnequipAllItems().Count > 0)
                {
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Focused Attack Applied"));
                    DamageStatus = "Focused Attack";
                    FocusedAttack = true;
                }
            }

            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Attacker Attack: " + attacker.GetAttack()));
            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Attacker Level: " + attacker.GetLevel()));
            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Target Defense: " + GetDefense()));
            BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Target Level: " + GetLevel()));
            // Hit Designation
            if (roll + attacker.GetLevel() + attacker.GetAttack() > GetDefense() + GetLevel() || hit || criticalHit || focusedAttack)
            {
                int damage = 0;
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Hit Success"));
                // Level Damage
                int lvlDamage = (int)(attacker.GetLevel() * .25)+1;
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Level Damage: " + lvlDamage));
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Weapon Damage: " + attacker.GetDamage()));
                // Applies Disable RNG Rule
                if (DisableRNG)
                {
                    damage = attacker.GetDamage() + lvlDamage + attacker.GetAttack();
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("RNG Disabled damage: " + damage));
                }

                // RNG enabled rule
                else
                {
                    Random weaponDamage = new Random();
                    damage = (1 + weaponDamage.Next(attacker.GetDamage() + attacker.GetAttack())) + lvlDamage;
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Resulting Damage: " + damage));
                }

                // Critical hit doubles the damage dealt
                if (criticalHit)
                {
                    damage = 2 * damage;
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Critical Applied damage: " + damage));
                }

                // Focused attack deals 10 times the damage dealt
                if (focusedAttack)
                {
                    damage = 10 * damage;
                    BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Focus Attack Applied damage: " + damage));
                }

                // Update life bar of this Combatant
                UpdateLifeBar(damage);

                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("End Damage"));
                // Return damage delt for recording
                return damage;
            }
            // Missed
            return 0;
        }

        // Updates to the current health of a monster or character need to update the life bar as well.
        private void UpdateLifeBar(int damage)
        {
            // Update Life bar for monster
            if (Type == "Monster")
            {
                Enemy.HealthCurr -= damage;
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Monster remaining Health: " + Enemy.HealthCurr));
                if (Enemy.HealthCurr < 0)
                {
                    Enemy.HealthCurr = 0;
                }
                // Life bar selection formula
                Life = "life" + (int)(((double)(Enemy.HealthCurr + 1) / (double)(Enemy.Health + 1)) * 10) + ".png";
            }

            // Update Life bar for character
            else if (Type == "Character")
            {
                Hero.HealthCurr -= damage;
                BattleSystemViewModel.Instance.AddTxt(new StringWrapper("Hero remaining Health: " + Hero.HealthCurr));
                if (Hero.HealthCurr < 0)
                {
                    Hero.HealthCurr = 0;
                }
                // Life bar selection formula
                Life = "life" + (int)(((double)(Hero.HealthCurr + 1) / (double)(Hero.Health + 1)) * 10) + ".png";
            }

            // If it is a tile then the Life bar should not be displayed
            else
            {
                Life = "";
            }
        }

        // When a player selects a tile for information, this function compiles and returns the relevant stats
        public string GetAllInfo()
        {
            string compile = "";

            // Tile is a Monster
            if (Type == "Monster")
            {
                if (Row >= 0 && Column >= 0)
                {
                    compile += "Row: " + Row + " Column: " + Column + "\n\n";
                }
                compile += "Health : " + Enemy.HealthCurr + "/" + Enemy.Health + "\n";
                compile += "Attack : " + Enemy.Attack + "\n";
                compile += "Damage : " + Enemy.Damage + "\n";
                compile += "Defense: " + Enemy.Defense + "\n";
                compile += "Speed  : " + Enemy.Speed + "\n";
                compile += "Range  : " + Enemy.Range + "\n";
                compile += "Moves  : " + Enemy.Move + "\n\n";
                compile += Enemy.Description;
            }

            // Tile is a Character
            else if (Type == "Character")
            {
                if (Row >= 0 && Column >= 0)
                {
                    compile += "Row: " + Row + " Column: " + Column + "\n\n";
                }
                compile += "Health : " + Hero.HealthCurr + "/" + Hero.Health + "\n";
                compile += "Attack : " + Hero.Attack + "\n";
                compile += "Damage : " + Hero.Damage + "\n";
                compile += "Defense: " + Hero.Defense + "\n";
                compile += "Speed  : " + Hero.Speed + "\n";
                compile += "Range  : " + Hero.Range + "\n";
                compile += "Moves  : " + Hero.Move + "\n\n";
                compile += Hero.Description;
            }

            // Tile is empty
            else
            {
                compile = "Just an empty tile.";
            }
            compile += "\n\nTile Legends - Green: Ally, Yellow: Enemy, Blue: Character Path, Red: Target.";
            return compile;
        }

        // Title of an information request to be displayed as DisplayAlert. 
        public string GetTitleInfo()
        {
            string compile = "";

            // If tile is a monster
            if (Type == "Monster")
            {
                compile += Enemy.Name + " lvl " + Enemy.Level;
            }

            // If tile is a Character
            else if (Type == "Character")
            {
                compile += Hero.Name + " lvl " + Hero.Level;
            }

            // If tile is empty
            else
            {
                compile = "Tile";
            }

            return compile;
        }

        // Returns the current health of the monster or character in this tile.
        public int GetHealthCurr()
        {
            if (Type == "Monster")
            {
                return Enemy.HealthCurr;
            }
            else if (Type == "Character")
            {
                return Hero.HealthCurr;
            }
            else
            {
                return 0;
            }
        }

        // Returns the damage of the monster or character in this tile.
        public int GetDamage()
        {
            if (Type == "Monster")
            {
                return Enemy.Damage;
            }
            else if (Type == "Character")
            {
                return Hero.Damage;
            }
            else
            {
                return 0;
            }
        }

        // Returns the attack of the monster or character in this tile.
        public int GetAttack()
        {
            if (Type == "Monster")
            {
                return Enemy.Attack;
            }
            else if (Type == "Character")
            {
                return Hero.Attack;
            }
            else
            {
                return 0;
            }
        }

        // Returns the defense of the monster or character in this tile.
        public int GetDefense()
        {
            if (Type == "Monster")
            {
                return Enemy.Defense;
            }
            else if (Type == "Character")
            {
                return Hero.Defense;
            }
            else
            {
                return 0;
            }
        }

        // Returns the level of the monster or character in this tile.
        public int GetLevel()
        {
            if (Type == "Monster")
            {
                return Enemy.Level;
            }
            else if (Type == "Character")
            {
                return Hero.Level;
            }
            else
            {
                return 0;
            }
        }

        // Returns the range of the monster or character in this tile.
        public int GetRange()
        {
            if (Type == "Monster")
            {
                return Enemy.Range;
            }
            else if (Type == "Character")
            {
                return Hero.Range;
            }
            else
            {
                return 0;
            }
        }

        // Returns the number of moves of the monster or character in this tile.
        public int GetMove()
        {
            if (Type == "Monster")
            {
                return Enemy.Move;
            }
            else if (Type == "Character")
            {
                return Hero.Move;
            }
            else
            {
                return 0;
            }
        }

        // Returns the speed of the monster or character in this tile.
        public int GetSpeed()
        {
            if (Type == "Monster")
            {
                return Enemy.Speed;
            }
            else if (Type == "Character")
            {
                return Hero.Speed;
            }
            else
            {
                return 0;
            }
        }

        // Returns if the tile contains a monster or character.
        public bool IsMonster()
        {
            if(Type == "Monster")
            {
                return true;
            }
            return false; 
        }
    }
}
