using System;
using System.Collections.Generic;
using System.Text;

namespace SewerScavenger.ViewModels
{
    class BattleRestViewModel : BaseViewModel
    {
        // Contains the battle count to be displayed on the Battle Rest page
        public string Count { get; set; }

        // Simple string constructor
        public BattleRestViewModel(string c = "Battle Count")
        {
            Count = c;
        }
    }
}
