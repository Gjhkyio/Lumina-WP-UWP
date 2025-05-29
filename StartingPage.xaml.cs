using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System;


namespace Lumina
{
    public sealed partial class StartingPage : Page
    {
        public StartingPage()
        {
            this.InitializeComponent();
            SetAccentColors(); // Set accent colors
        }

        // Set the accent color for Lumina text and Start button
        private void SetAccentColors()
        {
            // Access system accent color using PhoneAccentBrush
            var accentBrush = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];

            // Set the "Lumina" text to the accent color
            LuminaText.Foreground = accentBrush;

            // Set the Start button's background to the accent color
            StartButton.Background = accentBrush;
            StartButton.BorderBrush = accentBrush;
        }

        // Navigate to MainPage when the Start button is clicked
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));  // Navigate to MainPage when the button is clicked
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
