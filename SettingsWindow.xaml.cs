using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace IMODY
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AccountTextBlock.Text = string.IsNullOrWhiteSpace(UserSession.Email) ? "gezgin@imody.ai" : UserSession.Email;
            HighlightSelectedTheme(UserSession.CurrentTheme);
        }

        private void NavyTheme_Click(object sender, MouseButtonEventArgs e)
        {
            ThemeManager.ApplyTheme("Navy");
            HighlightSelectedTheme("Navy");
        }

        private void CherryTheme_Click(object sender, MouseButtonEventArgs e)
        {
            ThemeManager.ApplyTheme("Cherry");
            HighlightSelectedTheme("Cherry");
        }

        private void DarkTheme_Click(object sender, MouseButtonEventArgs e)
        {
            ThemeManager.ApplyTheme("Dark");
            HighlightSelectedTheme("Dark");
        }

        private void LightTheme_Click(object sender, MouseButtonEventArgs e)
        {
            ThemeManager.ApplyTheme("Light");
            HighlightSelectedTheme("Light");
        }

        private void HighlightSelectedTheme(string theme)
        {
            Brush border = (Brush)Application.Current.Resources["Theme_Border"];

            NavyThemeCard.BorderThickness = new Thickness(1);
            NavyThemeCard.BorderBrush = border;
            NavyCheckDot.Visibility = Visibility.Collapsed;

            CherryThemeCard.BorderThickness = new Thickness(1);
            CherryThemeCard.BorderBrush = border;
            CherryCheckDot.Visibility = Visibility.Collapsed;

            DarkThemeCard.BorderThickness = new Thickness(1);
            DarkThemeCard.BorderBrush = border;
            DarkCheckDot.Visibility = Visibility.Collapsed;

            LightThemeCard.BorderThickness = new Thickness(1);
            LightThemeCard.BorderBrush = border;
            LightCheckDot.Visibility = Visibility.Collapsed;

            Brush accent = (Brush)Application.Current.Resources["Theme_Accent"];

            if (theme == "Light")
            {
                LightThemeCard.BorderBrush = accent;
                LightThemeCard.BorderThickness = new Thickness(2.5);
                LightCheckDot.Visibility = Visibility.Visible;
            }
            else if (theme == "Dark")
            {
                DarkThemeCard.BorderBrush = accent;
                DarkThemeCard.BorderThickness = new Thickness(2.5);
                DarkCheckDot.Visibility = Visibility.Visible;
            }
            else if (theme == "Cherry")
            {
                CherryThemeCard.BorderBrush = accent;
                CherryThemeCard.BorderThickness = new Thickness(2.5);
                CherryCheckDot.Visibility = Visibility.Visible;
            }
            else
            {
                NavyThemeCard.BorderBrush = accent;
                NavyThemeCard.BorderThickness = new Thickness(2.5);
                NavyCheckDot.Visibility = Visibility.Visible;
            }
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Oturumu kapatmak istediğinize emin misiniz?", "Çıkış Yap", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                UserSession.ClearSession();
                LoginWindow login = new LoginWindow();
                login.Show();

                foreach (Window win in Application.Current.Windows)
                {
                    if (win != login)
                        win.Close();
                }
            }
        }
    }
}