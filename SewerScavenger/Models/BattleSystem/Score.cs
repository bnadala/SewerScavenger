using System;
using System.Collections.Generic;

namespace SewerScavenger.Models
{
    // String wrapper to be able to display strings in a listview
    public class StringWrapper : Entity<Item>
    {
        public string Text { get; set; }
        public StringWrapper(string s)
        {
            Text = s;
        }
    }

    public class Score : Entity<Item>
    {
        // Based on the total experience gained. 
        public int ScoreTotal { get; set; }

        // The Date the game played, and when the score was saved
        public DateTime GameDate { get; set; }

        // Tracks if auto battle is true, or if user battle = false
        public bool AutoBattle { get; set; }

        // The number of turns the battle took to finish
        public int TurnNumber { get; set; }

        // The count of monsters slain during battle
        public int MonsterSlainNumber { get; set; }

        // The total experience points all the characters received during the battle
        public int ExperienceGainedTotal { get; set; }

        // A list of all the characters at the time of death and their stats.  Needs to be in json format, so saving a string
        public string CharacterAtDeathList { get; set; }

        // All of the monsters killed and their stats. Needs to be in json format, so saving as a string
        public string MonstersKilledList { get; set; }

        // All of the items dropped and their stats. Needs to be in json format, so saving as a string
        public string ItemsDroppedList { get; set; }
        
        // List to display more details regarding the character at death, monsters killed, items dropped and battle text if debug enabled. 
        public List<Character> CharacterList = new List<Character>();
        public List<Monster> MonsterList = new List<Monster>();
        public List<Item> ItemList = new List<Item>();
        public List<StringWrapper> BattleText = new List<StringWrapper>();

        // New Score constructor
        public Score()
        {
            GameDate = DateTime.Now;    // Set to be now by default.
            AutoBattle = false;         //assume user battle
            ScoreTotal = 0;
            TurnNumber = 0;
            MonsterSlainNumber = 0;
            ExperienceGainedTotal = 0;
            CharacterAtDeathList = "";
            MonstersKilledList = "";
            ItemsDroppedList = "";
        }

        // Copy passed score stats into this score
        public void Update(Score newData)
        {
            if (newData == null)
            {
                return;
            }

            // Update all the fields in the Data, except for the Id
            Name = newData.Name;
            ScoreTotal = ScoreTotal;
        }
    }
}