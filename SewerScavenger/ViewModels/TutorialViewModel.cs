using System.Collections.Generic;

namespace SewerScavenger.ViewModels
{
    public class TutorialViewModel
    {
        // Image to be displayed during the tutorial
        public string Image { get; set; }

        // Contains the list of images for the tutorial
        private List<string> ImageList;

        // Counter to display the next image
        private int next = 0;

        // Generates the list of images to show during the tutorial. 
        public TutorialViewModel()
        {
            // Update the list of images
            ImageList = new List<string>
            {
                "PickCharacter.png",
                "BattleEnemy1.png",
                "BattleMove.png",
                "BattleAttack.png",
                "BattleEnemy2.png",
                "BattleEquip.png",
                "EquipItems.png",
                "ScorePage.png"
            };
            Image = ImageList[next];
        }

        // Display next image in the list or the last image on the list.
        public void Next()
        {
            next++;
            if(next >= ImageList.Count)
            {
                next--;
            }
            Image = ImageList[next];
        }

        // Display first image on the list or the first imae on the list.
        public void Back()
        {
            next--;
            if (next < 0)
            {
                next++;
            }
            Image = ImageList[next];
        }
    }
}
