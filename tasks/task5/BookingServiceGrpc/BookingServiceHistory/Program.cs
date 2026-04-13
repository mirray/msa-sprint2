using BookingServiceHistory;
using BookingServiceHistory.CommandHandlers;
using BookingServiceHistory.DbContexts;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
var port = Environment.GetEnvironmentVariable("DATABASE_PORT");
var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");
var connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={dbName};GSS Encryption Mode=Disable";

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<BookingHistoryDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddSingleton<ICreateHistoryCommandHandler>(c => new CreateHistoryCommandHandler(c.GetService<BookingHistoryDbContext>() ?? throw new InvalidOperationException()));
builder.Services.AddSingleton<ConsumerConfig>(_ => new ConsumerConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_HOST"),
    GroupId = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP"),
    AutoOffsetReset = AutoOffsetReset.Earliest
});
builder.Services.AddHostedService<Consumer>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingHistoryDbContext>();
    db.Database.Migrate();
}
app.Run();