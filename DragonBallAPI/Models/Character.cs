using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DragonBallAPI.Models
{
    public class Character
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [StringLength(35)]
        public string Ki { get; set; }

        [StringLength(25)]
        public string Race { get; set; }

        [StringLength(25)]
        public string Gender { get; set; }

        [StringLength(35)]
        public string Affiliation { get; set; }

        // Propiedad de navegación para transformaciones
        public ICollection<Transformation> Transformations { get; set; } = new List<Transformation>();
    }

    public class Transformation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [StringLength(35)]
        public string Ki { get; set; }

        [StringLength(20)]
        public string Description { get; set; }

        // Clave foránea para Character
        public int CharacterId { get; set; }

        [JsonIgnore]
        [ForeignKey("CharacterId")]
        public Character Character { get; set; }
    }

    // Modelos para la respuesta de la API externa
    public class ExternalCharacter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ki { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }
        public string Affiliation { get; set; }
        public List<ExternalTransformation> Transformations { get; set; } = new List<ExternalTransformation>();
    }

    public class ExternalTransformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ki { get; set; }
        public string Description { get; set; }
    }

    // Modelo de usuario para autenticación JWT
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}