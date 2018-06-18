using System;
using SQLite;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SewerScavenger.Models
{
    public class Monster : Entity<Item>
    {
        // Basic Monster stats
        public int Level { get; set; }          // Level of the Monster
        public int Health { get; set; }         // Maximum health of the Monster
        public int Damage { get; set; }         // Damage of the Monster. Only influenced by items
        public int Attack { get; set; }         // Attack of the Monster to determine hit and damage as well
        public int Defense { get; set; }        // Defense of the Monster to determine a missed attack
        public int Speed { get; set; }          // Speed of the Monster to determine the turnlist position
        public int Move { get; set; }           // Moves the Monster can make. Cannnot be editted outside of Monster creation
        public int Range { get; set; }          // Range of the Monster's attack. Only influenced by items

        // Experience given, Attack, Defense, Speed up to 20 levels
        // On levels greater than 20 the last attribute is added continuously to ensure that monsters are still getting stronger.
        // This table is similar to the character's level up table except that the defense is halved to prevent the fight to be all misses on both sides. 
        public int[,] LevelUpTable = new int[,]
        {
            { 0, 0, 0, 0 }, { 300, 1, 1, 1 }, { 900, 2, 1, 1 },
            { 2700, 2, 1, 1}, { 6500, 2, 2, 2}, { 14000, 3, 2, 2},
            { 23000, 3, 2, 2}, { 34000, 3, 2, 2}, { 48000, 3, 2, 2},
            { 64000, 4, 3, 3}, { 85000, 4, 3, 3}, { 100000, 4, 3, 3},
            { 120000, 4, 3, 3}, { 140000, 5, 3, 3}, { 165000, 5, 3, 4},
            { 195000, 5, 4, 4}, { 225000, 5, 4, 4}, { 265000, 6, 4, 4},
            { 305000, 7, 4, 4}, { 355000, 8, 5, 5}
        };
        
        // Battle System usable stats, Experience given and current health
        public int ExpToGive { get; set; }      // Experience given when monster is dealt enough damage to die.
        public int HealthCurr { get; set; }     // Current health

        // Monster constructor for basic empty stats
        public Monster()
        {
            Name = "";
            Description = "";
            Image = "";
            Health = 1;
            HealthCurr = 1;
            Attack = 1;
            Defense = 1;
            Speed = 1;
            Move = 1;
            Range = 1;
            ExpToGive = 0;
            Level = 1;
        }

        // Constructor that is a new instance of the passed monster
        public Monster(Monster newData)
        {
            if (newData == null)
            {
                return;
            }
            
            Name = newData.Name;
            Description = newData.Description;
            Image = newData.Image;
            Health = newData.Health;
            HealthCurr = Health;
            Attack = newData.Attack;
            Defense = newData.Defense;
            Speed = newData.Speed;
            Move = newData.Move;
            Range = newData.Range;
            ExpToGive = newData.ExpToGive;
            Level = newData.Level;
        }

        // Update the stats of this monster with the stats of the passed monster
        public void Update(Monster newData)
        {
            if (newData == null)
            {
                return;
            }

            // Update all the fields in the Data, except for the Id
            Name = newData.Name;
            Description = newData.Description;
            Image = newData.Image;
            Health = newData.Health;
            Attack = newData.Attack;
            Defense = newData.Defense;
            Speed = newData.Speed;
            Move = newData.Move;
            Range = newData.Range;
            ExpToGive = newData.ExpToGive;
            Level = newData.Level;
        }

        // Convert this monster into JSON usable string format.
        public string ConvertToString()
        {
            var dict = new Dictionary<string, string>
            {
                {"Name", Name.ToString()},
                {"Description", Description.ToString()},
                {"Image", Image.ToString()},
                {"Level",Level.ToString()},
                {"Health", Health.ToString()},
                {"Attack", Attack.ToString()},
                {"Defense", Defense.ToString()},
                {"Speed", Speed.ToString()},
                {"Range", Range.ToString()},
                {"Move", Move.ToString()}
            };

            // Convert parameters to a key value pairs to a json object
            JObject finalContentJson = (JObject)JToken.FromObject(dict);
            return finalContentJson.ToString();
        }

        // Set the level of the Monster from Zero as well as ExpToGive
        public void SetLevel(int level)
        {
            // level request 0 and negatives get turned to 1 instead.
            if(level <= 0)
            {
                level = 1;
            }

            // For each level the monsters are granted randomly increased health
            // Attack, Defense, and Speed are based on the level up tables
            for (int i = 0; i < level; i++)
            {
                // Random health increase from 1 to 10
                Random rng = new Random();
                int newHealth = 1+rng.Next(10);
                Health += newHealth;
                HealthCurr += newHealth;

                // If the level is less than 20 then add the level up stats from the table.
                if (level < 20)
                {
                    Attack += LevelUpTable[level, 1];
                    Defense += LevelUpTable[level, 2];
                    Speed += LevelUpTable[level, 3];
                }

                // If the level is greater than 19 then add the last stats on the table. 
                // Ensures monsters will always be stronger than characters at 20 + items at scaling levels. 
                else
                {
                    Attack += LevelUpTable[19, 1];
                    Defense += LevelUpTable[19, 2];
                    Speed += LevelUpTable[19, 3];
                }
            }

            // If the level is greater than 19, the experience based on the 
            // last experience given + last experience given * half for every level about 19
            // Ensures that monsters will keep giving more and more experience for the score
            // Even if the characters are all at max level 
            if (level >= 20)
            {
                ExpToGive = LevelUpTable[19, 0];

                double extraExp = 0.5 * (level - 19);
                ExpToGive += (int)(LevelUpTable[19, 0] * extraExp);
            }

            // If the level is less than 20 then the experience in the level up table is used. 
            else
            {
                ExpToGive = LevelUpTable[level, 0];
            }
            Level = level;
        }
    }
}