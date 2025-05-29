using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lumia
{
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

        private async void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            string query = searchTextBox.Text;
            if (!string.IsNullOrEmpty(query))
            {
                string data = await GetDeepSeekDataAsync(query);
                DisplayResults(data);
            }
        }

        private void DisplayResults(string data)
        {
            var json = JsonConvert.DeserializeObject<dynamic>(data);
            var results = new List<SearchResult>();

            foreach (var item in json.results)
            {
                results.Add(new SearchResult
                {
                    Title = item.title,
                    Description = item.description
                });
            }

            resultsListView.ItemsSource = results;
        }
    }

    public class SearchResult
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
