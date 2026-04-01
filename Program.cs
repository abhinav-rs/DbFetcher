using DbFetcher.Data;
using DbFetcher.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read connection string from environment variable, fall back to configuration
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("MongoDB connection string is not configured");

// Register MongoDB context
builder.Services.AddSingleton<InvoiceDbContext>(new InvoiceDbContext(mongoConnectionString));

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

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "DbFetcher API v1");
    options.RoutePrefix = "swagger";
});
app.UseCors("LocalNetwork");
app.UseAuthorization();
app.MapControllers();
app.Run();