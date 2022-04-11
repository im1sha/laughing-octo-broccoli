using Microsoft.EntityFrameworkCore;
using Broccoli.Contexts;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationBuilder configurationBuilder
    = new ConfigurationBuilder().SetBasePath(builder.Environment.ContentRootPath)
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                                .AddEnvironmentVariables();

builder.Configuration.AddConfiguration(configurationBuilder.Build());
// Add services to the container.
string defaultConnectionString;
if (builder.Environment.EnvironmentName == "Development")
{
    defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
    // Use connection string provided at runtime by Heroku.
    string connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    connectionUrl = connectionUrl.Replace("postgres://", string.Empty);
    string userPassSide = connectionUrl.Split("@")[0];
    string hostSide = connectionUrl.Split("@")[1];

    string user = userPassSide.Split(":")[0];
    string password = userPassSide.Split(":")[1];
    string host = hostSide.Split("/")[0];
    string database = hostSide.Split("/")[1].Split("?")[0];

    defaultConnectionString = $"Host={host};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}
builder.Services.AddDbContext<ToDoContext>(options => options.UseNpgsql(defaultConnectionString));
ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
try
{
    var dbContext = serviceProvider.GetRequiredService<ToDoContext>();
    dbContext.Database.Migrate();
}
catch
{
    // ignored
}

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
