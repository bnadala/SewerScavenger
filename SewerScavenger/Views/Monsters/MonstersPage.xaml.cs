using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonstersPage : ContentPage
    {
        
        private MonstersViewModel _viewModel;

        //Default Constructor
        public MonstersPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = MonstersViewModel.Instance;
        }

        //Handle selecting a monster
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var data = args.SelectedItem as Monster;
            if (data == null)
            {
                return;
            }

            //Open a new MonsterDetailPage using MonstersDetailViewModel
            await Navigation.PushAsync(new MonsterDetailPage(new MonstersDetailViewModel(data)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        //Handle add button clicked
        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            //Open a new NewMonsterPage
            await Navigation.PushAsync(new NewMonsterPage());
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