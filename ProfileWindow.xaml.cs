using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IMODY
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserData();
        }

        private void LoadUserData()
        {
            string displayName = string.IsNullOrWhiteSpace(UserSession.Name) ? "Gezgin" : UserSession.Name;
            UserNameTextBlock.Text = displayName;
            UserEmailTextBlock.Text = string.IsNullOrWhiteSpace(UserSession.Email) ? "gezgin@imody.ai" : UserSession.Email;
            
            string rank = UserSession.GetCurrentRankTitle();
            RankBadgeTextBlock.Text = rank;
            PointsTextBlock.Text = $"{UserSession.TravelPoints:N0} XP";

            // XP İlerleme barı
            int maxForLevel = UserSession.TravelPoints < 500 ? 500 :
                              UserSession.TravelPoints < 1500 ? 1500 :
                              UserSession.TravelPoints < 3000 ? 3000 :
                              UserSession.TravelPoints < 6000 ? 6000 : 10000;
            RankProgressBar.Maximum = maxForLevel;
            RankProgressBar.Value = Math.Min(UserSession.TravelPoints, maxForLevel);

            TripsCountTextBlock.Text = $"{UserSession.CompletedTripsCount} Rota";
            CitiesCountTextBlock.Text = $"{UserSession.CitiesDiscoveredCount} Şehir";

            if (!string.IsNullOrWhiteSpace(UserSession.ProfileImagePath) && File.Exists(UserSession.ProfileImagePath))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(UserSession.ProfileImagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    ProfileImageEllipse.Fill = new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
                    AvatarLetterTextBlock.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    AvatarLetterTextBlock.Text = displayName.Substring(0, 1).ToUpper();
                    AvatarLetterTextBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                AvatarLetterTextBlock.Text = displayName.Substring(0, 1).ToUpper();
                AvatarLetterTextBlock.Visibility = Visibility.Visible;
            }

            HighlightCurrentRank();
        }

        private void HighlightCurrentRank()
        {
            int points = UserSession.TravelPoints;
            Brush accent = (Brush)Application.Current.Resources["Theme_Accent"] ?? Brushes.Gold;

            RankCard1.Opacity = points >= 0 ? 1.0 : 0.5;
            RankCard2.Opacity = points >= 500 ? 1.0 : 0.5;
            RankCard3.Opacity = points >= 1500 ? 1.0 : 0.5;
            RankCard4.Opacity = points >= 3000 ? 1.0 : 0.5;
            RankCard5.Opacity = points >= 6000 ? 1.0 : 0.5;
        }

        private void TabProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfileTabContent.Visibility = Visibility.Visible;
            RanksTabContent.Visibility = Visibility.Collapsed;

            TabProfileBtn.Background = (Brush)Application.Current.Resources["Theme_Accent"];
            TabProfileBtn.Foreground = new SolidColorBrush(Color.FromRgb(43, 7, 23));
            TabProfileBtn.FontWeight = FontWeights.Bold;

            TabRanksBtn.Background = Brushes.Transparent;
            TabRanksBtn.Foreground = (Brush)Application.Current.Resources["Theme_TextSecondary"];
            TabRanksBtn.FontWeight = FontWeights.SemiBold;
        }

        private void TabRanks_Click(object sender, RoutedEventArgs e)
        {
            ProfileTabContent.Visibility = Visibility.Collapsed;
            RanksTabContent.Visibility = Visibility.Visible;

            TabRanksBtn.Background = (Brush)Application.Current.Resources["Theme_Accent"];
            TabRanksBtn.Foreground = new SolidColorBrush(Color.FromRgb(43, 7, 23));
            TabRanksBtn.FontWeight = FontWeights.Bold;

            TabProfileBtn.Background = Brushes.Transparent;
            TabProfileBtn.Foreground = (Brush)Application.Current.Resources["Theme_TextSecondary"];
            TabProfileBtn.FontWeight = FontWeights.SemiBold;
        }

        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Profil Fotoğrafı Seç",
                Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string selectedFile = openFileDialog.FileName;
                    string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IMODY");
                    if (!Directory.Exists(appDataFolder))
                        Directory.CreateDirectory(appDataFolder);

                    string destFile = Path.Combine(appDataFolder, "profile_pic" + Path.GetExtension(selectedFile));
                    File.Copy(selectedFile, destFile, true);

                    UserSession.ProfileImagePath = destFile;
                    UserSession.SaveSession();

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(destFile, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    ProfileImageEllipse.Fill = new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
                    AvatarLetterTextBlock.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fotoğraf yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}