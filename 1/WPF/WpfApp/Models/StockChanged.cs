namespace InventoryApp.Models;

public class StockChangedEventArgs : EventArgs
{
    public string ProductCode { get; }
    public int NewQuantity { get; }

    public StockChangedEventArgs(string productCode, int newQuantity)
    {
        ProductCode = productCode;
        NewQuantity = newQuantity;
    }
}