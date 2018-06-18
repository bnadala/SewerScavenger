using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        private ItemsViewModel _viewModel;

        //Default Constructor
        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = ItemsViewModel.Instance;
        }

        //Handle selecting an item
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Item;
            if (data == null)
                return;

            //Open a new ItemDetailPage using ItemDetailViewModel
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(data)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        //Handle add button clicked
        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            //Open a new NewItemPage
            await Navigation.PushAsync(new NewItemPage());
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