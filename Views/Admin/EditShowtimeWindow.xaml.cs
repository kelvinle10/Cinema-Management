
using CinemaManagementSystem.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class EditShowtimeWindow : Window
    {
        private int showtimeId;

        public EditShowtimeWindow(int id)
        {
            InitializeComponent();

            showtimeId = id;

            LoadMovies();
            LoadRooms();
            LoadShowtime();
        }

        private void LoadMovies()
        {
            using (DBContext db = new DBContext())
            {
                cbMovie.ItemsSource = db.Movies
                    .Where(m => m.Status == "Showing")
                    .ToList();

                cbMovie.DisplayMemberPath = "Title";
                cbMovie.SelectedValuePath = "MovieId";
            }
        }

        private void LoadRooms()
        {
            using (DBContext db = new DBContext())
            {
                cbRoom.ItemsSource = db.Rooms
                    .Where(r => r.IsActive)
                    .ToList();

                cbRoom.DisplayMemberPath = "RoomName";
                cbRoom.SelectedValuePath = "RoomId";
            }
        }

        private void LoadShowtime()
        {
            using (DBContext db = new DBContext())
            {
                Showtime? showtime = db.Showtimes
                    .FirstOrDefault(s => s.ShowtimeId == showtimeId);

                if (showtime == null)
                {
                    MessageBox.Show("Showtime not found!");
                    this.Close();
                    return;
                }

                cbMovie.SelectedValue = showtime.MovieId;
                cbRoom.SelectedValue = showtime.RoomId;

                dpStartDate.SelectedDate = showtime.StartTime.Date;
                txtStartTime.Text = showtime.StartTime.ToString("HH:mm");

                dpEndDate.SelectedDate = showtime.EndTime.Date;
                txtEndTime.Text = showtime.EndTime.ToString("HH:mm");

                txtTicketPrice.Text = showtime.TicketPrice.ToString(
                    CultureInfo.InvariantCulture);

                foreach (ComboBoxItem item in cbStatus.Items)
                {
                    if (item.Content.ToString() == showtime.Status)
                    {
                        cbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdateShowtime_Click(object sender, RoutedEventArgs e)
        {
            if (cbMovie.SelectedValue == null ||
                cbRoom.SelectedValue == null ||
                dpStartDate.SelectedDate == null ||
                dpEndDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(txtStartTime.Text) ||
                string.IsNullOrWhiteSpace(txtEndTime.Text) ||
                string.IsNullOrWhiteSpace(txtTicketPrice.Text) ||
                cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields!");
                return;
            }

            if (!TimeSpan.TryParseExact(
                txtStartTime.Text.Trim(), @"hh\:mm",
                CultureInfo.InvariantCulture, out TimeSpan startTime) ||
                !TimeSpan.TryParseExact(
                txtEndTime.Text.Trim(), @"hh\:mm",
                CultureInfo.InvariantCulture, out TimeSpan endTime))
            {
                MessageBox.Show("Please enter time in HH:mm format!");
                return;
            }

            DateTime startDateTime =
                dpStartDate.SelectedDate.Value.Date.Add(startTime);

            DateTime endDateTime =
                dpEndDate.SelectedDate.Value.Date.Add(endTime);

            if (endDateTime <= startDateTime)
            {
                MessageBox.Show("End Time must be later than Start Time!");
                return;
            }

            if (!decimal.TryParse(
                txtTicketPrice.Text.Trim(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal ticketPrice) || ticketPrice < 0)
            {
                MessageBox.Show("Ticket Price must be a non-negative number!");
                return;
            }

            int movieId = (int)cbMovie.SelectedValue;
            int roomId = (int)cbRoom.SelectedValue;

            string status = ((ComboBoxItem)cbStatus.SelectedItem)
                .Content.ToString()!;

            using (DBContext db = new DBContext())
            {
                bool isDuplicate = db.Showtimes.Any(s =>
                    s.ShowtimeId != showtimeId &&
                    s.RoomId == roomId &&
                    s.StartTime == startDateTime);

                if (isDuplicate)
                {
                    MessageBox.Show(
                        "A showtime already exists at this start time in this room!");
                    return;
                }

                Showtime? showtime = db.Showtimes
                    .FirstOrDefault(s => s.ShowtimeId == showtimeId);

                if (showtime == null)
                {
                    MessageBox.Show("Showtime not found!");
                    return;
                }

                showtime.MovieId = movieId;
                showtime.RoomId = roomId;
                showtime.StartTime = startDateTime;
                showtime.EndTime = endDateTime;
                showtime.TicketPrice = ticketPrice;
                showtime.Status = status;

                db.SaveChanges();
            }

            MessageBox.Show("Showtime updated successfully!");
            this.DialogResult = true;
            this.Close();
        }
    }
}
