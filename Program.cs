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
    // �@�eՈ����ԭ������ʽҪ�������г�ʽ�a��



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
    // �o���đ��ó�ʽ��δ����׽������ (Unhandled Exception)
    Log.Error(ex, "Something went wrong");
}
finally
{
    Log.CloseAndFlush(); // �ǳ���Ҫ��һ�Σ�
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