using DataTablesTest.Data;
using DataTablesTest.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(
    options => options.AddDefaultPolicy(
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition", "Content-Length")
    )
);
// Add services to the container.
builder.Services.RegisterDataServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddScoped<DataTableService<Customer>>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();
app.CreateDbIfNotExists();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();