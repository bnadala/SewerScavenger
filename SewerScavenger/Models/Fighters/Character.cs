using System;
using SQLite;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SewerScavenger.Models
{
    public class Character : Entity<Item>
    {
        // Basic Character stats
        public int Level { get; set; }          // Level of the Character
        public int Health { get; set; }         // Maximum health of the character
        public int Damage { get; set; }         // Damage of the character. Only influenced by items
        public int Attack { get; set; }         // Attack of the character to determine hit and damage as well
        public int Defense { get; set; }        // Defense of the character to determine a missed attack
        public int Speed { get; set; }          // Speed of the character to determine the turnlist position
        public int Move { get; set; }           // Moves the character can make. Cannnot be editted outside of character creation
        public int Range { get; set; }          // Range of the character's attack. Only influenced by items

        // Equipment locations
        public Item Head = new Item();          
        public Item Necklass = new Item();      // Named as such because of the Post items returned naming sense. 
        public Item PrimaryHand = new Item();
        public Item OffHand = new Item();
        public Item RightFinger = new Item();
        public Item LeftFinger = new Item();
        public Item Feet = new Item();

        // BattleSystem usable stats
        public int HealthCurr { get; set; }     // Current health
        public int XP { get; set; }             // Current experience so far

        // Character levelup table with added level 20 stats.
        // Character level starts at 1 and ends at 20 max. 
        public int[,] LevelUpTable = new int[,]
        {
            { 0, 0, 0, 0 }, { 300, 1, 2, 1 }, { 900, 2, 3, 1 },
            { 2700, 2, 3, 1}, { 6500, 2, 4, 2}, { 14000, 3, 4, 2},
            { 23000, 3, 5, 2}, { 34000, 3, 5, 2}, { 48000, 3, 5, 2},
            { 64000, 4, 6, 3}, { 85000, 4, 6, 3}, { 100000, 4, 6, 3},
            { 120000, 4, 7, 3}, { 140000, 5, 7, 3}, { 165000, 5, 7, 4},
            { 195000, 5, 8, 4}, { 225000, 5, 8, 4}, { 265000, 6, 8, 4},
            { 305000, 7, 9, 4}, { 355000, 8, 10, 5}, { 410000, 10, 12, 6}
        };

        // Constructor for creating a new instance of a Character
        public Character(Character newData)
        {
            Name = newData.Name;
            Description = newData.Description;
            Level = newData.Level;
            Image = newData.Image;
            HealthCurr = newData.HealthCurr;
            Health = newData.Health;
            Attack = newData.Attack;
            Defense = newData.Defense;
            Speed = newData.Speed;
            Move = newData.Move;
            Range = newData.Range;
            XP = newData.XP;
            Damage = newData.Damage;
            Head = newData.Head;
            Necklass = newData.Necklass;
            PrimaryHand = newData.PrimaryHand;
            OffHand = newData.OffHand;
            RightFinger = newData.RightFinger;
            LeftFinger = newData.LeftFinger;
            Feet = newData.Feet;
        }

        // Constructor for new empty character
        public Character()
        {
            Name = "";
            Description = "";
            Level = 1;
            Image = "";
            HealthCurr = 1;
            Health = 1;
            Attack = 1;
            Defense = 1;
            Speed = 1;
            Move = 1;
            Range = 1;
            XP = 0;
            Damage = 1;
            Head = new Item();
            Necklass = new Item();
            PrimaryHand = new Item();
            OffHand = new Item();
            RightFinger = new Item();
            LeftFinger = new Item();
            Feet = new Item();
        }

        // Copy stats from a character into this character
        public void Update(Character newData)
        {
            if (newData == null)
            {
                return;
            }

            // Update all the fields in the Data, except for the Id
            Name = newData.Name;
            Description = newData.Description;
            Level = newData.Level;
            Image = newData.Image;
            Health = newData.Health;
            Attack = newData.Attack;
            Defense = newData.Defense;
            Speed = newData.Speed;
            Move = newData.Move;
            Range = newData.Range;
            HealthCurr = newData.HealthCurr;
            XP = newData.XP;
            Damage = newData.Damage;
            Head = newData.Head;
            Necklass = newData.Necklass;
            PrimaryHand = newData.PrimaryHand;
            OffHand = newData.OffHand;
            RightFinger = newData.RightFinger;
            LeftFinger = newData.LeftFinger;
            Feet = newData.Feet;
        }

        // Returns a list of all items this character has equipped except for empty items
        public List<Item> UnequipAllItems()
        {
            List<Item> held = new List<Item>();
            if(Head.Name != "Unknown")
            {
                held.Add(Head);
            }
            if (Necklass.Name != "Unknown")
            {
                held.Add(Necklass);
            }
            if (PrimaryHand.Name != "Unknown")
            {
                held.Add(PrimaryHand);
            }
            if (OffHand.Name != "Unknown")
            {
                held.Add(OffHand);
            }
            if (RightFinger.Name != "Unknown")
            {
                held.Add(RightFinger);
            }
            if (LeftFinger.Name != "Unknown")
            {
                held.Add(LeftFinger);
            }
            if (Feet.Name != "Unknown")
            {
                held.Add(Feet);
            }
            return held;
        }

        // Attempts to equip all the items passed into the function into this character.
        public List<Item> LoadUp(List<Item> inventory)
        {
            List<Item> rejectPile = new List<Item>();
            foreach (Item item in inventory)
            {
                // Cannot equip an empty item
                if(item.Name != "Unknown")
                {
                    // Attempts to equip item. Returns unequipped or rejected items.
                    Item temp = TryItOn(item);
                    if (temp.Name != "Unknown")
                    {
                        rejectPile.Add(temp);
                    }
                }
                
            }
            return rejectPile;
        }

        // If the item passed in is better than the one currently equipped it is replaced otherwise it is returned. 
        public Item TryItOn(Item item)
        {
            if (item.Location == ItemLocationEnum.Head)
            {
                if (Head.Value < item.Value)
                {
                    Item temp = UnEquip(Head);
                    Head = Equip(item);
                    return temp;
                }
            }
            else if (item.Location == ItemLocationEnum.Necklass)
            {
                if (Necklass.Value < item.Value)
                {
                    Item temp = UnEquip(Necklass);
                    Necklass = Equip(item);
                    return temp;
                }
            }
            else if (item.Location == ItemLocationEnum.PrimaryHand)
            {
                if (PrimaryHand.Value < item.Value)
                {
                    Item temp = UnEquip(PrimaryHand);
                    PrimaryHand = Equip(item);
                    return temp;
                }
            }
            else if (item.Location == ItemLocationEnum.OffHand)
            {
                if (OffHand.Value < item.Value)
                {
                    Item temp = UnEquip(OffHand);
                    OffHand = Equip(item);
                    return temp;
                }
            }
            // Has to check if item is acceptable for the left or the right finger so extra checks are necessary
            else if (item.Location == ItemLocationEnum.Finger)
            {
                // Check left finger first
                if (LeftFinger.Value < item.Value)
                {
                    Item temp = UnEquip(LeftFinger);
                    LeftFinger = Equip(item);

                    // Now that the left is finished checking, the returned item is compared to the right finger
                    if(RightFinger.Value < temp.Value && ItemLocationEnum.LeftFinger != temp.Location)
                    {
                        Item temp2 = UnEquip(RightFinger);
                        RightFinger = Equip(temp);
                        return temp2;
                    }
                    else
                    {
                        return temp;
                    }
                }

                // Check right finger next
                else if (RightFinger.Value < item.Value)
                {
                    Item temp = UnEquip(RightFinger);
                    RightFinger = Equip(item);

                    // Now that the right finger has been eqipped, the left finger is checked next. 
                    if (LeftFinger.Value < temp.Value && ItemLocationEnum.RightFinger != temp.Location)
                    {
                        Item temp2 = UnEquip(LeftFinger);
                        LeftFinger = Equip(temp);
                        return temp2;
                    }
                    else
                    {
                        return temp;
                    }
                }
            }

            // Just in case the item is somehow designated as left finger
            else if (item.Location == ItemLocationEnum.LeftFinger)
            {
                if (LeftFinger.Value < item.Value)
                {
                    Item temp = UnEquip(LeftFinger);
                    LeftFinger = Equip(item);

                    // Checks the right finger as well
                    if (RightFinger.Value < item.Value && temp.Location == ItemLocationEnum.Finger)
                    {
                        Item temp2 = UnEquip(RightFinger);
                        RightFinger = Equip(temp);
                        return temp2;
                    }
                    else
                    {
                        return temp;
                    }
                }
            }

            // Just in case the item is designated as right finger
            else if (item.Location == ItemLocationEnum.RightFinger)
            {
                if (RightFinger.Value < item.Value)
                {
                    Item temp = UnEquip(RightFinger);
                    RightFinger = Equip(item);

                    // Checks if the return on the left finger is better.
                    if (LeftFinger.Value < item.Value && temp.Location == ItemLocationEnum.Finger)
                    {
                        Item temp2 = UnEquip(LeftFinger);
                        LeftFinger = Equip(temp);
                        return temp2;
                    }
                    else
                    {
                        return temp;
                    }
                }
            }
            else if (item.Location == ItemLocationEnum.Feet)
            {
                if (Feet.Value < item.Value)
                {
                    Item temp = UnEquip(Feet);
                    Feet = Equip(item);
                    return temp;
                }
            }
            return item;
        }

        // Equip in the sense that the items stats are applied but the item is not necessarily placed.
        // Slightly complex usage
        public Item Equip(Item item)
        {
            if(item.Attribute == AttributeEnum.Speed)
            {
                Speed += item.Value;
                Range += item.Range;
                Damage += item.Damage;
            }
            else if (item.Attribute == AttributeEnum.Defense)
            {
                Defense += item.Value;
                Range += item.Range;
                Damage += item.Damage;
            }
            else if (item.Attribute == AttributeEnum.Attack)
            {
                Attack += item.Value;
                Range += item.Range;
                Damage += item.Damage;
            }
            else if (item.Attribute == AttributeEnum.CurrentHealth)
            {
                HealthCurr += item.Value;
                if (HealthCurr > Health)
                {
                    HealthCurr = Health;
                }
                Range += item.Range;
                Damage += item.Damage;
            }

            // Just in case the max health is attribute
            else if (item.Attribute == AttributeEnum.MaxHealth)
            {
                Health += item.Value;
                HealthCurr += item.Value;
                Range += item.Range;
                Damage += item.Damage;
            }
            return item;
        }

        // UnEquip in the sense that the item's stats are removed from the character but not necessarily removed from equipped location
        // Sligtly complex usage.
        public Item UnEquip(Item item)
        {
            if (item.Attribute == AttributeEnum.Speed)
            {
                Speed -= item.Value;
                Range -= item.Range;
                Damage -= item.Damage;
            }
            else if (item.Attribute == AttributeEnum.Defense)
            {
                Defense -= item.Value;
                Range -= item.Range;
                Damage -= item.Damage;
            }
            else if (item.Attribute == AttributeEnum.Attack)
            {
                Attack -= item.Value;
                Range -= item.Range;
                Damage -= item.Damage;
            }
            else if (item.Attribute == AttributeEnum.CurrentHealth)
            {
                HealthCurr -= item.Value;
                if (HealthCurr < 0)
                {
                    HealthCurr = 1;
                }
                Range -= item.Range;
                Damage -= item.Damage;
            }

            // Just in case the attribute is max health
            else if (item.Attribute == AttributeEnum.MaxHealth)
            {
                Health -= item.Value;
                HealthCurr -= item.Value;
                if(HealthCurr < 0)
                {
                    HealthCurr = 1;
                }
                Range -= item.Range;
                Damage -= item.Damage;
            }
            return item;
        }

        // Convert this character's stats into JSON object usable string
        public string ConvertToString()
        {
            var dict = new Dictionary<string, string>
            {
                {"Name", Name.ToString()},
                {"Description", Description.ToString()},
                {"Image", Image.ToString()},
                {"Level",Level.ToString()},
                {"Experience", XP.ToString()},
                {"Health", Health.ToString()},
                {"Damage", Damage.ToString()},
                {"Attack", Attack.ToString()},
                {"Defense", Defense.ToString()},
                {"Speed", Speed.ToString()},
                {"Range", Range.ToString()},
                {"Move", Move.ToString()},
                {"Head", Head.FormatOutput()},
                {"Necklass", Necklass.FormatOutput()},
                {"PrimaryHand", PrimaryHand.FormatOutput()},
                {"OffHand", OffHand.FormatOutput()},
                {"LeftFinger", LeftFinger.FormatOutput()},
                {"RightFinger", RightFinger.FormatOutput()},
                {"Feet", Feet.FormatOutput()}
            };

            // Convert parameters to a key value pairs to a json object
            JObject finalContentJson = (JObject)JToken.FromObject(dict);
            return finalContentJson.ToString();
        }

        // Add experience to the character and level up if appropriate.
        public void AddExperience(int xp)
        {
            XP += xp;
            bool continueLeveling = true;

            // Continues when the character levels up as a result
            while (continueLeveling)
            {
                continueLeveling = false;

                // Level up accepted and computed
                // Removes he old level stat boost and adds the new level stat boost
                if (Level < 20 && XP > LevelUpTable[Level, 0])
                {
                    Level++;
                    Random rng = new Random();
                    int newHealth = rng.Next(10);
                    HealthCurr += newHealth;
                    Health += newHealth;
                    Attack -= LevelUpTable[Level-1, 1];
                    Defense -= LevelUpTable[Level-1, 2];
                    Speed -= LevelUpTable[Level-1, 3];
                    Attack += LevelUpTable[Level, 1];
                    Defense += LevelUpTable[Level, 2];
                    Speed += LevelUpTable[Level, 3];
                    continueLeveling = true;
                }
            }
        }

        // Set the level of the Character from Zero as well as set current XP
        public void SetLevel(int level)
        {
            // Ensure proper level value for update. 
            // Cannot set level lower than current level.
            if (level <= 1 || level <= Level)
            {
                return;
            }

            // Max level for all characters is 20
            if (level > 20)
            {
                level = 20;
            }
            Level = level;

            // Stats update per level up.
            // Redundant yes but also easy to program. 
            // With O(20) computation duration, the speed decrease is ignorable. 
            // Only called at the start of a game anyway and a max of 6 times. 
            // Okay im sorry.... i know it sucks
            for (int i = 1; i < level; i++)
            {
                Random rng = new Random();
                int newHealth = rng.Next(10);
                Health += newHealth;
                HealthCurr += newHealth;
                Attack -= LevelUpTable[i-1, 1];
                Defense -= LevelUpTable[i-1, 2];
                Speed -= LevelUpTable[i-1, 3];
                Attack += LevelUpTable[i, 1];
                Defense += LevelUpTable[i, 2];
                Speed += LevelUpTable[i, 3];
            }
            XP = LevelUpTable[level, 0];
        }
    }
}