using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;
using DataTablesTest.Data;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace DataTablesTest.Services;

public class DataTableService<T> where T : class
{
    private readonly ClassicModelsContext _context;

    public DataTableService(ClassicModelsContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Get the data from the database
    /// </summary>
    /// <returns>A IQueryable of the data</returns>
    public IQueryable<T> GetData()
    {
        return _context.Set<T>().AsQueryable().AsNoTracking();
    }

    /// <summary>
    ///     Get the dataTable data after searching and ordering
    /// </summary>
    /// <param name="request">The <see cref="DatatableRequest" /> object</param>
    /// <param name="model">The requested model to process</param>
    /// <returns>A <see cref="DatatableResponse{T}" /> object</returns>
    public DatatableResponse<T> GetDatatableObject(DatatableRequest request,
        IQueryable<T> model)
    {
        var recordsTotal = model.Count();
        if (request.Search.Value != "") model = Search(model, request.Search.Value, request.Columns);

        model = Sort(request.Order, request.Columns, model);
        var recordsFiltered = recordsTotal;
        try
        {
            recordsFiltered = model.Count();
        }
        catch (Exception)
        {
            // Ignored
        }

        var data = Paginate(model, request.Start, request.Length);

        return new DatatableResponse<T>
            { Draw = request.Draw, RecordsFiltered = recordsFiltered, RecordsTotal = recordsTotal, Data = data };
    }

    /// <summary>
    ///     Paginate the data
    /// </summary>
    /// <param name="model">The model to paginate</param>
    /// <param name="skip">The number of records to skip</param>
    /// <param name="pageSize">The number of records to take</param>
    /// <returns></returns>
    private static List<T> Paginate(IQueryable<T> model, int skip, int pageSize)
    {
        try
        {
            return model.Skip(skip).Take(pageSize).ToList();
        }
        catch (NullReferenceException)
        {
            return new List<T>();
        }
    }

    /// <summary>
    ///     Sorts the data based on the given columns and directions
    /// </summary>
    /// <param name="orders">List of <see cref="Sort" /> objects</param>
    /// <param name="columns">List of <see cref="Column" /> objects</param>
    /// <param name="model">The model to sort</param>
    /// <returns>The sorted model</returns>
    private static IQueryable<T> Sort(IReadOnlyList<Sort> orders, IReadOnlyList<Column> columns,
        IQueryable<T> model)
    {
        var column = FormatColumnName(columns[orders[0].Column].Data);

        var orderedQueryable = model
            .OrderBy(column + " " + orders[0].Dir);

        for (var i = 1; i < orders.Count; i++)
        {
            column = FormatColumnName(columns[orders[i].Column].Data);
            orderedQueryable = orderedQueryable.ThenBy(column + " " + orders[i].Dir);
        }

        return orderedQueryable;
    }


    /// <summary>
    ///     Search a queryable
    /// </summary>
    /// <param name="model">Queryable to search</param>
    /// <param name="searchValue">The value to search for</param>
    /// <param name="columns">A list of <see cref="Column" /></param>
    /// <returns>A filtered queryable</returns>
    private static IQueryable<T> Search(IQueryable<T> model, StringValues searchValue, List<Column> columns)
    {
        var predicate = PredicateBuilder.New<T>(true);

        predicate = columns.Aggregate(predicate, (current, column) => SearchAColumn(current, column, searchValue));
        return model.AsExpandable().Where(predicate.Compile()).AsQueryable();
    }

    /// <summary>
    ///     Search a column
    /// </summary>
    /// <param name="predicate">The predicate to add to</param>
    /// <param name="column">The column to search</param>
    /// <param name="value">The value to search for</param>
    /// <returns>A predicate with the search added</returns>
    private static ExpressionStarter<T> SearchAColumn(ExpressionStarter<T> predicate, Column column,
        StringValues value
    )
    {
        if (!column.Searchable) return predicate;

        var filterColumn = FormatColumnName(column.Data);
        var searchValue = value.ToString();
        var prop = typeof(T).GetProperty(filterColumn);

        if (prop == null) return predicate;

        return predicate.Or(x => Contains(prop, x, searchValue));
    }

    /// <summary>
    ///    Try to search a column as string
    /// </summary>
    /// <param name="prop">The property to search</param>
    /// <param name="x">The object to search</param>
    /// <param name="searchValue">The value to search for</param>
    /// <returns>True if the value is found, false otherwise</returns>
    private static bool Contains(PropertyInfo prop, T x, string searchValue)
    {
        return prop.GetValue(x) != null && prop.GetValue(x).ToString().ToLower().Contains(searchValue);
    }

    /// <summary>
    ///     Formats a string as a column name.
    /// </summary>
    /// <param name="name">The name to format.</param>
    /// <returns>The formatted name.</returns>
    private static string FormatColumnName(string name)
    {
        var col = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        col = col.Replace(" ", "");
        col = col.Split("_")
            .Aggregate((current, next) => current + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(next));

        return col;
    }
}