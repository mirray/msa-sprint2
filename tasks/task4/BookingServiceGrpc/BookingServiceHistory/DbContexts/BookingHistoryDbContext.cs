using Microsoft.EntityFrameworkCore;

namespace BookingServiceHistory.DbContexts;

public class BookingHistoryDbContext: DbContext
{
    public BookingHistoryDbContext(DbContextOptions<BookingHistoryDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Dac.BookingHistory> Bookings { get; set; } = null!;
    
    public async Task<long> CreateHistoryBooking(string? userId, string? hotelId, string? promoCode, double? discount, double? finalPrice, DateTime? createdAt, CancellationToken cancellationToken)
    {
        var booking = new Dac.BookingHistory
        {
            UserId = userId,
            HotelId = hotelId,
            DiscountPercent = discount,
            PromoCode = promoCode,
            Price = finalPrice,
            CreatedAt = createdAt,
        };
        await Bookings.AddAsync(booking, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return booking.HistoryId;
    }

}