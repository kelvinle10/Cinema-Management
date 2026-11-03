
using CinemaManagementSystem.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class AddShowtimeWindow : Window
    {
        public AddShowtimeWindow()
        {
            InitializeComponent();

            LoadMovies();
            LoadRooms();

            dpStartDate.SelectedDate = DateTime.Today;
            dpEndDate.SelectedDate = DateTime.Today;
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddShowtime_Click(object sender, RoutedEventArgs e)
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

            DateTime startDateTime = dpStartDate.SelectedDate.Value.Date.Add(startTime);
            DateTime endDateTime = dpEndDate.SelectedDate.Value.Date.Add(endTime);

            if (endDateTime <= startDateTime)
            {
                MessageBox.Show("End Time must be later than Start Time!");
                return;
            }

            if (!decimal.TryParse(txtTicketPrice.Text.Trim(), out decimal ticketPrice) ||
                ticketPrice < 0)
            {
                MessageBox.Show("Ticket Price must be a non-negative number!");
                return;
            }

            int movieId = (int)cbMovie.SelectedValue;
            int roomId = (int)cbRoom.SelectedValue;

            string status = ((System.Windows.Controls.ComboBoxItem)
                cbStatus.SelectedItem).Content.ToString()!;

            using (DBContext db = new DBContext())
            {
                bool isDuplicate = db.Showtimes.Any(s =>
                    s.RoomId == roomId &&
                    s.StartTime == startDateTime);

                if (isDuplicate)
                {
                    MessageBox.Show("A showtime already exists at this start time in this room!");
                    return;
                }

                Showtime showtime = new Showtime
                {
                    MovieId = movieId,
                    RoomId = roomId,
                    StartTime = startDateTime,
                    EndTime = endDateTime,
                    TicketPrice = ticketPrice,
                    Status = status
                };

                db.Showtimes.Add(showtime);
                db.SaveChanges();
            }

            MessageBox.Show("Showtime added successfully!");
            this.DialogResult = true;
            this.Close();
        }
    }
}
