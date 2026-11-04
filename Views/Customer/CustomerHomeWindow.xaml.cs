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
        private bool isLoadingGenres = false;

        public CustomerHomeWindow(int customerId)
        {
            InitializeComponent();

            this.customerId = customerId;

            LoadGenres();
            LoadMovies();
        }

        // Load genres from database
        private void LoadGenres()
        {
            isLoadingGenres = true;

            using (DBContext db = new DBContext())
            {
                var genres = db.Movies
                    .Select(m => m.Genre)
                    .ToList()
                    .Where(g => !string.IsNullOrWhiteSpace(g))
                    .SelectMany(g => g.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(g => g.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(g => g)
                    .ToList();

                genres.Insert(0, "All Genres");

                cbCategory.ItemsSource = genres;
                cbCategory.SelectedIndex = 0;
            }

            isLoadingGenres = false;
        }

        // Load and filter movies
        private void LoadMovies()
        {
            using (DBContext db = new DBContext())
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                var movies = db.Movies.ToList();

                // Search by movie title or genre
                string keyword = txtSearch.Text.Trim();

                if (!string.IsNullOrEmpty(keyword))
                {
                    movies = movies
                        .Where(m =>
                            m.Title.Contains(
                                keyword,
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            m.Genre.Contains(
                                keyword,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Filter by genre
                string selectedGenre = cbCategory.SelectedItem?.ToString()
                                       ?? "All Genres";

                if (selectedGenre != "All Genres")
                {
                    movies = movies
                        .Where(m => m.Genre
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Any(g => g.Trim().Equals(
                                selectedGenre,
                                StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                // Now Showing movies
                var nowShowing = movies
                    .Where(m =>
                        m.ReleaseDate.HasValue
                        && m.ReleaseDate.Value <= today
                        && (!m.EndDate.HasValue
                            || m.EndDate.Value >= today))
                    .OrderBy(m => m.Title)
                    .ToList();

                // Coming Soon movies
                var comingSoon = movies
                    .Where(m =>
                        m.ReleaseDate.HasValue
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
                    featuredPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    featuredPanel.DataContext = null;
                    featuredPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        // Search movies
        private void txtSearch_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            LoadMovies();
        }

        // Filter movies by genre
        private void cbCategory_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (isLoadingGenres)
                return;

            LoadMovies();
        }

        // Open Movie Details
        private void btnViewDetails_Click(
            object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Movie? selectedMovie = button.DataContext as Movie;

            if (selectedMovie != null)
            {
                MovieDetailsWindow details =
                    new MovieDetailsWindow(
                        selectedMovie.MovieId,
                        customerId);

                details.Owner = this;
                details.ShowDialog();
            }
        }

        // Logout
        private void btnLogout_Click(
            object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();

            Close();
        }
    }
}