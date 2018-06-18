using System;
using SewerScavenger.Models;
using SewerScavenger.Controllers;
using SewerScavenger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SewerScavenger.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditItemPage : ContentPage
	{
	    private ItemDetailViewModel _viewModel;

        // The data returned from the edit.
        public Item Data { get; set; }

        // The constructor takes a View Model
        // It needs to set the Picker values after doing the bindings.
        public EditItemPage(ItemDetailViewModel viewModel)
        {
            // Save off the item
            Data = viewModel.Data;

            viewModel.Title = "Edit " + viewModel.Title;

            InitializeComponent();

            // Set the data binding for the page
            //Establish Binding Context
            BindingContext = _viewModel = viewModel;

            //Need to make the SelectedItem a string, so it can select the correct item.
            LocationPicker.SelectedItem = Data.Location.ToString();
            AttributePicker.SelectedItem = Data.Attribute.ToString();

        }

        //Handle save clicked
        private async void Save_Clicked(object sender, EventArgs e)
        {
            // If the image in teh data box is empty, use the default one..
            if (string.IsNullOrEmpty(Data.ImageURI))
            {
                Data.ImageURI = ItemsController.DefaultImageURI;
            }

            //Send the edit message to the controller
            MessagingCenter.Send(this, "EditData", Data);

            // removing the old ItemDetails page, 2 up counting this page
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);

            // Add a new items details page, with the new Item data on it
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(Data)));

            // Last, remove this page
            Navigation.RemovePage(this);
        }

        // Cancel and go back a page in the navigation stack
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            //Go back to the last page
            await Navigation.PopAsync();
        }

        // The stepper function for Range
        void Range_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            RangeValue.Text = String.Format("{0}", e.NewValue);
        }

        // The stepper function for Value
        void Value_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            ValueValue.Text = String.Format("{0}", e.NewValue);
        }
    }
}