using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CharacterEquipPage : ContentPage
	{
        public List<Item> EquippedList = new List<Item>();  // All items currently equipped. Sorted in order
        public List<Item> HeadList = new List<Item>();      // All head items the character can equip
        public List<Item> NeckList = new List<Item>();      // All neck items the character can equip
        public List<Item> FeetList = new List<Item>();      // All feet items the character can equip
        public List<Item> PrimaryList = new List<Item>();   // All primary hand items the character can equip
        public List<Item> OffList = new List<Item>();       // All off hand items the character can equip
        public List<Item> LeftList = new List<Item>();      // All left finger items the character can equip
        public List<Item> RightList = new List<Item>();     // All right items the character can equip
        public List<Item> InventoryList = new List<Item>(); // All items the character may equip. Acts as the return so all edits to each location happen here as well.
        
        // Display of the character's stats
        private CharacterDetailViewModel viewModel;

        // Sets all the stats, equipped items, and equippable items
        public CharacterEquipPage(CharacterDetailViewModel viewmodel, List<Item> itemsList)
        {
            InitializeComponent();

            // Display the stats of the character
            BindingContext = viewModel = viewmodel;

            // Hold the list for sorting
            InventoryList = itemsList;

            // Sorts all items based on location
            foreach (Item item in itemsList)
            {
                PlaceItem(item);
            }

            // Setup to display all items currently equipped
            EquippedList.Add(viewModel.Data.Head);
            EquippedList.Add(viewModel.Data.Necklass);
            EquippedList.Add(viewModel.Data.Feet);
            EquippedList.Add(viewModel.Data.PrimaryHand);
            EquippedList.Add(viewModel.Data.OffHand);
            EquippedList.Add(viewModel.Data.LeftFinger);
            EquippedList.Add(viewModel.Data.RightFinger);

            // Setup to display all items to be equipped
            InventoryListView.ItemsSource = EquippedList;
            InventoryListView0.ItemsSource = HeadList;
            InventoryListView1.ItemsSource = NeckList;
            InventoryListView2.ItemsSource = FeetList;
            InventoryListView3.ItemsSource = PrimaryList;
            InventoryListView4.ItemsSource = OffList;
            InventoryListView5.ItemsSource = LeftList;
            InventoryListView6.ItemsSource = RightList;
        }
        
        // When the user wants to unequip an item
        private void ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            // Grab the item
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            // Ensure it is not an empty item
            if (data.Location != ItemLocationEnum.Unknown)
            {
                // Place the item into the appropriate slot
                PlaceItem(data);

                // Unequip the stats of the item
                viewModel.Data.UnEquip(data);

                // Add to the head list
                if (data.Location == ItemLocationEnum.Head)
                {
                    EquippedList[0] = new Item();
                    viewModel.Data.Head = new Item();
                    InventoryListView0.ItemsSource = null;
                    InventoryListView0.ItemsSource = HeadList;
                }

                // Add to the neck list
                else if (data.Location == ItemLocationEnum.Necklass)
                {
                    EquippedList[1] = new Item();
                    viewModel.Data.Necklass = new Item();
                    InventoryListView1.ItemsSource = null;
                    InventoryListView1.ItemsSource = NeckList;
                }

                // Add to the feet list
                else if (data.Location == ItemLocationEnum.Feet)
                {
                    EquippedList[2] = new Item();
                    viewModel.Data.Head = new Item();
                    InventoryListView2.ItemsSource = null;
                    InventoryListView2.ItemsSource = FeetList;
                }

                // Add to the primary hand list
                else if (data.Location == ItemLocationEnum.PrimaryHand)
                {
                    EquippedList[3] = new Item();
                    viewModel.Data.PrimaryHand = new Item();
                    InventoryListView3.ItemsSource = null;
                    InventoryListView3.ItemsSource = PrimaryList;
                }

                // Add to the off hand list
                else if (data.Location == ItemLocationEnum.OffHand)
                {
                    EquippedList[4] = new Item();
                    viewModel.Data.OffHand = new Item();
                    InventoryListView4.ItemsSource = null;
                    InventoryListView4.ItemsSource = OffList;
                }

                // Add to both finger lists
                else if (data.Location == ItemLocationEnum.Finger)
                {
                    // If the source was from the left finger, take it off
                    if(data.Guid == EquippedList[5].Guid)
                    {
                        EquippedList[5] = new Item();
                        viewModel.Data.LeftFinger = new Item();
                    }

                    // If the source was from the right finger, take it off
                    else
                    {
                        EquippedList[6] = new Item();
                        viewModel.Data.RightFinger = new Item();
                    }

                    // Update both lists because the place item call would have added it to both
                    InventoryListView5.ItemsSource = null;
                    InventoryListView5.ItemsSource = LeftList;
                    InventoryListView6.ItemsSource = null;
                    InventoryListView6.ItemsSource = RightList;
                }

                // Add back into the equip pool
                InventoryList.Add(data);
            }

            // Update the displays
            InventoryListView.ItemsSource = null;
            InventoryListView.ItemsSource = EquippedList;
            BindingContext = null;
            BindingContext = viewModel;

            // Manually deselect item.
            InventoryListView.SelectedItem = null;
        }

        // Place the item into the appropriatelist based on location
        private void PlaceItem(Item item)
        {
            if (item.Location == ItemLocationEnum.Head)
            {
                HeadList.Add(item);
            }
            else if (item.Location == ItemLocationEnum.Necklass)
            {
                NeckList.Add(item);
            }
            else if (item.Location == ItemLocationEnum.PrimaryHand)
            {
                PrimaryList.Add(item);
            }
            else if (item.Location == ItemLocationEnum.OffHand)
            {
                OffList.Add(item);
            }

            // A finger item belongs to both lists because he user can equip it to either finger
            else if (item.Location == ItemLocationEnum.Finger)
            {
                LeftList.Add(item);
                RightList.Add(item);
            }
            else if (item.Location == ItemLocationEnum.Feet)
            {
                FeetList.Add(item);
            }
        }

        // If a head item was selected for equipping
        private void ItemSelected0(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            int found1 = -1;
            // Find item in the head list
            for (int i = 0; i < HeadList.Count && found1 == -1; i++)
            {
                if (HeadList[i].Id == data.Id)
                {
                    int found2 = -1;
                    // Find item in the inventory list
                    for (int j = 0; j < InventoryList.Count && found2 == -1; j++)
                    {
                        if (InventoryList[j].Id == data.Id)
                        {
                            found2 = j;
                        }
                    }

                    // Swap the equipped item
                    if (viewModel.Data.Head.Name != "Unknown")
                    {
                        Item temp = viewModel.Data.UnEquip(viewModel.Data.Head);
                        EquippedList[0] = new Item();
                        HeadList[i] = temp;
                        InventoryList[found2] = temp;
                    }

                    // Remove from the lists
                    else
                    {
                        HeadList.RemoveAt(i);
                        InventoryList.RemoveAt(found2);
                    }
                    viewModel.Data.Head = data;
                    viewModel.Data.Equip(data);
                    EquippedList[0] = data;
                    found1 = i;
                }
            }

            // Update the display
            InventoryListView.ItemsSource = null;
            InventoryListView.ItemsSource = EquippedList;
            InventoryListView0.ItemsSource = null;
            InventoryListView0.ItemsSource = HeadList;
            BindingContext = null;
            BindingContext = viewModel;
            // Manually deselect item.
            InventoryListView0.SelectedItem = null;
        }

        // If a neck item was selected for equipping
        private void ItemSelected1(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            int found1 = -1;
            // Find item in the head list
            for (int i=0; i < NeckList.Count && found1==-1; i++)
            {
                if (NeckList[i].Id == data.Id)
                {
                    int found2 = -1;
                    // Find item in the inventory list
                    for (int j = 0; j < InventoryList.Count && found2==-1; j++)
                    {
                        if (InventoryList[j].Id == data.Id)
                        {
                            found2 = j;
                        }
                    }

                    // Swap the equipped item
                    if (viewModel.Data.Necklass.Name != "Unknown")
                    {
                        Item temp = viewModel.Data.UnEquip(viewModel.Data.Necklass);
                        NeckList[i] = temp;
                        InventoryList[found2] = temp;
                    }

                    // Remove from the lists
                    else
                    {
                        NeckList.RemoveAt(i);
                        InventoryList.RemoveAt(found2);
                    }
                    viewModel.Data.Necklass = data;
                    viewModel.Data.Equip(data);
                    EquippedList[1] = data;
                    found1 = i;
                }
            }

            // Update the display
            InventoryListView.ItemsSource = null;
            InventoryListView.ItemsSource = EquippedList;
            InventoryListView1.ItemsSource = null;
            InventoryListView1.ItemsSource = NeckList;
            BindingContext = null;
            BindingContext = viewModel;

            // Manually deselect item.
            InventoryListView1.SelectedItem = null;
        }

        // If a Feet item was selected for equipping
        private void ItemSelected2(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            int found1 = -1;
            // Find item in the head list
            for (int i = 0; i < FeetList.Count && found1 == -1; i++)
            {
                if (FeetList[i].Id == data.Id)
                {
                    int found2 = -1;
                    // Find item in the inventory list
                    for (int j = 0; j < InventoryList.Count && found2 == -1; j++)
                    {
                        if (InventoryList[j].Id == data.Id)
                        {
                            found2 = j;
                        }
                    }

                    // Swap the equipped item
                    if (viewModel.Data.Feet.Name != "Unknown")
                    {
                        Item temp = viewModel.Data.UnEquip(viewModel.Data.Feet);
                        FeetList[i] = temp;
                        InventoryList[found2] = temp;
                    }

                    // Remove from the lists
                    else
                    {
                        FeetList.RemoveAt(i);
                        InventoryList.RemoveAt(found2);
                    }
                    viewModel.Data.Feet = data;
                    viewModel.Data.Equip(data);
                    EquippedList[2] = data;
                    found1 = i;
                }
            }

            // Update the display
            InventoryListView.ItemsSource = null;
            InventoryListView.ItemsSource = EquippedList;
            InventoryListView2.ItemsSource = null;
            InventoryListView2.ItemsSource = FeetList;
            BindingContext = null;
            BindingContext = viewModel;

            // Manually deselect item.
            InventoryListView2.SelectedItem = null;
        }

        // If a Primary hand item was selected for equipping
        private void ItemSelected3(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            int found1 = -1;
            // Find item in the head list
            for (int i = 0; i < PrimaryList.Count && found1 == -1; i++)
            {
                if (PrimaryList[i].Id == data.Id)
                {
                    int found2 = -1;
                    // Find item in the inventory list
                    for (int j = 0; j < InventoryList.Count && found2 == -1; j++)
                    {
                        if (InventoryList[j].Id == data.Id)
                        {
                            found2 = j;
                        }
                    }

                    // Swap the equipped item
                    if (viewModel.Data.PrimaryHand.Name != "Unknown")
                    {
                        Item temp = viewModel.Data.UnEquip(viewModel.Data.PrimaryHand);
                        PrimaryList[i] = temp;
                        InventoryList[found2] = temp;
                    }

                    // Remove from the lists
                    else
                    {
                        PrimaryList.RemoveAt(i);
                        InventoryList.RemoveAt(found2);
                    }
                    viewModel.Data.PrimaryHand = data;
                    viewModel.Data.Equip(data);
                    EquippedList[3] = data;
                    found1 = i;
                }
            }

            // Update the display
            InventoryListView.ItemsSource = null;
            InventoryListView.ItemsSource = EquippedList;
            InventoryListView3.ItemsSource = null;
            InventoryListView3.ItemsSource = PrimaryList;
            BindingContext = null;
            BindingContext = viewModel;

            // Manually deselect item.
            InventoryListView3.SelectedItem = null;
        }

        // If a off hand item was selected for equipping
        private void ItemSelected4(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            int found1 = -1;
            // Find item in the head list
            for (int i = 0; i < OffList.Count && found1 == -1; i++)
            {
                if (OffList[i].Id == data.Id)
                {
                    int found2 = -1;
                    // Find item in the inventory list
                    for (int j = 0; j < InventoryList.Count && found2 == -1; j++)
                    {
                        if (InventoryList[j].Id == data.Id)
                        {
                            found2 = j;
                        }
                    }

                    // Swap the equipped item
                    if (viewModel.Data.OffHand.Name != "Unknown")
                    {
                        Item temp = viewModel.Data.UnEquip(viewModel.Data.OffHand);
                        OffList[i] = temp;
                        InventoryList[found2] = temp;
                    }

                    // Remove from the lists
                    else
                    {
                        OffList.RemoveAt(i);
                        InventoryList.RemoveAt(found2);
                    }
                    viewModel.Data.OffHand = data;
                    viewModel.Data.Equip(data);
                    EquippedList[4] = data;
                    found1 = i;
                }
            }

            // Update the display
            InventoryListView.ItemsSource = null;
            InventoryListView.ItemsSource = EquippedList;
            InventoryListView4.ItemsSource = null;
            InventoryListView4.ItemsSource = OffList;
            BindingContext = null;
            BindingContext = viewModel;

            // Manually deselect item.
            InventoryListView4.SelectedItem = null;
        }

        // If a left finger item was selected for equipping
        private void ItemSelected5(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            int found1 = -1;
            // Find item in the head list
            for (int i = 0; i < LeftList.Count && found1 == -1; i++)
            {
                if (LeftList[i].Id == data.Id)
                {
                    int found2 = -1;
                    // Find item in the inventory list
                    for (int j = 0; j < InventoryList.Count && found2 == -1; j++)
                    {
                        if (InventoryList[j].Id == data.Id)
                        {
                            found2 = j;
                        }
                    }

                    // Swap the equipped item
                    if (viewModel.Data.LeftFinger.Name != "Unknown")
                    {
                        Item temp = viewModel.Data.UnEquip(viewModel.Data.LeftFinger);
                        LeftList[i] = temp;
                        RightList[i] = temp;
                        InventoryList[found2] = temp;
                    }

                    // Remove from the lists
                    else
                    {
                        LeftList.RemoveAt(i);
                        RightList.RemoveAt(i);
                        InventoryList.RemoveAt(found2);
                    }
                    viewModel.Data.LeftFinger = data;
                    viewModel.Data.Equip(data);
                    EquippedList[5] = data;
                    found1 = i;
                }
            }

            // Update the display
            InventoryListView.ItemsSource = null;
            InventoryListView.ItemsSource = EquippedList;
            InventoryListView5.ItemsSource = null;
            InventoryListView5.ItemsSource = LeftList;
            InventoryListView6.ItemsSource = null;
            InventoryListView6.ItemsSource = RightList;
            BindingContext = null;
            BindingContext = viewModel;

            // Manually deselect item.
            InventoryListView5.SelectedItem = null;
        }

        // If a right item was selected for equipping
        private void ItemSelected6(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            int found1 = -1;
            // Find item in the head list
            for (int i = 0; i < RightList.Count && found1 == -1; i++)
            {
                if (RightList[i].Id == data.Id)
                {
                    int found2 = -1;
                    // Find item in the inventory list
                    for (int j = 0; j < InventoryList.Count && found2 == -1; j++)
                    {
                        if (InventoryList[j].Id == data.Id)
                        {
                            found2 = j;
                        }
                    }

                    // Swap the equipped item
                    if (viewModel.Data.RightFinger.Name != "Unknown")
                    {
                        Item temp = viewModel.Data.UnEquip(viewModel.Data.RightFinger);
                        RightList[i] = temp;
                        LeftList[i] = temp;
                        InventoryList[found2] = temp;
                    }

                    // Remove from the lists
                    else
                    {
                        RightList.RemoveAt(i);
                        LeftList.RemoveAt(i);
                        InventoryList.RemoveAt(found2);
                    }
                    viewModel.Data.RightFinger = data;
                    viewModel.Data.Equip(data);
                    EquippedList[6] = data;
                    found1 = i;
                }
            }

            // Update the display
            InventoryListView.ItemsSource = null;
            InventoryListView.ItemsSource = EquippedList;
            InventoryListView6.ItemsSource = null;
            InventoryListView6.ItemsSource = RightList;
            InventoryListView5.ItemsSource = null;
            InventoryListView5.ItemsSource = LeftList;
            BindingContext = null;
            BindingContext = viewModel;

            // Manually deselect item.
            InventoryListView6.SelectedItem = null;
        }

        // Go back a page in the navigation stack
        private async void Go_Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}