using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int ShowtimeId { get; set; }

    public int SeatId { get; set; }

    public int CustomerId { get; set; }

    public decimal Price { get; set; }

    public DateTime BookingTime { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;

    public virtual Showtime Showtime { get; set; } = null!;
}
