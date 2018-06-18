using SewerScavenger.Models;
using SewerScavenger.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SewerScavenger.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeleteCharacterPage : ContentPage
	{
	    private CharacterDetailViewModel _viewModel;

        public Character Data { get; set; }

        // The constructor takes a View Model
        public DeleteCharacterPage (CharacterDetailViewModel viewModel)
        {
            // Save off the item
            Data = viewModel.Data;
            viewModel.Title = "Delete " + viewModel.Title;

            InitializeComponent();

            // Set the data binding for the page
            //Establish Binding Context
            BindingContext = _viewModel = viewModel;
        }

        //Handle delete clicked
	    private async void Delete_Clicked(object sender, EventArgs e)
        {
            //Send the delete message to the controller
            MessagingCenter.Send(this, "DeleteData", Data);

            // Remove Item Details Page manualy
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);

            //Go back to the previous page
            await Navigation.PopAsync();
        }

        //Handle cancel clicked
	    private async void Cancel_Clicked(object sender, EventArgs e)
        {
            //Do nothing and go back to the previous page
            await Navigation.PopAsync();
        }
    }
}