using System.Windows;

namespace IMODY
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1 Kere oturum açıldıysa bir daha sormadan direkt ana ekrana geç!
            if (UserSession.TryLoadSession())
            {
                OpenHome();
            }
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow { Owner = this };
            forgotPasswordWindow.ShowDialog();
        }

        private void OpenKvkkModal_Click(object sender, RoutedEventArgs e)
        {
            KvkkModal.Visibility = Visibility.Visible;
        }

        private void CloseKvkkModal_Click(object sender, RoutedEventArgs e)
        {
            KvkkModal.Visibility = Visibility.Collapsed;
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tüm alanları doldur.", "Ody: Bilgi Eksik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (KvkkConsentCheckBox.IsChecked != true)
            {
                MessageBox.Show("Hesap oluşturabilmek için lütfen KVKK Aydınlatma Metnini onaylayın.", "Ody: KVKK Onayı Gerekli", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!AccountService.Create(name, email, password))
            {
                MessageBox.Show("Bu e-posta ile zaten bir hesap var.", "Ody: Kayıt Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserSession.Name = name;
            UserSession.Email = email;
            UserSession.SaveSession();

            // Telegram Telemetri Bildirimi (Kullanıcı Kaydı)
            TelemetryService.LogUserRegistered(name, email);

            OpenHome();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserAccount? account = AccountService.Login(EmailTextBox.Text.Trim(), PasswordBox.Password);

            if (account == null)
            {
                MessageBox.Show("E-posta veya şifre yanlış.", "Ody: Giriş Yapılamadı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserSession.Name = account.Name;
            UserSession.Email = account.Email;
            UserSession.SaveSession();

            // Telegram Telemetri Bildirimi (Kullanıcı Girişi)
            TelemetryService.LogUserLogin(account.Name, account.Email);

            OpenHome();
        }

        private void OpenHome()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}