using EShopEf.Data;
using EShopEf.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShopEf.Services;

public class EShopService
{
    public void CreateData()
    {
        using var ctx = new AppDbContext();
        
        var customer = new Customer { Name = "Jan Novák", Email = "jan@novak.cz" };
        var product1 = new Product { Name = "Klávesnice", Price = 1200 };
        var product2 = new Product { Name = "Myš", Price = 800 };

        ctx.Customers.Add(customer);
        ctx.Products.AddRange(product1, product2);
        ctx.SaveChanges();

        var order = new Order { Date = DateTime.Now, CustomerId = customer.Id };
        ctx.Orders.Add(order);
        ctx.SaveChanges();

        var item1 = new OrderItem { OrderId = order.Id, ProductId = product1.Id, Quantity = 1 };
        var item2 = new OrderItem { OrderId = order.Id, ProductId = product2.Id, Quantity = 2 };
        ctx.OrderItems.AddRange(item1, item2);
        ctx.SaveChanges();
        
        Console.WriteLine("CREATE: Data úspěšně vložena.");
    }

    public void ReadData()
    {
        using var ctx = new AppDbContext();
        var orders = ctx.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();

        Console.WriteLine("\n--- READ: Výpis objednávek ---");
        foreach (var o in orders)
        {
            Console.WriteLine($"Objednávka #{o.Id} od: {o.Customer?.Name}");
            decimal totalPrice = 0;
            foreach (var item in o.OrderItems)
            {
                var itemTotal = item.Quantity * (item.Product?.Price ?? 0);
                totalPrice += itemTotal;
                Console.WriteLine($" - {item.Quantity}x {item.Product?.Name} = {itemTotal} Kč");
            }
            Console.WriteLine($"Celkem: {totalPrice} Kč\n");
        }
    }

    public void UpdateData()
    {
        using var ctx = new AppDbContext();
        
        var cust = ctx.Customers.FirstOrDefault(c => c.Name == "Jan Novák");
        if (cust != null) cust.Name = "Jan Dvořák";

        var prod = ctx.Products.FirstOrDefault(p => p.Name == "Klávesnice");
        if (prod != null) prod.Price = 1100;

        ctx.SaveChanges();
        Console.WriteLine("UPDATE: Data upravena.");
    }

    public void DeleteData()
    {
        using var ctx = new AppDbContext();
        var order = ctx.Orders.FirstOrDefault();
        if (order != null)
        {
            ctx.Orders.Remove(order);
            ctx.SaveChanges();
            Console.WriteLine($"DELETE: Objednávka #{order.Id} smazána.");
        }
    }

    public void PrintCustomerStats()
    {
        using var ctx = new AppDbContext();
        var stats = ctx.Customers
            .Select(c => new { c.Name, OrderCount = c.Orders.Count })
            .ToList();

        Console.WriteLine("\n--- LINQ: Statistiky zákazníků ---");
        foreach (var stat in stats)
        {
            Console.WriteLine($"Zákazník: {stat.Name}, Počet objednávek: {stat.OrderCount}");
        }
    }
}