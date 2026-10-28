using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Genre { get; set; }

    public int Duration { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? AgeRating { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
