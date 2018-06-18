using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SewerScavenger.Models;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewCharacterPage : ContentPage
    {
        public Character Data { get; set; }

        public NewCharacterPage()
        {
            InitializeComponent();

            //define a new character
            Data = new Character
            {
                Name = "Character Name",
                Description = "This is a Character description.",
                Level = 1,
                Id = Guid.NewGuid().ToString()
            };

            //Establish binding context
            BindingContext = this;
        }

        //Handle save clicked
        public async void Save_Clicked(object sender, EventArgs e)
        {
            //Send AddData to the controller
            MessagingCenter.Send(this, "AddData", Data);
            //Go back to the last page
            await Navigation.PopAsync();
        }

        //Handle cancel clicked
        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            //Go back to the last page
            await Navigation.PopAsync();
        }

        // The stepper function for Level
        void Level_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            LevelValue.Text = String.Format("{0}", e.NewValue);
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