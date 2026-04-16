namespace InventoryApp;
using System.Collections;

public class Inventory : IEnumerable<Product>
{
    private readonly List<Product> _products;

    private readonly Dictionary<string, int> _itemsCount;

    public Inventory()
    {
        _products = new List<Product>();
        _itemsCount = new Dictionary<string, int>();
    }

    public Product this[int i]
    {
        get
        {
            if (i < 0 || i >= _products.Count)
            {
                throw new IndexOutOfRangeException($"Index {i} je mimo rozsah skladu (Počet položek: {_products.Count}).");
            }
            return _products[i];
        }
    }

    public IEnumerator<Product> GetEnumerator()
    {
        return _products.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void AddProduct(Product product, int initialQuantity)
    {
        if (!_itemsCount.ContainsKey(product.Code))
        {
            _products.Add(product);
            _itemsCount.Add(product.Code, initialQuantity);
            OnStockChanged(product.Code, initialQuantity);
        }
        else
        {
            _itemsCount[product.Code] += initialQuantity;
            OnStockChanged(product.Code, _itemsCount[product.Code]);
        }
    }

    public int this[string code]
    {
        get
        {
            if (_itemsCount.ContainsKey(code))
            {
                return _itemsCount[code];
            }
            return 0;
        }
        set
        {
            if (_itemsCount.ContainsKey(code))
            {
                if (value < 0)
                    throw new ArgumentException("Množství na skladě nesmí být záporné.");

                _itemsCount[code] = value;
                OnStockChanged(code, value);
            }
            else
            {
                throw new InvalidOperationException($"Produkt s kódem '{code}' neexistuje.");
            }
        }
    }

    public static Inventory operator +(Inventory a, Inventory b)
    {
        Inventory result = new Inventory();
        
        foreach (var p in a)
        {
            result.AddProduct(p, a[p.Code]);
        }

        foreach (var p in b)
        {
            result.AddProduct(p, b[p.Code]);
        }

        return result;
    }

    public static Inventory operator -(Inventory a, Inventory b)
    {
        Inventory result = new Inventory();

        foreach (var p in a)
        {
            result.AddProduct(p, a[p.Code]);
        }

        foreach (var p in b)
        {
            int current = result[p.Code];
            
            if (current > 0)
            {
                int toRemove = b[p.Code];
                result[p.Code] = Math.Max(0, current - toRemove);
            }
        }

        return result;
    }

    public static bool operator >(Inventory a, Inventory b)
    {
        long countA = a.Sum(p => (long)a[p.Code]);
        long countB = b.Sum(p => (long)b[p.Code]);
        return countA > countB;
    }

    public static bool operator <(Inventory a, Inventory b)
    {
        long countA = a.Sum(p => (long)a[p.Code]);
        long countB = b.Sum(p => (long)b[p.Code]);
        return countA < countB;
    }

    public event EventHandler<StockChangedEventArgs>? StockChanged;
    protected void OnStockChanged(string code, int newQuantity)
    {
        StockChanged?.Invoke(this, new StockChangedEventArgs(code, newQuantity));
    }
}