using CinemaManagementSystem.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class EditMovieWindow : Window
    {
        private int movieId;

        public EditMovieWindow(Movie movie)
        {
            InitializeComponent();

            movieId = movie.MovieId;

            // Display existing movie information
            txtTitle.Text = movie.Title;
            txtDescription.Text = movie.Description;
            txtGenre.Text = movie.Genre;
            txtDuration.Text = movie.Duration.ToString();

            dpReleaseDate.SelectedDate = movie.ReleaseDate.HasValue
                ? movie.ReleaseDate.Value.ToDateTime(TimeOnly.MinValue)
                : null;

            dpEndDate.SelectedDate = movie.EndDate.HasValue
                ? movie.EndDate.Value.ToDateTime(TimeOnly.MinValue)
                : null;

            txtPosterUrl.Text = movie.PosterUrl;
            txtTrailerUrl.Text = movie.TrailerUrl;

            SelectComboBoxItem(cbAgeRating, movie.AgeRating);
            SelectComboBoxItem(cbStatus, movie.Status);
        }

        private void SelectComboBoxItem(ComboBox comboBox, string? value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == value)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtGenre.Text) ||
                string.IsNullOrWhiteSpace(txtDuration.Text))
            {
                MessageBox.Show("Please fill in all required fields!");
                return;
            }

            if (!int.TryParse(txtDuration.Text, out int duration) ||
                duration <= 0)
            {
                MessageBox.Show("Duration must be a positive number!");
                return;
            }

            if (dpReleaseDate.SelectedDate.HasValue &&
                dpEndDate.SelectedDate.HasValue &&
                dpEndDate.SelectedDate < dpReleaseDate.SelectedDate)
            {
                MessageBox.Show("End Date must be after Release Date!");
                return;
            }

            string? ageRating =
                (cbAgeRating.SelectedItem as ComboBoxItem)?.Content.ToString();

            string? status =
                (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (ageRating == null || status == null)
            {
                MessageBox.Show("Please select Age Rating and Status!");
                return;
            }
            using (DBContext db = new DBContext())
            {
                Movie? movie = db.Movies.FirstOrDefault(
                    m => m.MovieId == movieId);

                if (movie == null)
                {
                    MessageBox.Show("Movie not found!");
                    return;
                }

                movie.Title = txtTitle.Text.Trim();
                movie.Description = txtDescription.Text.Trim();
                movie.Genre = txtGenre.Text.Trim();
                movie.Duration = duration;
                movie.AgeRating = ageRating;

                movie.ReleaseDate = dpReleaseDate.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(dpReleaseDate.SelectedDate.Value)
                    : null;

                movie.EndDate = dpEndDate.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(dpEndDate.SelectedDate.Value)
                    : null;

                movie.PosterUrl = txtPosterUrl.Text.Trim();
                movie.TrailerUrl = txtTrailerUrl.Text.Trim();
                movie.Status = status;

                db.SaveChanges();
            }

            MessageBox.Show("Movie updated successfully!");

            this.DialogResult = true;
            this.Close();
        }
    }
}