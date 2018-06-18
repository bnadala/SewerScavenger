using System;

using SewerScavenger.Models;
using SewerScavenger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditMonsterPage : ContentPage
    {
        
        private MonstersDetailViewModel _viewModel;

        public Monster Data { get; set; }

        // The constructor takes a View Model
        public EditMonsterPage(MonstersDetailViewModel viewModel)
        {
            // Save off the item
            Data = viewModel.Data;
            viewModel.Title = "Edit " + viewModel.Title;

            InitializeComponent();


            // Set the data binding for the page
            //Establish Binding Context
            BindingContext = _viewModel = viewModel;
        }

        //Handle save clicked
        private async void Save_Clicked(object sender, EventArgs e)
        {
            //Send the edit message to the controller
            MessagingCenter.Send(this, "EditData", Data);

            // removing the old MonsterDetailsPage, 2 up counting this page
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);

            // Add a new MonsterDetailsPage, with the new Monster data on it
            await Navigation.PushAsync(new MonsterDetailPage(new MonstersDetailViewModel(Data)));

            // Last, remove this page
            Navigation.RemovePage(this);
        }

        //Handle cancel clicked
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            //Go back to the last page
            await Navigation.PopAsync();
        }
        
        // The stepper function for Health
        void Health_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            HealthValue.Text = String.Format("{0}", e.NewValue);
        }
        // The stepper function for Attack
        void Attack_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            AttackValue.Text = String.Format("{0}", e.NewValue);
        }
        // The stepper function for Defense
        void Defense_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            DefenseValue.Text = String.Format("{0}", e.NewValue);
        }
        // The stepper function for Speed
        void Speed_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            SpeedValue.Text = String.Format("{0}", e.NewValue);
        }
        // The stepper function for Move
        void Move_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            MoveValue.Text = String.Format("{0}", e.NewValue);
        }
        // The stepper function for Range
        void Range_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            RangeValue.Text = String.Format("{0}", e.NewValue);
        }
        // The stepper function for Damage
        void Damage_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            DamageValue.Text = String.Format("{0}", e.NewValue);
        }

    }
}