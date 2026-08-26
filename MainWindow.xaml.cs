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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(UserSession.CurrentTheme);
            SetTimeBasedGreeting();
            Setup3DGlobe();
            LoadPuppetImages();
            StartOdyPuppetAnimation();
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

                string earthPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "earth.jpeg");
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
        // KATMANLI ODY KUKLA İSKELET ANİMASYONU
        // =========================================================
        private void LoadPuppetImages()
        {
            LoadLayerImage(OdyTailImage, "ody_layer_tail.png");
            LoadLayerImage(OdyTorsoImage, "ody_layer_head_torso.png");
            LoadLayerImage(OdyArmImage, "ody_layer_compass_arm.png");
        }

        private void LoadLayerImage(System.Windows.Controls.Image imgElement, string fileName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
                if (File.Exists(path))
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(path, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    imgElement.Source = bmp;
                }
                else
                {
                    imgElement.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/{fileName}"));
                }
            }
            catch { }
        }

        private void StartOdyPuppetAnimation()
        {
            try
            {
                // 1. Kuyruk Yüzme Salınımı (Undulating Tail Wave)
                var tailAnim = new DoubleAnimation
                {
                    From = -14,
                    To = 14,
                    Duration = TimeSpan.FromSeconds(2.2),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                OdyTailRotate.BeginAnimation(RotateTransform.AngleProperty, tailAnim);

                // 2. Kol ve Pusula Sallama (Arm & Compass Sway)
                var armAnim = new DoubleAnimation
                {
                    From = -6,
                    To = 18,
                    Duration = TimeSpan.FromSeconds(2.8),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                OdyArmRotate.BeginAnimation(RotateTransform.AngleProperty, armAnim);

                // 3. Gövde & Kafa Salınımı (Torso & Head Natural Sway)
                var torsoAnim = new DoubleAnimation
                {
                    From = -3.5,
                    To = 3.5,
                    Duration = TimeSpan.FromSeconds(3.4),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                OdyTorsoRotate.BeginAnimation(RotateTransform.AngleProperty, torsoAnim);

                // 4. Nefes Alma & Göğüs Hareketi (Breathing Scale)
                var breatheAnim = new DoubleAnimation
                {
                    From = 0.985,
                    To = 1.015,
                    Duration = TimeSpan.FromSeconds(2.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                OdyTorsoScale.BeginAnimation(ScaleTransform.ScaleXProperty, breatheAnim);
                OdyTorsoScale.BeginAnimation(ScaleTransform.ScaleYProperty, breatheAnim);

                // 5. Genel Sualtı Süzülmesi (Vertical Floating Float)
                var floatAnim = new DoubleAnimation
                {
                    From = -10,
                    To = 10,
                    Duration = TimeSpan.FromSeconds(3.8),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                OdyFloatTranslate.BeginAnimation(TranslateTransform.YProperty, floatAnim);
            }
            catch { }
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
                OdyMasterScale.BeginAnimation(ScaleTransform.ScaleXProperty, zoom);
                OdyMasterScale.BeginAnimation(ScaleTransform.ScaleYProperty, zoom);

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
                OdyMasterScale.BeginAnimation(ScaleTransform.ScaleXProperty, zoomOut);
                OdyMasterScale.BeginAnimation(ScaleTransform.ScaleYProperty, zoomOut);

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
                OdyFloatTranslate.BeginAnimation(TranslateTransform.YProperty, jump);

                OdySpeechText.Text = "Hadi keşfe başlayalım! 🌟";
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