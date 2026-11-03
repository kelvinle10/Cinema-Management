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
    }
}