namespace UsedAndReliableCars.Models
{
    public class UsedCar
    {
        public string CarId { get; set; }
        public string Type { get; set; }
        public int PriceCategory { get; set; }
        public int Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public List<UsedCar> UsedCars { get; set; }

        
    }
}
