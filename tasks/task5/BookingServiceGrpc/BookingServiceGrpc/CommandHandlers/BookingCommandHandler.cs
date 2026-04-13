using BookingServiceGrpc.Commands;
using BookingServiceGrpc.DatabaseContext;

namespace BookingServiceGrpc.CommandHandlers;

public interface IBookingCommandHandler
{
    IEnumerable<Type> SupportedCommands { get; }
    Task<long> Handle(CreateBooking command, CancellationToken cancellationToken);
}

public class BookingCommandHandler(BookingDbContext dbContext) : IBookingCommandHandler
{
    public IEnumerable<Type> SupportedCommands => [typeof(CreateBooking)];

    public async Task<long> Handle(CreateBooking command, CancellationToken cancellationToken)
    {
        return await dbContext.CreateBooking(command.UserId, command.HotelId, command.PromoCode,
            command.Discount, command.FinalPrice, cancellationToken);

    }
}