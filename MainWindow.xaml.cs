using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace IMODY
{
    public partial class MainWindow : Window
    {
        private string currentPose = "ody_mascot_portrait.png";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(UserSession.CurrentTheme);
            SetTimeBasedGreeting();
            Setup3DGlobe();
            LoadOdyImage("ody_mascot_portrait.png");
            StartOdyFloatingAnimation();
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

        // =========================================================
        // ARKA PLANSIZ ŞEFFAF ODY PNG (SÜZÜLME & NEFES ALMA)
        // =========================================================
        private void LoadOdyImage(string fileName)
        {
            try
            {
                currentPose = fileName;
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
                
                if (File.Exists(path))
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(path, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    OdyTransparentImage.Source = bmp;
                }
                else
                {
                    OdyTransparentImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/{fileName}"));
                }
            }
            catch { }
        }

        private void StartOdyFloatingAnimation()
        {
            try
            {
                // Y ekseninde akıcı süzülme
                var floatY = new DoubleAnimation
                {
                    From = -10,
                    To = 10,
                    Duration = TimeSpan.FromSeconds(3.2),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                OdyTranslate.BeginAnimation(TranslateTransform.YProperty, floatY);

                // Yüzerken hafif eğilme
                var tilt = new DoubleAnimation
                {
                    From = -2.5,
                    To = 2.5,
                    Duration = TimeSpan.FromSeconds(3.8),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                OdyRotate.BeginAnimation(RotateTransform.AngleProperty, tilt);

                // Nefes alma ölçeklemesi
                var breathe = new DoubleAnimation
                {
                    From = 0.985,
                    To = 1.015,
                    Duration = TimeSpan.FromSeconds(2.6),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                OdyScale.BeginAnimation(ScaleTransform.ScaleXProperty, breathe);
                OdyScale.BeginAnimation(ScaleTransform.ScaleYProperty, breathe);
            }
            catch { }
        }

        private void BtnPosePortrait_Click(object sender, RoutedEventArgs e)
        {
            LoadOdyImage("ody_mascot_portrait.png");
            BtnPosePortrait.Background = (Brush)Application.Current.Resources["Theme_Accent"];
            BtnPosePortrait.Foreground = new SolidColorBrush(Color.FromRgb(43, 7, 23));
            BtnPoseSwim.Background = (Brush)Application.Current.Resources["Theme_BgSub"];
            BtnPoseSwim.Foreground = (Brush)Application.Current.Resources["Theme_TextPrimary"];
        }

        private void BtnPoseSwim_Click(object sender, RoutedEventArgs e)
        {
            LoadOdyImage("ody_mascot_swim.png");
            BtnPoseSwim.Background = (Brush)Application.Current.Resources["Theme_Accent"];
            BtnPoseSwim.Foreground = new SolidColorBrush(Color.FromRgb(43, 7, 23));
            BtnPosePortrait.Background = (Brush)Application.Current.Resources["Theme_BgSub"];
            BtnPosePortrait.Foreground = (Brush)Application.Current.Resources["Theme_TextPrimary"];
        }

        // =========================================================
        // İNTERAKTİF FARE TEPKİLERİ
        // =========================================================
        private void Ody_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                var zoom = new DoubleAnimation(1.06, TimeSpan.FromMilliseconds(220))
                {
                    EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.4 }
                };
                OdyScale.BeginAnimation(ScaleTransform.ScaleXProperty, zoom);
                OdyScale.BeginAnimation(ScaleTransform.ScaleYProperty, zoom);

                var fadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200));
                OdySpeechBubble.BeginAnimation(OpacityProperty, fadeIn);
            }
            catch { }
        }

        private void Ody_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                var zoomOut = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(300))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                OdyScale.BeginAnimation(ScaleTransform.ScaleXProperty, zoomOut);
                OdyScale.BeginAnimation(ScaleTransform.ScaleYProperty, zoomOut);

                var fadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));
                OdySpeechBubble.BeginAnimation(OpacityProperty, fadeOut);
            }
            catch { }
        }

        private void Ody_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var jump = new DoubleAnimation
                {
                    From = -10,
                    To = -35,
                    Duration = TimeSpan.FromMilliseconds(250),
                    AutoReverse = true,
                    EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.6 }
                };
                OdyTranslate.BeginAnimation(TranslateTransform.YProperty, jump);

                OdySpeechText.Text = "Hadi yeni bir rota çizelim! 🚀";
            }
            catch { }
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