using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookingServiceHistory.Dac;

[PrimaryKey(nameof(HistoryId))]
[Table("booking_history")]
public class BookingHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("history_id")]public int HistoryId { get; set; }
    [Column("user_id")] public string? UserId { get; set; }
    [Column("hotel_id")] public string? HotelId { get; set; }
    [Column("promo_code")] public string? PromoCode { get; set; }
    [Column("discount_percent")] public double? DiscountPercent { get; set; }
    [Column("price")] public double? Price { get; set; }
    [Column("created_at",TypeName = "date")] public DateTime? CreatedAt { get; set; }
}