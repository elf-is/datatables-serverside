namespace DataTablesTest.Data;

public class Orderdetail
{
    public int OrderNumber { get; set; }
    public string ProductCode { get; set; } = null!;
    public int QuantityOrdered { get; set; }
    public decimal PriceEach { get; set; }
    public short OrderLineNumber { get; set; }

    public virtual Order OrderNumberNavigation { get; set; } = null!;
    public virtual Product ProductCodeNavigation { get; set; } = null!;
}