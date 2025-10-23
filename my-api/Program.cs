using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using my_api.Data;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.UseHttpClientMetrics();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Initialize database and tables automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var retryCount = 0;
    const int maxRetries = 30; // Increased retry attempts
    const int retryDelaySeconds = 2;

    while (retryCount < maxRetries)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database... Attempt {Retry} of {MaxRetries}", retryCount + 1, maxRetries);
            
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Create the database to ensure clean state
            context.Database.EnsureCreated();
            
            // Verify connection
            if (context.Database.CanConnect())
            {
                logger.LogInformation("Database connection successful and tables created/verified!");
                break;
            }
        }
        catch (Exception ex)
        {
            retryCount++;
            
            if (retryCount == maxRetries)
            {
                logger.LogError(ex, "Failed to connect to database after {MaxRetries} attempts.", maxRetries);
                throw;
            }

            logger.LogWarning(ex, "Connection error. Retrying in {Delay} seconds... Attempt {Retry} of {MaxRetries}",
                retryDelaySeconds, retryCount, maxRetries);
            Thread.Sleep(5000);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMetricServer();
app.UseHttpMetrics();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();