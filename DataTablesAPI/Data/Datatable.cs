namespace DataTablesTest.Data;

public class DatatableRequest
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public Search Search { get; set; } = null!;
    public List<Sort> Order { get; set; } = null!;
    public List<Column> Columns { get; set; } = null!;
}

public class Search
{
    public string? Value { get; set; }
    public bool Regex { get; set; }
}

public class Sort
{
    public int Column { get; set; }
    public string Dir { get; set; } = null!;
}

public class Column
{
    public string Data { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool Searchable { get; set; }
    public bool Orderable { get; set; }
    public Search? Search { get; set; }
}

public class DatatableResponse<T>
{
    public int Draw { get; set; }
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
    public List<T>? Data { get; set; }
}