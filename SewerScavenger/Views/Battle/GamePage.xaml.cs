using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SewerScavenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        // Initialize the components
        public GamePage()
        {
            InitializeComponent();
        }

        // Allows the user to start the battle by selecting the party and picking the battle type
        async void GameButtonClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PickCharacter());
        }
        
        // Allows the user to read through a tutorial
        async void TutorialButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new TutorialPage());
        }
    }
}