namespace DataTablesTest.Data;

public class Office
{
    public Office()
    {
        Employees = new HashSet<Employee>();
    }

    public string OfficeCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string? State { get; set; }
    public string Country { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Territory { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; }
}