using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("jokes")]
    public class ChuckNorrisApiController : ControllerBase
    {
        private readonly ILogger<ChuckNorrisApiController> _logger;
        private readonly HttpClient _httpClient;

        public ChuckNorrisApiController(ILogger<ChuckNorrisApiController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient; // Usa HttpClient inyectado
        }

        // Endpoint: /jokes/random
        [HttpGet("random", Name = "GetRandomJoke")]
        public async Task<IActionResult> GetRandomJoke()
        {
            string apiUrl = "https://api.chucknorris.io/jokes/random";

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return Content(json, "application/json");
                }

                return StatusCode((int)response.StatusCode, "Error al obtener el chiste");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consumir la API de Chuck Norris");
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }

        // Endpoint: /jokes/random?category={category}
        [HttpGet("random/category", Name = "GetJokeByCategory")]
        public async Task<IActionResult> GetJokeByCategory([FromQuery] string category)
        {
            string categoriesApiUrl = "https://api.chucknorris.io/jokes/categories";
            string apiUrl = $"https://api.chucknorris.io/jokes/random?category={category}";

            try
            {
                // Verifica si la categoría es válida
                var categoriesResponse = await _httpClient.GetAsync(categoriesApiUrl);

                if (!categoriesResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)categoriesResponse.StatusCode, "Error al obtener las categorías disponibles");
                }

                var categoriesJson = await categoriesResponse.Content.ReadAsStringAsync();
                var availableCategories = JsonSerializer.Deserialize<string[]>(categoriesJson);

                if (availableCategories == null || !availableCategories.Contains(category))
                {
                    return BadRequest($"La categoría '{category}' no es válida. Categorías disponibles: {string.Join(", ", availableCategories)}");
                }

                // Realiza la solicitud al endpoint de la categoría
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return Content(json, "application/json");
                }

                return StatusCode((int)response.StatusCode, $"Error al obtener un chiste para la categoría: {category}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consumir la API de Chuck Norris por categoría");
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }

        // Endpoint: /jokes/search?query={query}
        [HttpGet("search", Name = "SearchJokes")]
        public async Task<IActionResult> SearchJokes([FromQuery] string query)
        {
            string apiUrl = $"https://api.chucknorris.io/jokes/search?query={query}";

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return Content(json, "application/json");
                }

                return StatusCode((int)response.StatusCode, $"No se encontraron resultados para: {query}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar chistes en la API de Chuck Norris");
                return StatusCode(500, "Ocurrió un error inesperado");
            }
        }
    }
}
