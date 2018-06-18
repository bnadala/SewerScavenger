using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MonsterDetails : ContentPage
	{
        // Load details of the monster
        public MonsterDetails(MonstersDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }

        // Go back to the score page
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}