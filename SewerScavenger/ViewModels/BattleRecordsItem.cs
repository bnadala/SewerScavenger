namespace SewerScavenger.ViewModels
{
    // Class used to display the batte text at the bottom of the BattlePage
    public class BattleRecordsItem
    {
        // Accumulated list
        public string Records { get; set; }

        public BattleRecordsItem()
        {
            Records = "";
        }

        public BattleRecordsItem(string s)
        {
            Records = s;
        }
    }
}
