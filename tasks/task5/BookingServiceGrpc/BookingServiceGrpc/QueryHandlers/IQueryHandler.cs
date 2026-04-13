using BookingServiceGrpc.Queries;

namespace BookingServiceGrpc.QueryHandlers;

public interface IQueryHandler
{
    public IEnumerable<Type> SupportedQueries { get; }
    public Task<bool> Handle(IQuery<bool> query, CancellationToken cancellationToken = default);
    public Task<string> Handle(IQuery<string> query, CancellationToken cancellationToken = default);
    public Task<T> Handle<T>(IQuery<T> query, CancellationToken cancellationToken = default) where T:class;
}