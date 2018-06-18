using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SewerScavenger.Controllers;
using SewerScavenger.Models;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewItemPage : ContentPage
    {
        public Item Data { get; set; }

        // Constructor for the page, will create a new black item that can then get updated
        public NewItemPage()
        {
            InitializeComponent();

            //Define a new item
            Data = new Item
            {
                Name = "Item name",
                Description = "This is an item description.",
                Id = Guid.NewGuid().ToString(),
                Range=0,
                Value=1,
                ImageURI = ItemsController.DefaultImageURI
        };

            //Establish binding context
            BindingContext = this;
            //Need to make the SelectedItem a string, so it can select the correct item.
            LocationPicker.SelectedItem = Data.Location.ToString();
            AttributePicker.SelectedItem = Data.Attribute.ToString();
        }

        // Respond to the Save click
        // Send the add message to so it gets added...
        private async void Save_Clicked(object sender, EventArgs e)
        {
            // If the image in teh data box is empty, use the default one..
            if (string.IsNullOrEmpty(Data.ImageURI))
            {
                Data.ImageURI = ItemsController.DefaultImageURI;
            }

            //Send AddData to the controller
            MessagingCenter.Send(this, "AddData", Data);
            await Navigation.PopAsync();
        }

        // Cancel and go back a page in the navigation stack
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
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