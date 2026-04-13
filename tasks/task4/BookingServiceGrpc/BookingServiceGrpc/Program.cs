using BookingServiceGrpc.CommandHandlers;
using BookingServiceGrpc.DatabaseContext;
using BookingServiceGrpc.Producers;
using BookingServiceGrpc.Queries;
using BookingServiceGrpc.QueryHandlers;
using BookingServiceGrpc.Services;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
var port = Environment.GetEnvironmentVariable("DATABASE_PORT");
var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");
var connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={dbName};GSS Encryption Mode=Disable";
builder.Services.AddGrpc();

builder.Services.AddDbContext<BookingDbContext>(options => options.UseNpgsql(connectionString));
//Kafka
builder.Services.AddSingleton<ProducerConfig>(_ => new ProducerConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_HOST")// Address of your Kafka broker
});
builder.Services.AddSingleton<IKafkaProducer,KafkaProducer>();
//CQRS

builder.Services.AddTransient<IQueryHandler>(_ => 
    new BookingServiceGrpc.QueryHandlers.HotelService.RestQueryHandler(
        Environment.GetEnvironmentVariable("HOTEL_HOST") ?? throw new Exception("Provide HOTEL_HOST variable")));

builder.Services.AddTransient<IQueryHandler>(_ => 
    new BookingServiceGrpc.QueryHandlers.PromoCodeService.RestQueryHandler(
        Environment.GetEnvironmentVariable("PROMO_CODE_HOST") ?? throw new Exception("Provide PROMO_CODE_HOST variable")));

builder.Services.AddTransient<IQueryHandler>(_ => 
    new BookingServiceGrpc.QueryHandlers.ReviewService.RestQueryHandler(
        Environment.GetEnvironmentVariable("REVIEW_HOST") ?? throw new Exception("Provide REVIEW_HOST variable")));

builder.Services.AddTransient<IQueryHandler>(_ => 
    new BookingServiceGrpc.QueryHandlers.UserService.RestQueryHandler(
        Environment.GetEnvironmentVariable("USER_API_HOST") ?? throw new Exception("Provide USER_API_HOST variable")));

builder.Services.AddSingleton<IBookingQueryHandler>(c => 
    new BookingQueryHandler(c.GetService<BookingDbContext>() ?? throw new InvalidOperationException()));

builder.Services.AddSingleton<IBookingCommandHandler>(c => 
    new BookingCommandHandler(c.GetService<BookingDbContext>() ?? throw new InvalidOperationException()));

builder.Services.AddSingleton<Dictionary<Type, IQueryHandler>>(sp =>
{
    var services = sp.GetServices<IQueryHandler>().ToArray();
    var result = new Dictionary<Type, IQueryHandler>();
    foreach (var handler in services)
    {
        foreach (var query in handler.SupportedQueries)
        {
            result[query]= handler;    
        }
        
    }
    return result; 
});
builder.Services.AddSingleton<IRestQueryHandlerProvider, RestQueryHandlerProvider>();

var featureX = bool.TryParse(Environment.GetEnvironmentVariable("ENABLE_FEATURE_X"), out var enableFeatureX) && enableFeatureX;

var app = builder.Build();


app.MapGrpcService<BookingService>().AllowAnonymous();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.Migrate();
}
app.MapGet("/ping", () => "pong");
if (featureX)
{
    app.MapGet("/feature", () => "Feature X is enabled!");
}

app.Run();