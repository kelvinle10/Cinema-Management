
using CinemaManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaManagementSystem.Views.Admin
{
    public partial class CustomersWindow : Window
    {
        public CustomersWindow()
        {
            InitializeComponent();
            LoadCustomers();
        }

        // Load customer list
        private void LoadCustomers()
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            using (DBContext db = new DBContext())
            {
                var query = db.Customers
                    .Where(c => c.Account.Role == "Customer");

                // Search by Full Name or Email
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(c =>
                        c.Account.FullName.ToLower().Contains(keyword) ||
                        c.Email.ToLower().Contains(keyword));
                }

                dgCustomers.ItemsSource = query
                    .Select(c => new
                    {
                        c.CustomerId,
                        FullName = c.Account.FullName,
                        Username = c.Account.Username,
                        c.Email,
                        c.Phone,
                        c.DateOfBirth,
                        Status = c.Account.IsActive ? "Active" : "Inactive",
                        ActionText = c.Account.IsActive ? "Deactivate" : "Activate"
                    })
                    .OrderBy(c => c.CustomerId)
                    .ToList();
            }
        }

        // Search
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCustomers();
        }

        // Refresh
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            LoadCustomers();
        }

        // Activate / Deactivate customer
        private void btnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int customerId = (int)button.DataContext.GetType()
                .GetProperty("CustomerId")!
                .GetValue(button.DataContext)!;

            using (DBContext db = new DBContext())
            {
                CinemaManagementSystem.Models.Customer? customer = db.Customers
                                    .Include(c => c.Account)
                                    .FirstOrDefault(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    MessageBox.Show("Customer not found!");
                    return;
                }

                bool newStatus = !customer.Account.IsActive;

                string action = newStatus ? "activate" : "deactivate";

                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to {action} this customer?",
                    "Confirm Action",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                customer.Account.IsActive = newStatus;

                db.SaveChanges();
            }

            MessageBox.Show("Customer status updated successfully!");

            LoadCustomers();
        }

        // Back to Dashboard
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AdminDashboardWindow dashboard = new AdminDashboardWindow();
            dashboard.Show();
            this.Close();
        }
    }
}
