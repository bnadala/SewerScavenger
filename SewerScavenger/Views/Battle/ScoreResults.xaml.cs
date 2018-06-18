using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScoreResults : ContentPage
    {
        // Holds the score info
        private ScoreDetailViewModel _viewModel;

        // Fancy version of the score display after a battle
        public ScoreResults(ScoreDetailViewModel viewModel)
        {
            InitializeComponent();

            // Bind the data for the score, characters at death, monsters killed, and items dropped
            BindingContext = _viewModel = viewModel;
            CharacterList.ItemsSource = _viewModel.Data.CharacterList;
            MonsterList.ItemsSource = _viewModel.Data.MonsterList;
            ItemList.ItemsSource = _viewModel.Data.ItemList;
            ScoreList.ItemsSource = _viewModel.Data.BattleText;
        }

        // If the new game button is clicked just pop the page back.
        // It should go back to Pick Character if on Auto battle
        // It should continue poping till it reached the Pick Character page if on normal battle
        private async void New_Game_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        // If a character is selected, open a details page
        private void CharacterDetails(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Character data)
            {
                Navigation.PushModalAsync(new CharacterDetails(new CharacterDetailViewModel(data)));
            }
        }

        // If a monster is selected, open a details page
        private void MonsterDetails(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Monster data)
            {
                Navigation.PushModalAsync(new MonsterDetails(new MonstersDetailViewModel(data)));
            }
        }

        // If an item is selected, open a details page.
        private void ItemDetails(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Item data)
            {
                Navigation.PushModalAsync(new InventoryDetailsPage(new ItemDetailViewModel(data)));
            }
        }
    }
}