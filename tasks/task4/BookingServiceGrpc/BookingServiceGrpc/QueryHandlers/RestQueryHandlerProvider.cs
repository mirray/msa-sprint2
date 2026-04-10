using BookingServiceGrpc.Queries;

namespace BookingServiceGrpc.QueryHandlers;

public class RestQueryHandlerProvider(Dictionary<Type, IQueryHandler> handlers):IRestQueryHandlerProvider
{
    public async Task<string> Handle(IQuery<string> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        if (!handlers.TryGetValue(queryType, out var handler))
        {
            throw new NotSupportedException($"{queryType.Name} is not supported");
        }
        return await handler.Handle(query, cancellationToken);
    }
    public async Task<bool> Handle(IQuery<bool> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        if (!handlers.TryGetValue(queryType, out var handler))
        {
            throw new NotSupportedException($"{queryType.Name} is not supported");
        }
        return await handler.Handle(query, cancellationToken);
    }
    public async Task<T> Handle<T>(IQuery<T> query, CancellationToken cancellationToken = default) where T : class
    {
        var queryType = query.GetType();
        if (!handlers.TryGetValue(queryType, out var handler))
        {
            throw new NotSupportedException($"{queryType.Name} is not supported");
        }
        return await handler.Handle(query, cancellationToken);
    }
}