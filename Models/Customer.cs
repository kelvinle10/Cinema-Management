using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
