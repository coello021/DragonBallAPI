using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DragonBallAPI.Models;
using Microsoft.EntityFrameworkCore;
using DragonBallAPI.Data;
using DragonBallAPI.Services;

namespace DragonBallAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Character>>> GetAllCharacters()
        {
            var characters = await _characterService.GetAllCharactersAsync();
            return Ok(characters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Character>> GetCharacterById(int id)
        {
            var character = await _characterService.GetCharacterByIdAsync(id);

            if (character == null)
                return NotFound($"Personaje con ID {id} no encontrado.");

            return Ok(character);
        }

        [HttpGet("byname/{name}")]
        public async Task<ActionResult<Character>> GetCharacterByName(string name)
        {
            var character = await _characterService.GetCharacterByNameAsync(name);

            if (character == null)
                return NotFound($"Personaje con nombre '{name}' no encontrado.");

            return Ok(character);
        }

        [HttpGet("byaffiliation/{affiliation}")]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharactersByAffiliation(string affiliation)
        {
            var characters = await _characterService.GetCharactersByAffiliationAsync(affiliation);
            return Ok(characters);
        }

        [HttpPost("sync")]
        public async Task<ActionResult<string>> SyncCharacters()
        {
            var result = await _characterService.SyncCharactersAsync();
            return Ok(result);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransformationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransformationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transformation>>> GetAllTransformations()
        {
            // Directamente obtenemos las transformaciones desde el contexto
            var transformations = await _context.Transformations
                .Include(t => t.Character) // Incluir relación con personajes
                .ToListAsync();

            return Ok(transformations);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel model)
        {
            var token = await _authService.AuthenticateAsync(model.Username, model.Password);

            if (string.IsNullOrEmpty(token))
                return Unauthorized("Usuario o contraseña inválidos");

            return Ok(new { Token = token });
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
