using System.Globalization;
using System.Linq.Dynamic.Core;
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

    public IQueryable<T> GetDataTable()
    {
        return _context.Set<T>().AsQueryable().AsNoTracking();
    }

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
        var filterColumn = FormatColumnName(column.Data);
        var searchValue = value.ToString();
        var prop = typeof(T).GetProperty(filterColumn);
        if (prop == null) return predicate;
        // search all types as string
        return predicate.Or(x =>
            prop.GetValue(x)!.ToString()!.Contains(searchValue));
    }

    /// <summary>
    ///     Formats a string as a column name.
    /// </summary>
    /// <param name="name">The name to format.</param>
    /// <returns>The formatted name.</returns>
    public static string FormatColumnName(string name)
    {
        var col = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        col = col.Replace(" ", "");
        col = col.Split("_")
            .Aggregate((current, next) => current + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(next));

        return col;
    }
}