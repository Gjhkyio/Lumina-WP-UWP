using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

namespace Lumina
{
    public sealed partial class SettingsPage : Page
    {
        private string previousModel = null;

        public SettingsPage()
        {
            this.InitializeComponent();
            LoadSettings();  // Load saved settings when the page is loaded
        }

        // Load the saved settings (API Key and Model) from local storage
        private void LoadSettings()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("API_URL"))
            {
                ApiKeyTextBox.Text = localSettings.Values["API_URL"].ToString();
            }

            if (localSettings.Values.ContainsKey("Model"))
            {
                string savedModel = localSettings.Values["Model"].ToString();
                previousModel = savedModel;

                foreach (ComboBoxItem item in ModelComboBox.Items)
                {
                    if (item.Content.ToString() == savedModel)
                    {
                        ModelComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        // Event handler for when the selection in the ComboBox changes
        private void ModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)ModelComboBox.SelectedItem;
            if (selectedItem != null)
            {
                var newModel = selectedItem.Content.ToString();
                var localSettings = ApplicationData.Current.LocalSettings;

                // Only clear memory if model changed
                if (previousModel != null && previousModel != newModel)
                {
                    ClearMemory(); // Clear chat history/memory
                }

                // Save the new model
                localSettings.Values["Model"] = newModel;
                previousModel = newModel;
            }
        }

        // Function to clear chat session memory (keep API key)
        private void ClearMemory()
        {
            SessionManager.ClearSessions(); // Clears all chat sessions only
        }

        private void ApiKeyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string apiUrl = ApiKeyTextBox.Text;

            if (!string.IsNullOrEmpty(apiUrl))
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["API_URL"] = apiUrl;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            localSettings.Values["API_URL"] = ApiKeyTextBox.Text.Trim();
            localSettings.Values["Model"] = ModelComboBox.SelectedItem.ToString();

            this.Frame.Navigate(typeof(MainPage));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
        private void OpenWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the InfoPage
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(InfoPage));
            }
        }


    }
}
