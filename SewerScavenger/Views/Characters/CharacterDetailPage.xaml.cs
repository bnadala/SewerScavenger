using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable RedundantExtendsListEntry

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CharacterDetailPage : ContentPage
    {
        private CharacterDetailViewModel _viewModel;

        // The constructor takes a View Model
        public CharacterDetailPage(CharacterDetailViewModel viewModel)
        {
            InitializeComponent();
            //Establish Binding Context
            BindingContext = _viewModel = viewModel;
        }

        //Default Constructor
        public CharacterDetailPage()
        {
            InitializeComponent();
            //define a new character
            var data = new Character
            {
                Name = "Item 1",
                Description = "This is an item description.",
                Level = 1
            };
            //Use a CharacterDetailViewModel
            _viewModel = new CharacterDetailViewModel(data);
            //Establish Binding Context
            BindingContext = _viewModel;
        }

        //Handle edit clicked
        public async void Edit_Clicked(object sender, EventArgs e)
        {
            //Open a new EditCharacterPage
            await Navigation.PushAsync(new EditCharacterPage(_viewModel));
        }
        //Handle delete clicked
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            //Open a new DeleteCharacterPage
            await Navigation.PushAsync(new DeleteCharacterPage(_viewModel));
        }
        //Handle cancel clicked
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            //Go to the last page
            await Navigation.PopAsync();
        }
    }
}