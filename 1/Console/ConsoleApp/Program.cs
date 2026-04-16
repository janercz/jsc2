namespace InventoryApp;

class Program
{
    static void Main(string[] args)
    {
        var p1 = new Product("A100", "Notebook Dell", 25000.50m);
        var p2 = new Product("B200", "Myš Logitech", 899.00m);
        var p3 = new Product("C300", "Monitor Samsung", 5400.00m);

        var inventoryA = new Inventory();
        var inventoryB = new Inventory();

        inventoryA.StockChanged += (sender, e) => 
        {
            Console.WriteLine($"[EVENT] Sklad A: Změna stavu u '{e.ProductCode}' na {e.NewQuantity} ks.");
        };

        Console.WriteLine("--- Přidávání produktů ---");
        inventoryA.AddProduct(p1, 10);
        inventoryA.AddProduct(p2, 50);
        
        inventoryB.AddProduct(p2, 20);
        inventoryB.AddProduct(p3, 5);

        Console.WriteLine("\n--- Práce s indexery ---");
        Console.WriteLine($"Produkt na indexu 0 ve Skladu A: {inventoryA[0].Name}");
        Console.WriteLine("Měním množství u A100 pomocí indexeru...");
        inventoryA["A100"] = 15; 


        Console.WriteLine("\n--- Porovnání skladů ---");
        Console.WriteLine($"Sklad A má více kusů než Sklad B: {inventoryA > inventoryB}");


        Console.WriteLine("\n--- Operace se sklady (+, -) ---");

        var combinedInventory = inventoryA + inventoryB;
        Console.WriteLine("Sklad A + Sklad B (Celkový přehled):");
        foreach (var prod in combinedInventory)
        {
            Console.WriteLine($"- {prod.Name}: {combinedInventory[prod.Code]} ks");
        }

        var subtractedInventory = inventoryA - inventoryB;
        Console.WriteLine("\nSklad A - Sklad B (Odečtení duplicit):");
        Console.WriteLine($"- {p2.Name} zbývá: {subtractedInventory[p2.Code]} ks");


        Console.WriteLine("\n--- Test validace ---");
        try
        {
            p1.Price = -100;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Chycena očekávaná chyba: {ex.ParamName} - {ex.Message.Split('\r')[0]}");
        }
    }
}