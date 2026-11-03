using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class MoviesWindow : Window
    {
        public MoviesWindow()
        {
            InitializeComponent();
            LoadMovies();
        }

        private void LoadMovies()
        {
            using (DBContext db = new DBContext())
            {
                dgMovies.ItemsSource = db.Movies.ToList();
            }
        }

        private void txtSearch_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            using (DBContext db = new DBContext())
            {
                dgMovies.ItemsSource = db.Movies
                    .Where(m => m.Title.ToLower().Contains(keyword))
                    .ToList();
            }
        }

        private void btnAddMovie_Click(object sender, RoutedEventArgs e)
        {
            AddMovieWindow addMovieWindow = new AddMovieWindow();

            if (addMovieWindow.ShowDialog() == true)
            {
                LoadMovies();
            }
        }
        private void btnEditMovie_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            Movie? selectedMovie = button?.DataContext as Movie;

            if (selectedMovie == null)
            {
                return;
            }

            EditMovieWindow editWindow = new EditMovieWindow(selectedMovie);

            if (editWindow.ShowDialog() == true)
            {
                LoadMovies();
            }
        }
        private void btnDeleteMovie_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            Movie? selectedMovie = button?.DataContext as Movie;

            if (selectedMovie == null)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete '{selectedMovie.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (DBContext db = new DBContext())
                {
                    Movie? movie = db.Movies.FirstOrDefault(
                        m => m.MovieId == selectedMovie.MovieId);

                    if (movie != null)
                    {
                        db.Movies.Remove(movie);
                        db.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("Movie not found!");
                        return;
                    }
                }

                MessageBox.Show("Movie deleted successfully!");
                LoadMovies();
            }
        }
    }
}