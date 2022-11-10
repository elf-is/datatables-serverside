using DataTablesTest.Data;
using DataTablesTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataTablesTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataTableController : ControllerBase
{
    [HttpPost]
    public ActionResult GetDataTableData([FromBody] DatatableRequest request,
        [FromServices] DataTableService<Customer> service)
    {
        var model = service.GetData();
        var response = service.GetDatatableObject(request, model);
        return Ok(response);
    }
}