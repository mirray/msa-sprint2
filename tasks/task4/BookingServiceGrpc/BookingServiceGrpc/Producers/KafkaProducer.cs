using System.Text.Json;
using Confluent.Kafka;

namespace BookingServiceGrpc.Producers;

public interface IKafkaProducer
{
    Task ProduceMessageBookingCreated( Dac.Booking booking, CancellationToken cancellationToken);
}

public class KafkaProducer(ProducerConfig config, ILogger<KafkaProducer> logger) : IKafkaProducer
{
    public async Task ProduceMessageBookingCreated(Dac.Booking booking, CancellationToken cancellationToken)
    {
        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            try
            {
                var message = new Message<Null, string> { Value =  JsonSerializer.Serialize(booking) };

                // ProduceAsync is recommended for high throughput and reliability
                var result = await producer.ProduceAsync("BookingCreated", message, cancellationToken);

                Console.WriteLine($"Delivered to: {result.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}