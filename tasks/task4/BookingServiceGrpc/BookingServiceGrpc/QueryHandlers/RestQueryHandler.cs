using BookingServiceGrpc.Queries;

namespace BookingServiceGrpc.QueryHandlers;

public abstract class RestQueryHandlerBase:IQueryHandler
{
    public abstract IEnumerable<Type> SupportedQueries { get; }
    public virtual async Task<bool> Handle(IQuery<bool> query, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        return await client.GetFromJsonAsync<bool?>(BuildEntryPoint(query), cancellationToken: cancellationToken) ?? throw new InvalidOperationException();
    }

    public virtual async Task<string> Handle(IQuery<string> query, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        return await client.GetStringAsync(BuildEntryPoint(query), cancellationToken: cancellationToken) ?? throw new InvalidOperationException();
    }

    public virtual async Task<T> Handle<T>(IQuery<T> query, CancellationToken cancellationToken = default) where T : class
    {
        using var client = new HttpClient();
        return await client.GetFromJsonAsync<T>(BuildEntryPoint(query), cancellationToken: cancellationToken) ?? throw new InvalidOperationException();
    }

    protected abstract Uri BuildEntryPoint<T>(IQuery<T> query);
}