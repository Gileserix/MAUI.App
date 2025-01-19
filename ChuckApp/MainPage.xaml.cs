using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ChuckApp
{
    public partial class MainPage : ContentPage
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7216/"), // Cambia aquí al puerto correcto
            Timeout = TimeSpan.FromSeconds(30)
        };

        // Lista de categorías de chistes
        private readonly string[] categories = new string[]
        {
            "animal", "career", "celebrity", "dev", "explicit", "fashion",
            "food", "history", "money", "movie", "music", "political",
            "religion", "science", "sport", "travel"
        };

        public MainPage()
        {
            InitializeComponent();

            // Asignar las categorías al Picker
            CategoryPicker.ItemsSource = categories;

            // Comprobar si las categorías se asignaron correctamente
            ResultsLabel.Text = "Categorías asignadas: " + string.Join(", ", categories);
        }

        // Obtener un chiste aleatorio
        private async void OnRandomJokeClicked(object sender, EventArgs e)
        {
            await CallApiAndDisplayResultAsync("jokes/random");
        }

        // Obtener un chiste por categoría
        private async void OnJokeByCategoryClicked(object sender, EventArgs e)
        {
            string selectedCategory = CategoryPicker.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedCategory))
            {
                await DisplayAlert("Error", "Por favor selecciona una categoría.", "OK");
                return;
            }

            string endpoint = $"jokes/random/category?category={selectedCategory}";
            await CallApiAndDisplayResultAsync(endpoint);
        }

        // Buscar chistes por palabra clave
        private async void OnSearchJokesClicked(object sender, EventArgs e)
        {
            string query = SearchEntry.Text;
            if (string.IsNullOrEmpty(query))
            {
                await DisplayAlert("Error", "Por favor escribe algo para buscar.", "OK");
                return;
            }

            string endpoint = $"jokes/search?query={query}";
            await CallApiAndDisplayResultAsync(endpoint);
        }

        // Llamar a la API y mostrar el resultado
        private async Task CallApiAndDisplayResultAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var jokeObject = JsonConvert.DeserializeObject<dynamic>(result);

                        if (jokeObject?.value != null)
                        {
                            ResultsLabel.Text = jokeObject.value.ToString();
                        }
                        else
                        {
                            ResultsLabel.Text = result;
                        }
                    }
                    catch
                    {
                        ResultsLabel.Text = result;
                    }
                }
                else
                {
                    await DisplayAlert("Error", $"Error de API: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo conectar a la API: {ex.Message}", "OK");
            }
        }
    }
}

