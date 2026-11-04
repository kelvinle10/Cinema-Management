
using CinemaManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class BookingDetailsWindow : Window
    {
        private int bookingId;

        public BookingDetailsWindow(int id)
        {
            InitializeComponent();

            bookingId = id;
            LoadBookingDetails();
        }


        private void LoadBookingDetails()
        {
            using (DBContext db = new DBContext())
            {
                Booking? booking = db.Bookings
                    .Include(b => b.Customer)
                        .ThenInclude(c => c.Account)
                    .Include(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(b => b.Showtime)
                        .ThenInclude(s => s.Room)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    MessageBox.Show("Booking not found!");
                    this.Close();
                    return;
                }

                txtBookingCode.Text = "Booking Code: " + booking.BookingCode;

                txtCustomerName.Text = booking.Customer.Account.FullName;
                txtEmail.Text = booking.Customer.Email;

                txtMovie.Text = booking.Showtime.Movie.Title;
                txtRoom.Text = booking.Showtime.Room.RoomName;

                txtStartTime.Text =
                    booking.Showtime.StartTime.ToString("dd/MM/yyyy HH:mm");

                txtBookingTime.Text =
                    booking.BookingTime.ToString("dd/MM/yyyy HH:mm");

                txtTotalAmount.Text =
                    booking.TotalAmount.ToString("N0") + " VND";

                // Load tickets
                dgTickets.ItemsSource = db.Tickets
                    .Where(t => t.BookingId == bookingId)
                    .Select(t => new
                    {
                        t.TicketCode,
                        SeatName = t.Seat.SeatRow + t.Seat.SeatNumber,
                        t.Seat.SeatType,
                        t.Price,
                        t.Status
                    })
                    .ToList();

                // Load payment information
                Payment? payment = db.Payments
                    .FirstOrDefault(p => p.BookingId == bookingId);

                if (payment != null)
                {
                    txtPaymentMethod.Text = payment.Method;
                    txtPaymentStatus.Text = payment.Status;

                    txtPaymentTime.Text = payment.PaymentTime.HasValue
                        ? payment.PaymentTime.Value.ToString("dd/MM/yyyy HH:mm")
                        : "Not paid yet";

                    txtTransactionCode.Text =
                        payment.TransactionCode ?? "N/A";
                }
                else
                {
                    txtPaymentMethod.Text = "N/A";
                    txtPaymentStatus.Text = "No Payment";
                    txtPaymentTime.Text = "N/A";
                    txtTransactionCode.Text = "N/A";
                }
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
