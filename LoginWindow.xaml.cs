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

            if (!AccountService.Create(name, email, password))
            {
                MessageBox.Show("Bu e-posta ile zaten bir hesap var.", "Ody: Kayıt Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserSession.Name = name;
            UserSession.Email = email;
            UserSession.SaveSession();

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