using BookingServiceGrpc.Queries;
using BookingServiceGrpc.Queries.HotelService;

namespace BookingServiceGrpc.QueryHandlers.HotelService;

public class RestQueryHandler(string host) : RestQueryHandlerBase
{
    private readonly Uri _baseHost = new(host);

    public override IEnumerable<Type> SupportedQueries => [typeof(IsHotelFullyBooked), typeof(IsHotelOperational)];

    protected override Uri BuildEntryPoint<T>(IQuery<T> query)
    {
        switch (query)
        {
            case  IsHotelFullyBooked fullyBooked:
                return new Uri(_baseHost, $"{fullyBooked.HotelId}/fully-booked");

            case IsHotelOperational operational:
                return new Uri(_baseHost, $"{operational.HotelId}/operational");
        }
        throw new NotSupportedException($"{query.GetType().Name} is not supported"); 
    }
}