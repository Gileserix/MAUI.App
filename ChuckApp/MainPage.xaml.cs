using ChuckApp.Models; // Importar el espacio de nombres adecuado
using Newtonsoft.Json;

namespace ChuckApp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public MainPage(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Inicializar ItemsSource del Picker
            CategoryPicker.ItemsSource = new string[]
            {
                "animal", "career", "celebrity", "dev", "explicit", "fashion",
                "food", "history", "money", "movie", "music", "political",
                "religion", "science", "sport", "travel"
            };
        }

        private async void OnRandomJokeClicked(object sender, EventArgs e)
        {
            try
            {
                var response = await _httpClient.GetAsync("https://webapi20250123162938.azurewebsites.net/jokes/random");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var joke = JsonConvert.DeserializeObject<ChuckNorrisJoke>(json);
                    if (joke != null)
                    {
                        ResultsLabel.Text = joke.Value;
                    }
                    else
                    {
                        ResultsLabel.Text = "Error al deserializar el chiste.";
                    }
                }
                else
                {
                    ResultsLabel.Text = "Error al obtener el chiste.";
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

                throw;
            }
  
        }

        private async void OnJokeByCategoryClicked(object sender, EventArgs e)
        {
            var category = CategoryPicker.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(category))
            {
                ResultsLabel.Text = "Seleccione una categoría.";
                return;
            }

            try
            {
                var response = await _httpClient.GetAsync($"https://webapi20250123162938.azurewebsites.net/jokes/random/category?category={category}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var joke = JsonConvert.DeserializeObject<ChuckNorrisJoke>(json);
                    if (joke != null)
                    {
                        ResultsLabel.Text = joke.Value;
                    }
                    else
                    {
                        ResultsLabel.Text = "Error al deserializar el chiste.";
                    }
                }
                else
                {
                    ResultsLabel.Text = "Error al obtener el chiste.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                throw;
            }

        }

        private async void OnSearchJokesClicked(object sender, EventArgs e)
        {
            var query = SearchEntry.Text;
            if (string.IsNullOrEmpty(query))
            {
                ResultsLabel.Text = "Ingrese una palabra clave.";
                return;
            }

            var response = await _httpClient.GetAsync($"https://webapi20250123162938.azurewebsites.net/jokes/search?query={query}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<ChuckNorrisSearchResponse>(json);
                if (searchResponse != null && searchResponse.Result != null)
                {
                    ResultsLabel.Text = string.Join("\n", searchResponse.Result.Select(j => j.Value));
                }
                else
                {
                    ResultsLabel.Text = "Error al deserializar los resultados de la búsqueda.";
                }
            }
            else
            {
                ResultsLabel.Text = "Error al buscar chistes.";
            }
        }
    }
}
