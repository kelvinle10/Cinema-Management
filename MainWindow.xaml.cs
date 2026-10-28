using CinemaManagementSystem.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CinemaManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (username == "" || password == "")
            {
                MessageBox.Show("Please enter username and password!");
                return;
            }

            using var db = new DBContext();

            var account = db.Accounts.FirstOrDefault(
                a => a.Username == username &&
                     a.Password == password
            );

            if (account == null)
            {
                MessageBox.Show("Invalid username or password!");
                return;
            }
            MessageBox.Show("Login successful! Welcome " + account.FullName);
        }
    }
}