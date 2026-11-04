
using CinemaManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Customer
{
    public partial class PaymentWindow : Window
    {
        private readonly int bookingId;

        public PaymentWindow(int bookingId)
        {
            InitializeComponent();

            this.bookingId = bookingId;

            LoadBookingDetails();
        }

        private void LoadBookingDetails()
        {
            using (DBContext db = new DBContext())
            {
                var booking = db.Bookings
                    .Include(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(b => b.Showtime)
                        .ThenInclude(s => s.Room)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Seat)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    MessageBox.Show("Booking not found!");
                    Close();
                    return;
                }

                txtBookingCode.Text = booking.BookingCode;
                txtMovie.Text = booking.Showtime.Movie.Title;

                txtShowtime.Text =
                    booking.Showtime.StartTime.ToString("dd/MM/yyyy HH:mm");

                txtRoom.Text = booking.Showtime.Room.RoomName;

                txtSeats.Text = string.Join(", ",
                    booking.Tickets.Select(t =>
                        t.Seat.SeatRow + t.Seat.SeatNumber));

                txtTotalAmount.Text =
                    booking.TotalAmount.ToString("N0") + " VND";
            }
        }

        private void btnConfirmPayment_Click(
            object sender, RoutedEventArgs e)
        {
            // Check payment method
            if (cbPaymentMethod.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method!");
                return;
            }

            ComboBoxItem selectedItem =
                (ComboBoxItem)cbPaymentMethod.SelectedItem;

            string method = selectedItem.Content.ToString()!;

            // Convert display name to database value
            if (method == "Bank Transfer")
                method = "BankTransfer";
            else if (method == "E-Wallet")
                method = "EWallet";

            using (DBContext db = new DBContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                var booking = db.Bookings
                    .Include(b => b.Payment)
                    .Include(b => b.Tickets)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    MessageBox.Show("Booking not found!");
                    return;
                }

                if (booking.Status != "Pending" ||
                    booking.Payment == null ||
                    booking.Payment.Status != "Pending")
                {
                    MessageBox.Show("This booking cannot be paid!");
                    return;
                }

                // Update Payment
                booking.Payment.Method = method;
                booking.Payment.Status = "Paid";
                booking.Payment.PaymentTime = DateTime.Now;

                // Confirm Booking
                booking.Status = "Confirmed";

                // Update Tickets
                foreach (Ticket ticket in booking.Tickets)
                {
                    ticket.Status = "Sold";
                }

                db.SaveChanges();
                transaction.Commit();
            }

            MessageBox.Show(
                $"Payment successful!\n" +
                $"Payment Method: {method}\n" +
                "Your booking has been confirmed.",
                "Payment Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
