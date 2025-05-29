using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Lumina
{
    public sealed partial class ChatHistoryPage : Page
    {
        // If you have one persistent user, use the same ID everywhere.
        private const string CurrentUserId = "user123";

        public ChatHistoryPage()
        {
            this.InitializeComponent();

            // Populate the model dropdown
            ModelComboBox.ItemsSource = new[]
            {
                "allam-2-7b",
                "compound-beta",
                "compound-beta-mini",
                "gemma2-9b-it",
                "llama-3.1-8b-instant",
                "llama-3.3-70b-versatile",
                "llama3-70b-8192",
                "llama3-8b-8192",
                "meta-llama/llama-4-maverick-17b-128e-instruct",
                "meta-llama/llama-4-scout-17b-16e-instruct"
            };
            ModelComboBox.SelectedIndex = 0;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Load everything once
            await SessionManager.LoadSessionsAsync();
            // Show the currently selected
            DisplayHistoryForSelectedModel();
        }

        private void ModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => DisplayHistoryForSelectedModel();

        private void DisplayHistoryForSelectedModel()
        {
            var model = ModelComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(model))
            {
                ChatListBox.ItemsSource = null;
                return;
            }

            // Gets (or creates) exactly the session for this model
            var session = SessionManager.GetOrCreateSession(CurrentUserId, model);
            ChatListBox.ItemsSource = session.Messages;
        }

        private async void DeleteHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var model = ModelComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(model))
                return;

            // Clear only this model’s messages
            var session = SessionManager.GetOrCreateSession(CurrentUserId, model);
            session.Messages.Clear();

            // Persist change
            await SessionManager.SaveSessionsAsync();

            // Refresh the list (now empty)
            ChatListBox.ItemsSource = null;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }
    }
}
