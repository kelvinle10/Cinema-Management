
using CinemaManagementSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class AddMovieWindow : Window
    {
        public AddMovieWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtGenre.Text) ||
                string.IsNullOrWhiteSpace(txtDuration.Text))
            {
                MessageBox.Show("Please fill in all required fields!");
                return;
            }

            // Validate duration
            if (!int.TryParse(txtDuration.Text, out int duration) ||
                duration <= 0)
            {
                MessageBox.Show("Duration must be a positive number!");
                return;
            }

            // Validate dates
            if (dpReleaseDate.SelectedDate.HasValue &&
                dpEndDate.SelectedDate.HasValue &&
                dpEndDate.SelectedDate < dpReleaseDate.SelectedDate)
            {
                MessageBox.Show("End Date must be after Release Date!");
                return;
            }

            string ageRating =
                (cbAgeRating.SelectedItem as ComboBoxItem)?.Content.ToString();

            string status =
                (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            Movie movie = new Movie
            {
                Title = txtTitle.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Genre = txtGenre.Text.Trim(),
                Duration = duration,
                AgeRating = ageRating,
                ReleaseDate = dpReleaseDate.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(dpReleaseDate.SelectedDate.Value)
                    : null,
                EndDate = dpEndDate.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(dpEndDate.SelectedDate.Value)
                    : null,
                PosterUrl = txtPosterUrl.Text.Trim(),
                TrailerUrl = txtTrailerUrl.Text.Trim(),
                Status = status,
                CreatedAt = DateTime.Now
            };

            using (DBContext db = new DBContext())
            {
                db.Movies.Add(movie);
                db.SaveChanges();
            }

            MessageBox.Show("Movie added successfully!");

            this.DialogResult = true;
            this.Close();
        }
    }
}
