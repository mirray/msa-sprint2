using BookingServiceGrpc.Queries;
using BookingServiceGrpc.Queries.UserService;

namespace BookingServiceGrpc.QueryHandlers.UserService;

public class RestQueryHandler(string host) : IQueryHandler
{
    private readonly Uri _baseHost = new(host);

    public IEnumerable<Type> SupportedQueries => [typeof(IsUserActive), typeof(IsUserBlacklisted), typeof(GetUserStatus)];

    public async Task<bool> Handle(IQuery<bool> query, CancellationToken  cancellationToken = default)
    {
        using var client = new HttpClient();
        return await client.GetFromJsonAsync<bool?>(BuildEntryPoint(query), cancellationToken: cancellationToken) ?? throw new InvalidOperationException();
    }
    public async Task<string> Handle(IQuery<string> query, CancellationToken  cancellationToken = default)
    {
        using var client = new HttpClient();
        return await client.GetStringAsync(BuildEntryPoint(query), cancellationToken: cancellationToken) ?? throw new InvalidOperationException();
    }

    public Task<T> Handle<T>(IQuery<T> query, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    private Uri BuildEntryPoint<T>(IQuery<T> query)
    {
        switch (query)
        {
            case IsUserActive isUserActive:
                return new Uri(_baseHost, $"{isUserActive.UserId}/active");
            case IsUserBlacklisted blacklisted:
                return new Uri(_baseHost, $"{blacklisted.UserId}/blacklisted");
            case GetUserStatus getUserStatus:
                return new Uri(_baseHost, $"{getUserStatus.UserId}/status");
        }
        throw new NotSupportedException($"{query.GetType().Name} is not supported"); 
    }
}