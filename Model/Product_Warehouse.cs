namespace Tutorial9.Model;

public class Product_Warehouse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Product Product { get; set; }
    public Warehouse Warehouse { get; set; }
}