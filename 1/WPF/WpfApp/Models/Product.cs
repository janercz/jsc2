namespace InventoryApp.Models;

public class Product : IEquatable<Product>
{
    public string Code { get; }

    public string Name { get; set; }

    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Cena nesmí být záporná.");
            }
            _price = value;
        }
    }

    public Product(string code, string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Kód produktu nesmí být prázdný.", nameof(code));
        }

        Code = code;
        Name = name;
        Price = price;
    }

    public bool Equals(Product? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Product);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public override string ToString()
    {
        return $"Produkt: {Name} [{Code}], Cena: {Price} CZK";
    }

    public static bool operator ==(Product left, Product right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(Product left, Product right) => !(left == right);
}