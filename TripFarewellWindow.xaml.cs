using System;
using System.Windows;

namespace IMODY
{
    public partial class TripFarewellWindow : Window
    {
        private string destinationName = "Seçilen Şehir";

        public TripFarewellWindow(string destination)
        {
            InitializeComponent();
            destinationName = destination;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(UserSession.CurrentTheme);
            DestinationSubtitleText.Text = $"{destinationName} seyahatin için her şey eksiksiz hazırlandı.";
        }

        private void GoToMyTrips_Click(object sender, RoutedEventArgs e)
        {
            MyTripsWindow myTrips = new MyTripsWindow();
            myTrips.Show();
            Close();

            // Eğer sahibi CreateTripWindow ise onu da kapatabiliriz veya açık bırakabiliriz
            if (Owner != null)
            {
                Owner.Close();
            }
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            HomeWindow home = new HomeWindow();
            home.Show();
            Close();

            if (Owner != null)
            {
                Owner.Close();
            }
        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
