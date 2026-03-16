using DbFetcher.Data;
using DbFetcher.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));


builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalNetwork", policy =>
    {
        policy.AllowAnyOrigin()   
              .AllowAnyMethod()   
              .AllowAnyHeader(); 
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DbFetcher API v1");
        options.RoutePrefix = "swagger";
    });
}
app.UseCors("LocalNetwork");
app.UseAuthorization();
app.MapControllers();
app.Run();