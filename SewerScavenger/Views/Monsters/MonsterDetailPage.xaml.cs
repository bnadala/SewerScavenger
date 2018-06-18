using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonsterDetailPage : ContentPage
    {
        private MonstersDetailViewModel _viewModel;

        // The constructor takes a View Model
        public MonsterDetailPage(MonstersDetailViewModel viewModel)
        {
            InitializeComponent();

            //Establish Binding Context
            BindingContext = _viewModel = viewModel;
        }

        //Default constructor
        public MonsterDetailPage()
        {
            InitializeComponent();
            //Define a new Monster
            var data = new Monster
            {
                Name = "Item 1",
                Description = "This is an item description."
            };

            //Use a MonstersDetailViewModel
            _viewModel = new MonstersDetailViewModel(data);
            //Establish Binding Context
            BindingContext = _viewModel;
        }

        //Handle edit clicked
        private async void Edit_Clicked(object sender, EventArgs e)
        {
            //Open a new EditMonsterPage
            await Navigation.PushAsync(new EditMonsterPage(_viewModel));
        }

        //Handle delete clicked
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            //Open a new DeleteMonsterPage
            await Navigation.PushAsync(new DeleteMonsterPage(_viewModel));
        }

        //Handle cancel clicked
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            //Go to the last page
            await Navigation.PopAsync();
        }
    }
}