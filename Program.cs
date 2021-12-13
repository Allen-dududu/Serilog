using Serilog;
using Serilog.Formatting.Compact;

var config = GetConfiguration();
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MyApplication")
        .Enrich.WithProperty("Environment", "Environment")
    .WriteTo.File(new CompactJsonFormatter(), "logs/myapp.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Console(outputTemplate:
        "{Timestamp:HH:mm:ss} [{EventType:x8} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    // 這裡請放你原本主程式要寫的所有程式碼！



    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.WebHost.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseSerilogRequestLogging(); // <-- Add this line
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // 紀錄你的應用程式中未被捕捉的例外 (Unhandled Exception)
    Log.Error(ex, "Something went wrong");
}
finally
{
    Log.CloseAndFlush(); // 非常重要的一段！
}



IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    var config = builder.Build();

    return builder.Build();
}