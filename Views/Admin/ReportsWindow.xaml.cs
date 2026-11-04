
using CinemaManagementSystem.Models;
using System.Linq;
using System.Windows;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class ReportsWindow : Window
    {
        public ReportsWindow()
        {
            InitializeComponent();
            LoadReports();
        }

        private void LoadReports()
        {
            using (DBContext db = new DBContext())
            {
                // Calculate total revenue from successful payments
                decimal totalRevenue = db.Payments
                    .Where(p => p.Status == "Paid")
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                // Count total bookings
                int totalBookings = db.Bookings.Count();

                // Count bookings by status
                int pendingBookings = db.Bookings
                    .Count(b => b.Status == "Pending");

                int confirmedBookings = db.Bookings
                    .Count(b => b.Status == "Confirmed");

                int cancelledBookings = db.Bookings
                    .Count(b => b.Status == "Cancelled");

                int expiredBookings = db.Bookings
                    .Count(b => b.Status == "Expired");
                // Display data
                txtTotalRevenue.Text = totalRevenue.ToString("N0") + " VND";

                txtTotalBookings.Text = totalBookings.ToString();

                txtPendingBookings.Text = pendingBookings.ToString();

                txtConfirmedBookings.Text = confirmedBookings.ToString();

                txtCancelledBookings.Text = cancelledBookings.ToString();

                txtExpiredBookings.Text = expiredBookings.ToString();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadReports();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminDashboardWindow dashboard = new AdminDashboardWindow();
            dashboard.Show();
            this.Close();
        }
    }
}
