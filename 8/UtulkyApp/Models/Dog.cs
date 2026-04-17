namespace UtulkyApp.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Breed { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Cizí klíč a vazba na útulek
        public int ShelterId { get; set; }
        public Shelter? Shelter { get; set; }
    }
}