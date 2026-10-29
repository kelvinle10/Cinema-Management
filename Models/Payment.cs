using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? TransactionCode { get; set; }

    public DateTime? PaymentTime { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
