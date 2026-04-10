using BookingServiceGrpc.Queries;
using BookingServiceGrpc.Queries.ReviewService;

namespace BookingServiceGrpc.QueryHandlers.ReviewService;

public class RestQueryHandler(string host) : RestQueryHandlerBase
{
    private readonly Uri _baseHost = new(host);

    public override IEnumerable<Type> SupportedQueries => [typeof(IsTrustedHotel)];

    protected override Uri BuildEntryPoint<T>(IQuery<T> query)
    {
        switch (query)
        {
            case IsTrustedHotel trustedHotel:
                return new Uri(_baseHost, $"hotel/{trustedHotel.HotelId}/trusted");
        }
        throw new NotSupportedException($"{query.GetType().Name} is not supported"); 
    }
}