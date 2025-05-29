using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Lumina; // For SessionManager and ChatSession

namespace Lumina
{
    public sealed partial class MainPage : Page
    {
        private static readonly HttpClient client = new HttpClient();
        private const string API_KEY = "";

        private ObservableCollection<AIMessage> chatMessages = new ObservableCollection<AIMessage>();

        // Store the API URL and model loaded from settings
        private string apiUrl;
        private string model;

        public MainPage()
        {
            this.InitializeComponent();
            ChatItems.ItemsSource = chatMessages;
            SetStatusBarColor();
            LoadSettings(); // Load API and model from settings
        }

        // Load settings from ApplicationData
        private void LoadSettings()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            apiUrl = localSettings.Values["ApiUrl"] as string ?? "https://api.groq.com/openai/v1/chat/completions";
            model = localSettings.Values["Model"] as string ?? "meta-llama/llama-4-maverick-17b-128e-instruct";
        }

        private async void SetStatusBarColor()
        {
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = Colors.Black;
            statusBar.BackgroundOpacity = 1;
            statusBar.ForegroundColor = Colors.White;
            await statusBar.ShowAsync();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput))
                return;

            // Determine adaptive colors for the user's message bubble
            var accentBrush = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
            var userColors = GetAdaptiveBubbleColors(accentBrush.Color);
            SolidColorBrush userBubbleBrush = userColors.Item1;
            SolidColorBrush userTextBrush = userColors.Item2;

            // Display the user's message in the chat
            chatMessages.Add(new AIMessage
            {
                Content = userInput,
                Align = HorizontalAlignment.Right,
                BackgroundColor = userBubbleBrush,
                ForegroundColor = userTextBrush
            });
            InputTextBox.Text = "";
            ScrollToBottom();

            string userId = "user123";
            // Pass the model loaded from settings as the second parameter
            var session = SessionManager.GetOrCreateSession(userId, model);
            session.AddMessage("User", userInput);

            string aiText;
            try
            {
                string fullPrompt = session.GetFullPrompt(userInput);
                var payload = new
                {
                    model = model,
                    messages = new[] { new { role = "user", content = fullPrompt } }
                };

                var json = JsonConvert.SerializeObject(payload);
                var bodyContent = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");

                var response = await client.PostAsync(apiUrl, bodyContent);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var j = JObject.Parse(body);
                    aiText = j["choices"]?[0]?["message"]?["content"]?.ToString() ?? "No response.";
                }
                else
                {
                    aiText = $"Error {response.StatusCode}: {body}";
                }
            }
            catch (Exception ex)
            {
                aiText = $"Exception: {ex.Message}";
            }

            // Determine adaptive colors for the AI's message bubble
            var aiAccentColor = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
            var aiBubbleColor = DarkenColor(aiAccentColor.Color, 30);
            SolidColorBrush aiBubbleBrush = new SolidColorBrush(aiBubbleColor);
            SolidColorBrush aiTextBrush = new SolidColorBrush(Colors.White);

            // Display the AI's message in the chat
            chatMessages.Add(new AIMessage
            {
                Content = aiText,
                Align = HorizontalAlignment.Left,
                BackgroundColor = aiBubbleBrush,
                ForegroundColor = aiTextBrush
            });

            session.AddMessage("Assistant", aiText);

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ScrollToBottom();
                PlaySound();
            });
        }

        private void ScrollToBottom()
        {
            if (ChatScrollViewer != null)
            {
                ChatScrollViewer.ChangeView(null, ChatScrollViewer.ScrollableHeight, null);
            }
        }

        private void PlaySound()
        {
            try
            {
                if (ReplySound != null)
                {
                    ReplySound.Source = new Uri("ms-appx:///Assets/replySound.mp3");
                    ReplySound.AutoPlay = true;
                    ReplySound.Play();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ReplySound is not initialized.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        private Tuple<SolidColorBrush, SolidColorBrush> GetAdaptiveBubbleColors(Color background)
        {
            bool isLight = (background.R * 0.299 + background.G * 0.587 + background.B * 0.114) > 186;
            SolidColorBrush bubbleBrush = new SolidColorBrush(background);
            SolidColorBrush textBrush = new SolidColorBrush(isLight ? Colors.Black : Colors.White);
            return Tuple.Create(bubbleBrush, textBrush);
        }

        private Color DarkenColor(Color originalColor, double amount)
        {
            double r = originalColor.R * (1 - amount / 100);
            double g = originalColor.G * (1 - amount / 100);
            double b = originalColor.B * (1 - amount / 100);
            return Color.FromArgb(originalColor.A, (byte)r, (byte)g, (byte)b);
        }

        private void SetSendButtonColor()
        {
            var accentBrush = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
            SubmitButton.Background = accentBrush;
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SetSendButtonColor();
        }

        // Handler to navigate to SettingsPage
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to ChatHistoryPage when the History button is clicked.
            this.Frame.Navigate(typeof(ChatHistoryPage));
        }

    }

    public class AIMessage
    {
        public string Content { get; set; }
        public HorizontalAlignment Align { get; set; }
        public SolidColorBrush BackgroundColor { get; set; }
        public SolidColorBrush ForegroundColor { get; set; }
    }
}
