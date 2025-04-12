using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DragonBallAPI.Data;
using DragonBallAPI.Models;

namespace DragonBallAPI.Services
{
    public interface ICharacterService
    {
        Task<List<Character>> GetAllCharactersAsync();
        Task<Character> GetCharacterByIdAsync(int id);
        Task<Character> GetCharacterByNameAsync(string name);
        Task<List<Character>> GetCharactersByAffiliationAsync(string affiliation);
        Task<string> SyncCharactersAsync();
    }

    public class CharacterService : ICharacterService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDragonBallApiClient _apiClient;

        public CharacterService(ApplicationDbContext context, IDragonBallApiClient apiClient)
        {
            _context = context;
            _apiClient = apiClient;
        }

        public async Task<List<Character>> GetAllCharactersAsync()
        {
            return await _context.Characters
                .Include(c => c.Transformations)
                .ToListAsync();
        }

        public async Task<Character> GetCharacterByIdAsync(int id)
        {
            return await _context.Characters
                .Include(c => c.Transformations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Character> GetCharacterByNameAsync(string name)
        {
            return await _context.Characters
                .Include(c => c.Transformations)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Character>> GetCharactersByAffiliationAsync(string affiliation)
        {
            return await _context.Characters
                .Include(c => c.Transformations)
                .Where(c => c.Affiliation.ToLower() == affiliation.ToLower())
                .ToListAsync();
        }

        public async Task<string> SyncCharactersAsync()
        {
            // Verificar si ya hay datos en la base de datos
            if (await _context.Characters.AnyAsync())
            {
                return "La base de datos ya contiene datos. Por favor, limpie la base de datos antes de sincronizar.";
            }

            // Obtener personajes desde la API externa
            var externalCharacters = await _apiClient.GetAllCharactersAsync();

            // Filtrar solo personajes con afiliación "Z Fighter"
            var zFighters = externalCharacters.Where(c => c.Affiliation?.ToLower() == "z fighter").ToList();
            Console.WriteLine($"Total Z Fighters encontrados: {zFighters.Count}");

            foreach (var externalChar in zFighters)
            {
                // Crear y guardar el personaje
                var character = new Character
                {
                    Name = externalChar.Name,
                    Ki = externalChar.Ki,
                    Race = externalChar.Race,
                    Gender = externalChar.Gender,
                    Affiliation = externalChar.Affiliation
                };

                _context.Characters.Add(character);
                await _context.SaveChangesAsync(); // Generar ID del personaje

                // Guardar transformaciones asociadas
                if (externalChar.Transformations != null && externalChar.Transformations.Any())
                {
                    Console.WriteLine($"Procesando transformaciones para {externalChar.Name}: {externalChar.Transformations.Count}");

                    foreach (var externalTransform in externalChar.Transformations)
                    {
                        var transformation = new Transformation
                        {
                            Name = externalTransform.Name,
                            Ki = externalTransform.Ki,
                            Description = externalTransform.Description,
                            CharacterId = character.Id // Asociar con el personaje creado
                        };

                        _context.Transformations.Add(transformation);
                        Console.WriteLine($"Transformación '{externalTransform.Name}' agregada para {externalChar.Name}.");
                    }
                }
                else
                {
                    Console.WriteLine($"No se encontraron transformaciones para {externalChar.Name}.");
                }
            }

            await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos
            return $"Sincronización completada: {zFighters.Count} personajes Z Fighters y sus transformaciones.";
        }
    }
}