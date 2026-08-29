using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace IMODY
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(UserSession.CurrentTheme);
            SetTimeBasedGreeting();
            Setup3DGlobe();
            PlayGalaxyVideo();
        }

        private void PlayGalaxyVideo()
        {
            try
            {
                string videoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "bg_galaxy.mp4");
                if (File.Exists(videoPath))
                {
                    GalaxyVideoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
                    GalaxyVideoPlayer.IsMuted = true;
                    GalaxyVideoPlayer.Volume = 0;
                    GalaxyVideoPlayer.SpeedRatio = 1.0;
                    GalaxyVideoPlayer.Play();
                }
            }
            catch { }
        }

        private void GalaxyVideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                GalaxyVideoPlayer.Position = TimeSpan.Zero;
                GalaxyVideoPlayer.Play();
            }
            catch { }
        }

        // =========================================================
        // 3D DÜNYA KÜRESİ (GLOBE)
        // =========================================================
        private void Setup3DGlobe()
        {
            try
            {
                var mesh = CreateSphereMesh(1.0, 36, 36);
                GlobeGeometryModel.Geometry = mesh;

                string earthPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "earth.jpeg");
                ImageSource imageSource;

                if (File.Exists(earthPath))
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(earthPath, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    imageSource = bmp;
                }
                else
                {
                    imageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/earth.jpeg"));
                }

                GlobeGeometryModel.Material = new DiffuseMaterial(new ImageBrush(imageSource));

                var rotationAnim = new DoubleAnimation
                {
                    From = 0,
                    To = 360,
                    Duration = TimeSpan.FromSeconds(25),
                    RepeatBehavior = RepeatBehavior.Forever
                };
                GlobeRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotationAnim);
            }
            catch { }
        }

        private MeshGeometry3D CreateSphereMesh(double radius, int slices, int stacks)
        {
            var mesh = new MeshGeometry3D();

            for (int stack = 0; stack <= stacks; stack++)
            {
                double phi = Math.PI * stack / stacks;
                double y = radius * Math.Cos(phi);
                double ringRadius = radius * Math.Sin(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    double theta = 2.0 * Math.PI * slice / slices;
                    double x = ringRadius * Math.Sin(theta);
                    double z = ringRadius * Math.Cos(theta);

                    mesh.Positions.Add(new Point3D(x, y, z));
                    mesh.Normals.Add(new Vector3D(x / radius, y / radius, z / radius));
                    mesh.TextureCoordinates.Add(new Point((double)slice / slices, (double)stack / stacks));
                }
            }

            for (int stack = 0; stack < stacks; stack++)
            {
                for (int slice = 0; slice < slices; slice++)
                {
                    int first = (stack * (slices + 1)) + slice;
                    int second = first + slices + 1;

                    mesh.TriangleIndices.Add(first);
                    mesh.TriangleIndices.Add(second);
                    mesh.TriangleIndices.Add(first + 1);

                    mesh.TriangleIndices.Add(second);
                    mesh.TriangleIndices.Add(second + 1);
                    mesh.TriangleIndices.Add(first + 1);
                }
            }

            return mesh;
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
            home.WindowState = this.WindowState;
            if (this.WindowState != WindowState.Maximized)
            {
                home.Left = this.Left;
                home.Top = this.Top;
                home.Width = this.Width;
                home.Height = this.Height;
            }
            home.Show();
            Close();
        }

        private void MyTripsButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsWindow myTrips = new MyTripsWindow { Owner = this };
            myTrips.ShowDialog();
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