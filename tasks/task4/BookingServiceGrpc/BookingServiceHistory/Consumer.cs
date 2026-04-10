using System.Text.Json;
using BookingServiceHistory.CommandHandlers;
using BookingServiceHistory.Commands;
using BookingServiceHistory.Dac;
using Confluent.Kafka;

namespace BookingServiceHistory;

public class Consumer(ConsumerConfig config, ICreateHistoryCommandHandler handler, ILogger<Consumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("BookingCreated"); 
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            Console.WriteLine($"Received message: {consumeResult.Message.Value} at: '{consumeResult.TopicPartitionOffset}'.");
            var booking = JsonSerializer.Deserialize<Booking>(consumeResult.Message.Value);
            if (booking == null)
            {
                logger.LogError("Booking is null");
                continue;
            }
            await handler.Handle(new CreateHistoryBooking
            {
                UserId = booking.UserId,
                HotelId = booking.HotelId,
                PromoCode = booking.PromoCode,
                Discount = booking.DiscountPercent,
                FinalPrice = booking.Price,
                CreateAt = booking.CreatedAt
            }, stoppingToken);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
        }
    }
}