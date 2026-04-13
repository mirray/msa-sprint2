using BookingServiceGrpc.Queries;
using BookingServiceGrpc.Queries.PromoCodeService;

namespace BookingServiceGrpc.QueryHandlers.PromoCodeService;

public class RestQueryHandler(string host) : RestQueryHandlerBase
{
    private readonly Uri _baseHost = new(host);

    public override IEnumerable<Type> SupportedQueries => [typeof(Validate)];


    public override async Task<T> Handle<T>(IQuery<T> query, CancellationToken cancellationToken = default) where T : class
    {
        using var client = new HttpClient();
        var response = await client.PostAsync(BuildEntryPoint(query), null, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken) ?? throw new Exception("Not valid response");
        }

        throw new Exception("Not valid response");
    }

    protected override Uri BuildEntryPoint<T>(IQuery<T> query)
    {
        switch (query)
        {
            case Validate validate:
                return new Uri(_baseHost,$"validate?code={validate.Code}&userId={validate.UserId}");
        }
        throw new NotSupportedException($"{query.GetType().Name} is not supported"); 
    }
}