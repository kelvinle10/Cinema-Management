
using CinemaManagementSystem.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Customer
{
    public partial class MovieDetailsWindow : Window
    {
        private readonly int movieId;
        private readonly int customerId;
        public MovieDetailsWindow(int movieId, int customerId)
        {
            InitializeComponent();

            this.movieId = movieId;
            this.customerId = customerId;

            LoadMovieDetails();
            LoadShowtimes();
        }

        private void LoadMovieDetails()
        {
            using (DBContext db = new DBContext())
            {
                Movie? movie = db.Movies
                    .FirstOrDefault(m => m.MovieId == movieId);

                if (movie == null)
                {
                    MessageBox.Show("Movie not found!");
                    Close();
                    return;
                }

                imgPoster.Source = !string.IsNullOrEmpty(movie.PosterUrl)
                    ? new System.Windows.Media.Imaging.BitmapImage(
                        new Uri(movie.PosterUrl))
                    : null;

                txtTitle.Text = movie.Title;
                txtGenre.Text = movie.Genre ?? "Unknown Genre";
                txtDuration.Text = movie.Duration + " minutes";
                txtAgeRating.Text = "Age Rating: " + (movie.AgeRating ?? "N/A");
                txtReleaseDate.Text = movie.ReleaseDate.HasValue
                    ? "Release Date: " + movie.ReleaseDate.Value.ToString("dd/MM/yyyy")
                    : "Release Date: N/A";

                txtDescription.Text = movie.Description ?? "No description available.";
            }
        }

        private void LoadShowtimes()
        {
            using (DBContext db = new DBContext())
            {
                var showtimes = db.Showtimes
                    .Where(s => s.MovieId == movieId)
                    .OrderBy(s => s.StartTime)
                    .Select(s => new
                    {
                        s.ShowtimeId,
                        RoomName = s.Room.RoomName,
                        s.StartTime,
                        s.EndTime,
                        s.TicketPrice,
                        ShowtimeStatus = s.Status,
                        RoomStatus = s.Room.IsActive ? "Active" : "Inactive"
                    })
                    .ToList();

                dgShowtimes.ItemsSource = showtimes;

                txtNoShowtimes.Visibility = showtimes.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnSelectSeats_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int showtimeId = (int)button.Tag;

            SeatSelectionWindow seatWindow =
                new SeatSelectionWindow(showtimeId, customerId);

            seatWindow.Owner = this;
            seatWindow.ShowDialog();
        }
    }
}
