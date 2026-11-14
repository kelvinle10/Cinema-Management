
using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class GenreWindow : Window
    {
        public GenreWindow()
        {
            InitializeComponent();
            LoadGenre();
        }

        private void LoadGenre()
        {
            using (DBContext db = new DBContext())
            {
                var result = db.Movies
                    .ToList()
                    .SelectMany(m => m.Genre.Split(','))
                    .Select(g => g.Trim())
                    .GroupBy(g => g)
                    .Select(g => new
                    {
                        Genre = g.Key,
                        TotalMovies = g.Count()
                    })
                    .OrderByDescending(g => g.TotalMovies)
                    .ToList();

                dgGenre.ItemsSource = result;
            }
        }
    }
}
