namespace DataTablesTest.Data;

public class Employee
{
    public Employee()
    {
        Customers = new HashSet<Customer>();
        InverseReportsToNavigation = new HashSet<Employee>();
    }

    public int EmployeeNumber { get; set; }
    public string LastName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string Extension { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string OfficeCode { get; set; } = null!;
    public int? ReportsTo { get; set; }
    public string JobTitle { get; set; } = null!;

    public virtual Office OfficeCodeNavigation { get; set; } = null!;
    public virtual Employee? ReportsToNavigation { get; set; }
    public virtual ICollection<Customer> Customers { get; set; }
    public virtual ICollection<Employee> InverseReportsToNavigation { get; set; }
}