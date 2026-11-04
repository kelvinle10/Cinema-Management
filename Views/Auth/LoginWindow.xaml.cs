using CinemaManagementSystem.Models;
using CinemaManagementSystem.Views.Admin;
using CinemaManagementSystem.Views.Customer;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
namespace CinemaManagementSystem.Views.Auth
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usernameOrEmail = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            // Check empty input
            if (string.IsNullOrEmpty(usernameOrEmail) ||
                string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password!");
                return;
            }

            // Hash password using SHA256
            byte[] passwordHash = SHA256.HashData(
                Encoding.UTF8.GetBytes(password));

            using (DBContext db = new DBContext())
            {
                // Find account by username or email
                var account = db.Accounts
                    .Include(a => a.Customer)
                    .FirstOrDefault(a =>
                        a.Username == usernameOrEmail ||
                        (a.Customer != null &&
                         a.Customer.Email == usernameOrEmail));

                // Check account existence
                if (account == null)
                {
                    MessageBox.Show("Account does not exist!");
                    return;
                }

                // Check password
                if (!account.PasswordHash.SequenceEqual(passwordHash))
                {
                    MessageBox.Show("Incorrect password!");
                    return;
                }

                // Check account status
                if (!account.IsActive)
                {
                    MessageBox.Show("Your account has been disabled!");
                    return;
                }

                // Navigate based on role
                Window nextWindow;

                if (account.Role == "Admin")
                {
                    nextWindow = new AdminDashboardWindow();
                }
                else if (account.Role == "Customer")
                {
                    if (account.Customer == null)
                    {
                        MessageBox.Show("Customer information not found!");
                        return;
                    }

                    nextWindow = new CustomerHomeWindow(account.Customer.CustomerId);
                }
                else
                {
                    MessageBox.Show("Invalid account role!");
                    return;
                }

                MessageBox.Show("Login successful! Welcome " + account.FullName);

                nextWindow.Show();
                this.Close();
            }
        }
        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.Show();
            this.Close();
        }
    }
}
