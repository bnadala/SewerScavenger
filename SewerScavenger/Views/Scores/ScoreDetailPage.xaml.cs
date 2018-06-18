using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScoreDetailPage : ContentPage
    {
        private ScoreDetailViewModel _viewModel;

        public Score Data { get; set; }

        // The constructor takes a View Model
        public ScoreDetailPage(ScoreDetailViewModel viewModel)
        {
            InitializeComponent();

            Data = viewModel.Data;
            //Establish Binding Context
            BindingContext = _viewModel = viewModel;
        }

        //Handle delete clicked
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            //Display an alert
            var answer = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this score?", "Yes", "No");
            if (answer)
            {
                //Send DeleteData message to the controller
                MessagingCenter.Send(this, "DeleteData", Data);
                //Go back to the last page
                await Navigation.PopAsync();
            }
        }

        //Handle page appearing
        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = null;
            
            InitializeComponent();

            BindingContext = _viewModel;
        }
    }
}