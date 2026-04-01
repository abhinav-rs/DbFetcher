using DbFetcher.Models;
using MongoDB.Driver;

namespace DbFetcher.Data;

public class InvoiceDbContext
{
    private readonly IMongoDatabase _database;

    public InvoiceDbContext(string connectionString)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("dotnet");
    }

    public IMongoCollection<Invoice> Invoices => _database.GetCollection<Invoice>("Invoices");
}
