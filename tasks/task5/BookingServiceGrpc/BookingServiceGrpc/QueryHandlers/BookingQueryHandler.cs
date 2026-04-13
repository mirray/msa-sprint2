using BookingServiceGrpc.DatabaseContext;
using BookingServiceGrpc.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookingServiceGrpc.QueryHandlers;

public interface IBookingQueryHandler
{
    IEnumerable<Type> SupportedQueries { get; }
    Task<IEnumerable<Dac.Booking>> Handle(GetBookings getBookings, CancellationToken cancellationToken = default);
    Task<Dac.Booking?> Handle(GetBookingById getBookingById, CancellationToken cancellationToken = default);
}

public class BookingQueryHandler(BookingDbContext dbContext) : IBookingQueryHandler
{
    public IEnumerable<Type> SupportedQueries => [typeof(GetBookings), typeof(GetBookingById)];

    public async Task<IEnumerable<Dac.Booking>> Handle(GetBookings getBookings, CancellationToken cancellationToken = default)
    {
        if (getBookings.UserId == null)
        {
            return await dbContext.AllBookings().ToListAsync(cancellationToken);
        }
        return await dbContext.ListBookings(getBookings.UserId).ToListAsync(cancellationToken);

    }
    public async Task<Dac.Booking?> Handle(GetBookingById getBookingById, CancellationToken cancellationToken = default)
    {
        return await dbContext.GetBookingById(getBookingById.Id, cancellationToken);
    }
}