using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CharactersPage : ContentPage
    {
        private CharactersViewModel _viewModel;

        //Default Constructor
        public CharactersPage()
        {
            InitializeComponent();

            //Establish the Binding Context
            BindingContext = _viewModel = CharactersViewModel.Instance;
        }

        //Handle selecting a character
        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Character;
            if (data == null)
            {
                return;
            }

            //Open a new CharacterDetailPage using CharacterDetailViewModel
            await Navigation.PushAsync(new CharacterDetailPage(new CharacterDetailViewModel(data)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        //Handle add button clicked
        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            //Open a new NewCharacterPage
            await Navigation.PushAsync(new NewCharacterPage());
        }

        //Handle page appearing
        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = null;

            if (ToolbarItems.Count > 0)
            {
                ToolbarItems.RemoveAt(0);
            }

            InitializeComponent();

            if (_viewModel.Dataset.Count == 0)
            {
                _viewModel.LoadDataCommand.Execute(null);
            }
            else if (_viewModel.NeedsRefresh())
            {
                _viewModel.LoadDataCommand.Execute(null);
            }
            
            BindingContext = _viewModel;
        }
    }
}