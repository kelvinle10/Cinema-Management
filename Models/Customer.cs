using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
