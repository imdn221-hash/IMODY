using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IMODY
{
    public partial class MainWindow : Window
    {
        private string activeVideoName = "ody_video_1.mp4";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(UserSession.CurrentTheme);
            SetTimeBasedGreeting();
            PlayOdyVideo("ody_video_1.mp4");
        }

        // =========================================================
        // CANLI ODY VİDEO PORTALI (MEDIAELEMENT LOOP)
        // =========================================================
        private void PlayOdyVideo(string fileName)
        {
            try
            {
                activeVideoName = fileName;
                string videoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
                
                if (File.Exists(videoPath))
                {
                    OdyVideoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
                    OdyVideoPlayer.IsMuted = true;
                    OdyVideoPlayer.Volume = 0;
                    OdyVideoPlayer.Play();
                }
            }
            catch { }
        }

        private void OdyVideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                OdyVideoPlayer.Position = TimeSpan.Zero;
                OdyVideoPlayer.Play();
            }
            catch { }
        }

        private void BtnVideo1_Click(object sender, RoutedEventArgs e)
        {
            PlayOdyVideo("ody_video_1.mp4");
            BtnVideo1.Background = (Brush)Application.Current.Resources["Theme_Accent"];
            BtnVideo1.Foreground = new SolidColorBrush(Color.FromRgb(43, 7, 23));
            BtnVideo2.Background = (Brush)Application.Current.Resources["Theme_BgSub"];
            BtnVideo2.Foreground = (Brush)Application.Current.Resources["Theme_TextPrimary"];
        }

        private void BtnVideo2_Click(object sender, RoutedEventArgs e)
        {
            PlayOdyVideo("ody_video_2.mp4");
            BtnVideo2.Background = (Brush)Application.Current.Resources["Theme_Accent"];
            BtnVideo2.Foreground = new SolidColorBrush(Color.FromRgb(43, 7, 23));
            BtnVideo1.Background = (Brush)Application.Current.Resources["Theme_BgSub"];
            BtnVideo1.Foreground = (Brush)Application.Current.Resources["Theme_TextPrimary"];
        }

        private void SetTimeBasedGreeting()
        {
            int hour = DateTime.Now.Hour;
            string greeting;

            if (hour >= 6 && hour < 12)
                greeting = "GÜNAYDIN! YOLCULUK BURADA BAŞLAR";
            else if (hour >= 12 && hour < 18)
                greeting = "TÜNAYDIN! YENİ BİR ROTA ÇİZELİM Mİ?";
            else if (hour >= 18 && hour < 23)
                greeting = "İYİ AKŞAMLAR! YOLCULUK PLANINA HAZIR MISIN?";
            else
                greeting = "İYİ GECELER! GECEYE ÖZEL BİR MACERA PLANLAYALIM";

            OdyGreetingTextBlock.Text = greeting;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            HomeWindow home = new HomeWindow();
            home.Show();
            Close();
        }

        private void ExploreButton_Click(object sender, RoutedEventArgs e)
        {
            ExploreWindow explore = new ExploreWindow { Owner = this };
            explore.ShowDialog();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow { Owner = this };
            about.ShowDialog();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow { Owner = this };
            settings.ShowDialog();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profile = new ProfileWindow { Owner = this };
            profile.ShowDialog();
        }
    }
}