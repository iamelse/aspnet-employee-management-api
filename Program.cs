using EmployeeManagementApi;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Services;
using EmployeeManagementApi.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Konfigurasi Sumber Konfigurasi (appsettings, env, secrets)

// Tambahkan konfigurasi dari appsettings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Tambahkan User Secrets jika di environment Development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Tambahkan environment variables (untuk production atau CI/CD)
builder.Configuration.AddEnvironmentVariables();

#endregion

#region Konfigurasi Layanan Aplikasi

// Ambil konfigurasi database secara terstruktur
var dbSettingsSection = builder.Configuration.GetSection("DatabaseSettings");
var provider = dbSettingsSection["DefaultProvider"] ?? throw new Exception("Database provider not configured");

// Ambil connection string sesuai provider
string GetConnectionString()
{
    var connStr = provider switch
    {
        "MySQL" => dbSettingsSection.GetSection("MySQL")["ConnectionString"],
        "SQLite" => dbSettingsSection.GetSection("SQLite")["ConnectionString"],
        _ => throw new Exception($"Database provider '{provider}' not supported")
    };

    return connStr ?? throw new Exception($"Connection string for provider '{provider}' is not configured.");
}

var connectionString = GetConnectionString();

// Registrasi DbContext dengan provider dan connection string yang sesuai
builder.Services.AddDbContext<AppDbContext>(options =>
{
    switch (provider)
    {
        case "MySQL":
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            break;
        case "SQLite":
            options.UseSqlite(connectionString);
            break;
        default:
            throw new Exception($"Database provider '{provider}' not supported");
    }
});

// Registrasi konfigurasi enkripsi dan service terkait sebagai singleton
builder.Services.Configure<EncryptionSettings>(
    builder.Configuration.GetSection("Encryption"));
builder.Services.AddSingleton<EncryptionService>();

// Registrasi layanan Web API (controller)
builder.Services.AddControllers();

// Registrasi Swagger/OpenAPI untuk dokumentasi API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

#region Konfigurasi Middleware dan Pipeline HTTP

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var encryptionService = services.GetRequiredService<EncryptionService>(); // ✅ Tambahkan ini

        // Terapkan migrasi otomatis
        context.Database.Migrate();

        // Jalankan seeder data
        await SeedData.Initialize(context, encryptionService); // ✅ Perbaiki pemanggilan
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error during migration or seeding database");
        throw;
    }
}

app.Run();