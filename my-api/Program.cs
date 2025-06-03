using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.UseHttpClientMetrics(); 
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMetricServer();
app.UseHttpMetrics();

app.MapGet("/current-time", () =>
{
    var utcNow = DateTime.UtcNow;
    var localNow = DateTime.Now;
    string greeting = localNow.Hour switch
    {
        >= 5 and < 12 => "Bom dia!",
        >= 12 and < 18 => "Boa tarde!",
        _ => "Boa noite!"
    };

    return Results.Ok(new
    {
        greeting,
        utcNow = utcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
        localNow = localNow.ToString("yyyy-MM-dd HH:mm:ss"),
        timezone = TimeZoneInfo.Local.DisplayName
    });
});

app.Run();