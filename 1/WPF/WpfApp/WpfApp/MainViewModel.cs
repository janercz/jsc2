using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfApp.Models;

namespace WpfApp;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly Inventory _inventory;

    public ObservableCollection<Product> Products { get; } = new();

    private string _code = "";
    public string Code { get => _code; set { _code = value; OnPropertyChanged(); } }

    private string _name = "";
    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

    private decimal _price;
    public decimal Price { get => _price; set { _price = value; OnPropertyChanged(); } }

    private int _qty;
    public int Qty { get => _qty; set { _qty = value; OnPropertyChanged(); } }

    public int TotalQuantity => Products.Sum(p => _inventory[p.Code]);

    public RelayCommand AddCommand { get; }
    public RelayCommand RemoveCommand { get; }

    public MainViewModel()
    {
        _inventory = new Inventory();

        _inventory.StockChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(TotalQuantity));
        };

        AddCommand = new RelayCommand(_ => {
            try
            {
                var newProduct = new Product(Code, Name, Price);
                _inventory.AddProduct(newProduct, Qty);

                if (!Products.Any(p => p.Code == newProduct.Code))
                {
                    Products.Add(newProduct);
                }
                OnPropertyChanged(nameof(TotalQuantity));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        });

        RemoveCommand = new RelayCommand(_ => {
            if (!string.IsNullOrEmpty(Code))
            {
                _inventory[Code] = 0;
                var toRemove = Products.FirstOrDefault(p => p.Code == Code);
                if (toRemove != null) Products.Remove(toRemove);
            }
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}