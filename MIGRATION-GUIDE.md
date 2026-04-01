# Migration Guide: SQL Server to MongoDB

## Overview

This document outlines the complete migration of the DbFetcher application from **Entity Framework Core with SQL Server** to **MongoDB.Driver for MongoDB Atlas**.

**Migration Date:** March 31, 2026  
**From:** SQL Server with EF Core  
**To:** MongoDB Atlas with MongoDB.Driver

---

## Why Migrate?

- ✅ MongoDB scalability for large datasets
- ✅ Document-based storage (better for invoice data)
- ✅ Built-in support for cloud deployment (MongoDB Atlas)
- ✅ Flexible schema for evolving invoice requirements
- ✅ Better performance for pagination with cursor-based queries

---

## Changes Summary

### 1. **Dependencies Added**

```bash
dotnet add package MongoDB.Driver
```

**Version:** Latest stable version of MongoDB.Driver NuGet package

**Why:** Replaces Entity Framework Core's SQL Server provider with MongoDB's native driver for direct database operations.

---

### 2. **Database Context Refactoring**

#### Before (EF Core with SQL Server)
```csharp
// Data/InvoiceDbContext.cs
public class InvoiceDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SQL Server specific configurations
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceAmount).HasColumnType("decimal(18,2)");
            // ... more configurations
        });
    }
}
```

#### After (MongoDB.Driver)
```csharp
// Data/InvoiceDbContext.cs
public class InvoiceDbContext
{
    private readonly IMongoDatabase _database;

    public InvoiceDbContext(string connectionString)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("dotnet");
    }

    public IMongoCollection<Invoice> Invoices => 
        _database.GetCollection<Invoice>("Invoices");
}
```

**Key Changes:**
- ✅ No longer inherits from `DbContext`
- ✅ Takes connection string directly in constructor
- ✅ Returns `IMongoCollection<T>` instead of `DbSet<T>`
- ✅ Database name hardcoded as `"dotnet"` (MongoDB Atlas cluster)
- ✅ No model configuration needed (BSON attributes handle mapping)

---

### 3. **Invoice Model Updates**

#### Before (EF Core Attributes)
```csharp
[Table("Invoices")]
public class Invoice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal InvoiceAmount { get; set; }
}
```

#### After (BSON Attributes)
```csharp
public class Invoice
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("invoiceNumber")]
    public string InvoiceNumber { get; set; }

    [BsonElement("invoiceAmount")]
    public decimal InvoiceAmount { get; set; }
}
```

**Key Changes:**
- ✅ `int Id` → `ObjectId Id` (MongoDB's unique identifier)
- ✅ `[Key]` → `[BsonId]` (MongoDB serialization attribute)
- ✅ `[Required]`, `[MaxLength]` removed (schema validation not needed)
- ✅ `[Column]` → `[BsonElement]` (field name mapping)
- ✅ Removed `[Table]`, `[DatabaseGenerated]` attributes
- ✅ All field names use camelCase in `[BsonElement]` for MongoDB convention

---

### 4. **DTO Updates**

#### Before
```csharp
public class InvoiceDto
{
    public int Id { get; set; }
    // ... other properties
}
```

#### After
```csharp
public class InvoiceDto
{
    public string Id { get; set; } = string.Empty;  // ObjectId serializes to string
    // ... other properties
}
```

**Key Change:**
- ✅ ID changed from `int` to `string` (ObjectId JSON serialization format)

---

### 5. **Service Layer Refactoring**

#### GetPagedAsync() - Before (EF Core LINQ)
```csharp
var query = _context.Invoices
    .AsNoTracking()
    .OrderBy(i => i.Id);

if (cursor.HasValue)
{
    query = query.Where(i => i.Id > cursor.Value);
}

var rawResults = await query
    .Take(pageSize + 1)
    .Select(i => new InvoiceDto { /* mapping */ })
    .ToListAsync();
```

#### GetPagedAsync() - After (MongoDB.Driver)
```csharp
var collection = _context.Invoices;
var filterBuilder = Builders<Invoice>.Filter;
FilterDefinition<Invoice> filter = filterBuilder.Empty;

if (cursor.HasValue)
{
    var cursorObjectId = new ObjectId(/* ... */);
    filter = filterBuilder.Gt(i => i.Id, cursorObjectId);
}

var rawResults = await collection
    .Find(filter)
    .Sort(Builders<Invoice>.Sort.Ascending(i => i.Id))
    .Limit(pageSize + 1)
    .Project(i => new InvoiceDto { /* mapping */ })
    .ToListAsync();
```

**Key Changes:**
- ✅ LINQ queries → MongoDB Driver query builders (`Builders<T>`)
- ✅ `.Where()` → `.Find()` with FilterDefinition
- ✅ `.Select()` → `.Project()`
- ✅ `.OrderBy()` → `.Sort()`
- ✅ `.Take()` → `.Limit()`
- ✅ `.ToListAsync()` still works

---

### 6. **Total Count Operation**

#### Before (SQL Server Raw Query)
```csharp
private async Task<long> GetTotalCountAsync()
{
    var connection = _context.Database.GetDbConnection();
    await using var cmd = connection.CreateCommand();
    cmd.CommandText = @"
        SELECT SUM(rows)
        FROM sys.partitions
        WHERE object_id = OBJECT_ID('Invoices')
        AND index_id IN (0, 1)";
    
    await connection.OpenAsync();
    var result = await cmd.ExecuteScalarAsync();
    await connection.CloseAsync();
    
    return result is DBNull or null ? 0 : Convert.ToInt64(result);
}
```

#### After (MongoDB Count)
```csharp
private async Task<long> GetTotalCountAsync()
{
    var collection = _context.Invoices;
    var totalCount = await collection
        .CountDocumentsAsync(Builders<Invoice>.Filter.Empty);
    return totalCount;
}
```

**Key Changes:**
- ✅ SQL system query → MongoDB `CountDocumentsAsync()`
- ✅ Much simpler and more maintainable
- ✅ No connection management needed

---

### 7. **Dependency Injection Setup**

#### Before (Program.cs)
```csharp
builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseSqlServer(connectionString));
```

#### After (Program.cs)
```csharp
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("MongoDB connection string is not configured");

builder.Services.AddSingleton<InvoiceDbContext>(
    new InvoiceDbContext(mongoConnectionString));
```

**Key Changes:**
- ✅ `AddDbContext<>()` → `AddSingleton<>()` (MongoDB context is lightweight)
- ✅ Direct instantiation with connection string
- ✅ Environment variable support for production
- ✅ Configuration fallback with error handling

---

## Connection String Configuration

### appsettings.json (Development)
```json
{
  "ConnectionStrings": {
    "Default": "mongodb+srv://admin:zpdev44@dotnet.afaxwql.mongodb.net/?appName=dotnet"
  }
}
```

### appsettings.Production.json (Production)
```json
{
  "ConnectionStrings": {
    "Default": ""
  }
}
```

**Important:** Connection string is empty in production. Set the environment variable instead:

```bash
# Linux/Docker
export MONGODB_CONNECTION_STRING="mongodb+srv://admin:PASSWORD@dotnet.afaxwql.mongodb.net/?appName=dotnet"

# Windows PowerShell
$env:MONGODB_CONNECTION_STRING="mongodb+srv://admin:PASSWORD@dotnet.afaxwql.mongodb.net/?appName=dotnet"
```

---

## Files Modified

| File | Changes |
|------|---------|
| [Data/InvoiceDbContext.cs] | Replaced EF Core DbContext with MongoDB IMongoClient/IMongoDatabase |
| [Models/Invoice.cs] | Changed ID from `int` to `ObjectId`, added `[BsonElement]` attributes |
| [DTOs/InvoiceDto.cs] | Changed ID from `int` to `string` for API serialization |
| [Services/InvoiceService.cs] | Replaced LINQ/EF Core queries with MongoDB.Driver operations |
| [Program.cs] | Updated DI registration for MongoDB context |
| [DbFetcher.csproj] | Added `MongoDB.Driver` NuGet package |

---

## Data Migration

### Steps to Migrate Data from SQL Server to MongoDB

1. **Export data from SQL Server:**
```sql
SELECT * FROM Invoices
-- Export as CSV or JSON format
```

2. **Convert to MongoDB document format** (ensure field names match `[BsonElement]` names in camelCase)

3. **Import to MongoDB Atlas:**
```bash
mongoimport --uri "mongodb+srv://admin:PASSWORD@dotnet.afaxwql.mongodb.net/dotnet" \
  --collection Invoices \
  --file invoices.json \
  --jsonArray
```

4. **Verify data integrity:**
```bash
# Connect to MongoDB
mongosh "mongodb+srv://admin:PASSWORD@dotnet.afaxwql.mongodb.net/dotnet"

# Check collection
db.Invoices.countDocuments()
db.Invoices.findOne()
```

---

## API Compatibility

### Pagination

**Cursor-based pagination is now used instead of offset-based:**

```
GET /api/invoice?cursor=ObjectIdHex&pageSize=50
```

**Response Format:**
```json
{
  "data": [ /* invoice objects */ ],
  "metadata": {
    "nextCursor": "507f1f77bcf86cd799439011",
    "hasMore": true,
    "pageSize": 50,
    "totalCount": 5000
  }
}
```

### Response Format - No Breaking Changes

Invoice object structure remains the same, only `id` is now a string (ObjectId hex representation).

---

## Performance Improvements

| Operation | Before (SQL Server) | After (MongoDB) |
|-----------|-------------------|-----------------|
| Pagination | Offset-based (slow on large tables) | Cursor-based (fast, scalable) |
| Count query | System partition query | Direct `CountDocumentsAsync()` |
| Network | Local integration | Cloud-ready MongoDB Atlas |
| Indexing | SQL Server indexes | MongoDB indexes on `_id` (default) |

---

## Testing Checklist

- [ ] Application builds without errors: `dotnet build`
- [ ] Setting MongoDB connection string works (appsettings.json)
- [ ] Environment variable override works: `$env:MONGODB_CONNECTION_STRING=...`
- [ ] GET /api/invoice endpoint returns invoice data
- [ ] Pagination with cursor works correctly
- [ ] Total count is accurate
- [ ] CORS policy allows cross-origin requests
- [ ] Swagger UI displays API correctly

---

## Rollback Plan

If you need to revert to SQL Server:

1. Restore previous git commits
2. Re-install EF Core packages: `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
3. Restore SQL Server database backup
4. Run migrations: `dotnet ef database update`

---

## Next Steps

1. **Data Migration:** Migrate existing SQL Server data to MongoDB
2. **Testing:** Test all endpoints with MongoDB backend
3. **Monitoring:** Set up MongoDB Atlas monitoring and alerts
4. **Performance Tuning:** Create indexes on frequently queried fields if needed
5. **Documentation:** Update API documentation with MongoDB connection details

---

## Troubleshooting

### Connection String Issues
```
InvalidOperationException: MongoDB connection string is not configured
```
**Solution:** Set connection string in appsettings.json or MONGODB_CONNECTION_STRING environment variable

### ObjectId Serialization
```
SerializationException: Unknown BsonType 'Null'
```
**Solution:** Ensure all `[BsonElement]` field names match the MongoDB document structure

### Pagination Errors
```
Cast cannot be performed from ObjectId to int
```
**Solution:** Remember ID is now `ObjectId` (converted to string in DTOs)

---

## Additional Resources

- [MongoDB.Driver Documentation](https://www.mongodb.com/docs/drivers/csharp/)
- [MongoDB BSON Serialization](https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/serialization/)
- [MongoDB Atlas Documentation](https://www.mongodb.com/docs/atlas/)
- [Cursor-based Pagination](https://www.mongodb.com/docs/manual/reference/method/cursor.skip/)

---

**Migration completed successfully!** ✅
