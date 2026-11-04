
using CinemaManagementSystem.Models;
using CinemaManagementSystem.Views.Auth;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Customer
{
    public partial class CustomerHomeWindow : Window
    {
        private readonly int customerId;

        public CustomerHomeWindow(int customerId)
        {
            InitializeComponent();

            this.customerId = customerId;

            LoadMovies();
        }

        private void LoadMovies()
        {
            using (DBContext db = new DBContext())
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                var movies = db.Movies.ToList();

                // Movies currently showing
                var nowShowing = movies
                    .Where(m => m.ReleaseDate.HasValue
                        && m.ReleaseDate.Value <= today
                        && (!m.EndDate.HasValue || m.EndDate.Value >= today))
                    .OrderBy(m => m.Title)
                    .ToList();

                // Upcoming movies
                var comingSoon = movies
                    .Where(m => m.ReleaseDate.HasValue
                        && m.ReleaseDate.Value > today)
                    .OrderBy(m => m.ReleaseDate)
                    .ToList();

                icNowShowing.ItemsSource = nowShowing;
                icComingSoon.ItemsSource = comingSoon;

                // Featured Movie
                var featuredMovie = nowShowing.FirstOrDefault()
                                    ?? comingSoon.FirstOrDefault();

                if (featuredMovie != null)
                {
                    featuredPanel.DataContext = featuredMovie;
                }
            }
        }

        // Open Movie Details
        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Movie? selectedMovie = button.DataContext as Movie;

            if (selectedMovie != null)
            {
                MovieDetailsWindow details =
                       new MovieDetailsWindow(selectedMovie.MovieId, customerId);

                details.Owner = this;
                details.ShowDialog();
            }
        }

        // Logout
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
