using System.Windows;

namespace IMODY
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void ResetPasswordButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string newPassword = NewPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Tüm alanları doldur.");
                return;
            }

            if (!AccountService.ResetPassword(email, newPassword))
            {
                MessageBox.Show("Bu e-posta ile kayıtlı hesap bulunamadı.");
                return;
            }

            MessageBox.Show("Şifren yenilendi. Giriş yapabilirsin.");

            Close();
        }
    }
}