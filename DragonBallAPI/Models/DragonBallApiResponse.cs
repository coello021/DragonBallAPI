using System.Collections.Generic;

namespace DragonBallAPI.Models
{
    public class DragonBallApiResponse
    {
        public List<ExternalCharacter> Items { get; set; }
        public MetaData Meta { get; set; }
        public Links Links { get; set; }
    }

    public class MetaData
    {
        public int TotalItems { get; set; }
        public int ItemCount { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

    public class Links
    {
        public string First { get; set; }
        public string Previous { get; set; }
        public string Next { get; set; }
        public string Last { get; set; }
    }
}

