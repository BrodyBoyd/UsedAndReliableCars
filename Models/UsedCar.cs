using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

namespace UsedAndReliableCars.Models
{
    public class UsedCar
    {
        public string Type { get; set; }
        public int PriceCategory { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }

        public List<UsedCar> UsedCars { get; set; } = new List<UsedCar>();

        [JsonIgnore]
        public UsedCar Car { get; set; }

    }
}