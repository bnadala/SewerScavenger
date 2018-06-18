using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        private ItemDetailViewModel _viewModel;

        // The constructor takes a View Model
        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            //Establish Binding Context
            BindingContext = _viewModel = viewModel;
        }

        //Default Constructor
        public ItemDetailPage()
        {
            InitializeComponent();
            //Define a new item
            var data = new Item();

            //Establish binding context
            _viewModel = new ItemDetailViewModel(data);
            BindingContext = _viewModel;
        }

        //Handle edit clicked
        private async void Edit_Clicked(object sender, EventArgs e)
        {
            //Open a new EditItemPage
            await Navigation.PushAsync(new EditItemPage(_viewModel));
        }

        //Handle delete clicked
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            //Open a new DeleteItemPage
            await Navigation.PushAsync(new DeleteItemPage(_viewModel));
        }

        //Handle cancel clicked
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            //Go to the last page
            await Navigation.PopAsync();
        }
    }
}