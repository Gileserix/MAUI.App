namespace WebAPI
{
    public class ChuckNorrisJoke
    {
        public string Id { get; set; } // ID único del chiste
        public string Value { get; set; } // Contenido del chiste
        public string Url { get; set; } // URL del chiste en la API
    }

    public class ChuckNorrisSearchResponse
    {
        public int Total { get; set; } // Total de resultados encontrados
        public ChuckNorrisJoke[] Result { get; set; } // Lista de chistes encontrados
    }
}
