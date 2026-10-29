using System;
using System.Collections.Generic;

namespace CinemaManagementSystem.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Genre { get; set; }

    public int Duration { get; set; }

    public string? AgeRating { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? PosterUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
