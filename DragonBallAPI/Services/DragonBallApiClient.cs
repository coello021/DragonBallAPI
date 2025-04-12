using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DragonBallAPI.Models;

namespace DragonBallAPI.Services
{
    public interface IDragonBallApiClient
    {
        Task<List<ExternalCharacter>> GetAllCharactersAsync();
        Task<ExternalCharacter> GetCharacterByIdAsync(int id);
        Task<List<ExternalTransformation>> GetTransformationsForCharacterAsync(int characterId);
    }

    public class DragonBallApiClient : IDragonBallApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public DragonBallApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<ExternalCharacter>> GetAllCharactersAsync()
        {
            var response = await _httpClient.GetAsync("api/characters");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("JSON recibido: " + content);

            try
            {
                // Deserializar como objeto contenedor con una propiedad 'items'
                var apiResponse = JsonSerializer.Deserialize<DragonBallApiResponse>(content, _jsonOptions);

                // Obtener transformaciones para cada personaje
                foreach (var character in apiResponse.Items)
                {
                    character.Transformations = await GetTransformationsForCharacterAsync(character.Id);
                }

                return apiResponse.Items;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializando los personajes: {ex.Message}");
                throw new Exception("La respuesta de la API externa no tiene el formato esperado.", ex);
            }
        }

        public async Task<ExternalCharacter> GetCharacterByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/characters/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var character = JsonSerializer.Deserialize<ExternalCharacter>(content, _jsonOptions);

            // Obtener transformaciones para el personaje
            character.Transformations = await GetTransformationsForCharacterAsync(id);

            return character;
        }

        public async Task<List<ExternalTransformation>> GetTransformationsForCharacterAsync(int characterId)
        {
            var response = await _httpClient.GetAsync($"api/characters/{characterId}/transformations");

            // Si no hay transformaciones, devolver una lista vacía
            if (!response.IsSuccessStatusCode)
                return new List<ExternalTransformation>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ExternalTransformation>>(content, _jsonOptions);
        }
    }
}
