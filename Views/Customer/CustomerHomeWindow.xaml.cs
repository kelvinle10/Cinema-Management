
using CinemaManagementSystem.Models;
using CinemaManagementSystem.Views.Auth;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Customer
{
    public partial class CustomerHomeWindow : Window
    {
        public CustomerHomeWindow()
        {
            InitializeComponent();
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

                // Featured movie
                var featuredMovie = nowShowing.FirstOrDefault()
                                    ?? comingSoon.FirstOrDefault();

                if (featuredMovie != null)
                {
                    featuredPanel.DataContext = featuredMovie;
                }
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
