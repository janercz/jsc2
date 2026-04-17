namespace UtulkyApp.Models
{
    public class Shelter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        
        // Útulek ví, jaké má psy
        public ICollection<Dog> Dogs { get; set; } = new List<Dog>();
    }
}