using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SewerScavenger.ViewModels;

namespace SewerScavenger.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TutorialPage : ContentPage
	{
        // Holds the current image on the page
        private TutorialViewModel viewModel;

        // Constructor for the page. Attaches the binding
		public TutorialPage ()
		{
            viewModel = new TutorialViewModel();

			InitializeComponent ();

            BindingContext = viewModel;
		}

        // Game Return button was clicked, go back to the GamePage
        async void GameButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        // Back button clicked, loads previous image of the tutorial
        public void BackButtonClicked(object sender, EventArgs e)
        {
            viewModel.Back();
            BindingContext = null;
            BindingContext = viewModel;
        }

        // Next button clicked, loads next image of the tutorial
        public void NextButtonClicked(object sender, EventArgs e)
        {
            viewModel.Next();
            BindingContext = null;
            BindingContext = viewModel;
        }
    }
}