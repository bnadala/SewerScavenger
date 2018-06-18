using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CharacterDetails : ContentPage
	{
        // List of Equipped items. Attached to the character
        public List<Item> EquippedList = new List<Item>();

        // Used to display the character's stats and equipped items from the Score page right after the battle
        public CharacterDetails (CharacterDetailViewModel viewmodel)
		{
            InitializeComponent();

            BindingContext = viewmodel;

            // Collect all the equipped items
            EquippedList.Add(viewmodel.Data.Head);
            EquippedList.Add(viewmodel.Data.Necklass);
            EquippedList.Add(viewmodel.Data.Feet);
            EquippedList.Add(viewmodel.Data.PrimaryHand);
            EquippedList.Add(viewmodel.Data.OffHand);
            EquippedList.Add(viewmodel.Data.LeftFinger);
            EquippedList.Add(viewmodel.Data.RightFinger);

            // Display after collection
            InventoryListView.ItemsSource = EquippedList;
        }
	}
}