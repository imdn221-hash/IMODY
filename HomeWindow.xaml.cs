using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IMODY
{
    public partial class HomeWindow : Window
    {
        private AxisAngleRotation3D rotationX;
        private AxisAngleRotation3D rotationY;
        private DispatcherTimer timer;

        private readonly Random random = new();
        private double rotationSpeed = 0.4;
        private bool isSpinning;
        private bool isExploreMode;
        private List<GeoPin> geoPins = new();

        private class GeoPin
        {
            public string Name { get; set; } = "";
            public string DisplayText { get; set; } = "";
            public double Lat { get; set; }
            public double Lon { get; set; }
            public bool IsCity { get; set; }
            public Button? Element { get; set; }
        }

        // Fare ile serbest çevirme
        private bool isDragging;
        private Point lastMousePos;

        private const string WelcomeMessage = "Sıradaki Rotamız Neresi?";
        private const double LetterFontSize = 74;
        private const double LetterSpacing = 2;

        public HomeWindow()
        {
            InitializeComponent();

            // 3D Dönüş Matrisi: Y ekseninde dönüş + Dünyanın doğal 23.4° eksen eğikliği
            rotationY = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            rotationX = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            var tiltZ = new AxisAngleRotation3D(new Vector3D(0, 0, 1), -23.4);

            Transform3DGroup transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new RotateTransform3D(rotationY));
            transformGroup.Children.Add(new RotateTransform3D(rotationX));
            transformGroup.Children.Add(new RotateTransform3D(tiltZ));

            EarthModelGroup.Transform = transformGroup;

            InitializeGeoPins();

            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(25) };
            timer.Tick += RotateWorld;
            timer.Start();
        }

        private void InitializeGeoPins()
        {
            geoPins = new List<GeoPin>
            {
                // TÜRKİYE & ŞEHİRLERİ
                new GeoPin { Name = "Türkiye", DisplayText = "🇹🇷 Türkiye", Lat = 39.0, Lon = 35.0, IsCity = false },
                new GeoPin { Name = "İstanbul", DisplayText = "📍 İstanbul", Lat = 41.0, Lon = 28.9, IsCity = true },
                new GeoPin { Name = "Kapadokya", DisplayText = "📍 Kapadokya", Lat = 38.6, Lon = 34.8, IsCity = true },
                new GeoPin { Name = "İzmir", DisplayText = "📍 İzmir", Lat = 38.4, Lon = 27.1, IsCity = true },

                // AVUSTRALYA & ŞEHİRLERİ
                new GeoPin { Name = "Avustralya", DisplayText = "🇦🇺 Avustralya", Lat = -25.2, Lon = 133.7, IsCity = false },
                new GeoPin { Name = "Sidney", DisplayText = "📍 Sidney", Lat = -33.8, Lon = 151.2, IsCity = true },
                new GeoPin { Name = "Melbourne", DisplayText = "📍 Melbourne", Lat = -37.8, Lon = 144.9, IsCity = true },

                // JAMAIKA & ŞEHİRLERİ
                new GeoPin { Name = "Jamaika", DisplayText = "🇯🇲 Jamaika", Lat = 18.1, Lon = -77.2, IsCity = false },
                new GeoPin { Name = "Kingston", DisplayText = "📍 Kingston", Lat = 17.9, Lon = -76.8, IsCity = true },

                // İTALYA & ŞEHİRLERİ
                new GeoPin { Name = "İtalya", DisplayText = "🇮🇹 İtalya", Lat = 42.5, Lon = 12.5, IsCity = false },
                new GeoPin { Name = "Roma", DisplayText = "📍 Roma", Lat = 41.9, Lon = 12.5, IsCity = true },
                new GeoPin { Name = "Venedik", DisplayText = "📍 Venedik", Lat = 45.4, Lon = 12.3, IsCity = true },

                // İNGİLTERE & ŞEHİRLERİ
                new GeoPin { Name = "İngiltere", DisplayText = "🇬🇧 İngiltere", Lat = 53.0, Lon = -1.5, IsCity = false },
                new GeoPin { Name = "Londra", DisplayText = "📍 Londra", Lat = 51.5, Lon = -0.1, IsCity = true },

                // FRANSA & ŞEHİRLERİ
                new GeoPin { Name = "Fransa", DisplayText = "🇫🇷 Fransa", Lat = 46.6, Lon = 2.2, IsCity = false },
                new GeoPin { Name = "Paris", DisplayText = "📍 Paris", Lat = 48.8, Lon = 2.3, IsCity = true },

                // İSPANYA & ŞEHİRLERİ
                new GeoPin { Name = "İspanya", DisplayText = "🇪🇸 İspanya", Lat = 40.4, Lon = -3.7, IsCity = false },
                new GeoPin { Name = "Barselona", DisplayText = "📍 Barselona", Lat = 41.3, Lon = 2.1, IsCity = true },

                // JAPONYA & ŞEHİRLERİ
                new GeoPin { Name = "Japonya", DisplayText = "🇯🇵 Japonya", Lat = 36.2, Lon = 138.2, IsCity = false },
                new GeoPin { Name = "Tokyo", DisplayText = "📍 Tokyo", Lat = 35.6, Lon = 139.6, IsCity = true },
                new GeoPin { Name = "Kyoto", DisplayText = "📍 Kyoto", Lat = 35.0, Lon = 135.7, IsCity = true },

                // ABD & ŞEHİRLERİ
                new GeoPin { Name = "ABD", DisplayText = "🇺🇸 ABD", Lat = 38.0, Lon = -97.0, IsCity = false },
                new GeoPin { Name = "New York", DisplayText = "📍 New York", Lat = 40.7, Lon = -74.0, IsCity = true },
                new GeoPin { Name = "Los Angeles", DisplayText = "📍 Los Angeles", Lat = 34.0, Lon = -118.2, IsCity = true },

                // BREZİLYA & ŞEHİRLERİ
                new GeoPin { Name = "Brezilya", DisplayText = "🇧🇷 Brezilya", Lat = -14.2, Lon = -51.9, IsCity = false },
                new GeoPin { Name = "Rio de Janeiro", DisplayText = "📍 Rio", Lat = -22.9, Lon = -43.1, IsCity = true },

                // MISIR & ŞEHİRLERİ
                new GeoPin { Name = "Mısır", DisplayText = "🇪🇬 Mısır", Lat = 26.8, Lon = 30.8, IsCity = false },
                new GeoPin { Name = "Kahire", DisplayText = "📍 Kahire", Lat = 30.0, Lon = 31.2, IsCity = true },

                // DİĞER POPÜLER ÜLKELER
                new GeoPin { Name = "İsviçre", DisplayText = "🇨🇭 İsviçre", Lat = 46.8, Lon = 8.2, IsCity = false },
                new GeoPin { Name = "Zürih", DisplayText = "📍 Zürih", Lat = 47.3, Lon = 8.5, IsCity = true },
                new GeoPin { Name = "Norveç", DisplayText = "🇳🇴 Norveç", Lat = 60.4, Lon = 8.4, IsCity = false },
                new GeoPin { Name = "Yunanistan", DisplayText = "🇬🇷 Yunanistan", Lat = 39.0, Lon = 21.8, IsCity = false },
                new GeoPin { Name = "BAE", DisplayText = "🇦🇪 Dubai", Lat = 25.2, Lon = 55.2, IsCity = false },
                new GeoPin { Name = "İzlanda", DisplayText = "🇮🇸 İzlanda", Lat = 64.9, Lon = -19.0, IsCity = false },
                new GeoPin { Name = "Tayland", DisplayText = "🇹🇭 Tayland", Lat = 15.8, Lon = 100.9, IsCity = false },
                new GeoPin { Name = "Güney Afrika", DisplayText = "🇿🇦 G. Afrika", Lat = -30.5, Lon = 22.9, IsCity = false },
                new GeoPin { Name = "Kanada", DisplayText = "🇨🇦 Kanada", Lat = 56.1, Lon = -106.3, IsCity = false },
                new GeoPin { Name = "Almanya", DisplayText = "🇩🇪 Almanya", Lat = 51.1, Lon = 10.4, IsCity = false }
            };

            foreach (var pin in geoPins)
            {
                Button btn = new Button
                {
                    Content = pin.DisplayText,
                    Style = (Style)FindResource(pin.IsCity ? "CityPinStyle" : "CountryPinStyle"),
                    Tag = pin.Name,
                    Visibility = Visibility.Collapsed
                };

                btn.Click += (s, e) =>
                {
                    SearchTextBox.Text = pin.Name;
                    ExitExploreMode_Click(s, e);
                };

                pin.Element = btn;
                ExplorePinsCanvas.Children.Add(btn);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(UserSession.CurrentTheme);
            GenerateStars();
            Build3DEarth();
            RoutePanel.Opacity = 0;
            WelcomeAnimationCanvas.Children.Clear();
            StartWelcomeAnimation();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GenerateStars();
        }

        private void GenerateStars()
        {
            try
            {
                SpaceBackgroundCanvas.Children.Clear();
                double w = ActualWidth > 200 ? ActualWidth : 1600;
                double h = ActualHeight > 200 ? ActualHeight : 1000;
                var rand = new Random();
                int starCount = 140; // Tüm ekranda eşit, derin ve dengeli yıldız dağılımı

                for (int i = 0; i < starCount; i++)
                {
                    double size = rand.NextDouble() * 2.2 + 0.8;
                    Ellipse star = new Ellipse
                    {
                        Width = size,
                        Height = size,
                        Fill = new SolidColorBrush(Color.FromArgb((byte)rand.Next(160, 255), 240, 248, 255)),
                        Opacity = rand.NextDouble() * 0.7 + 0.2
                    };

                    Canvas.SetLeft(star, rand.NextDouble() * w);
                    Canvas.SetTop(star, rand.NextDouble() * h);

                    // Yıldız yanıp sönme animasyonu (Twinkling)
                    var anim = new DoubleAnimation
                    {
                        From = star.Opacity,
                        To = rand.NextDouble() * 0.15 + 0.05,
                        Duration = TimeSpan.FromSeconds(rand.NextDouble() * 2.5 + 1.2),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    star.BeginAnimation(UIElement.OpacityProperty, anim);

                    SpaceBackgroundCanvas.Children.Add(star);
                }
            }
            catch { }
        }

        // =========================================================
        // 3D DÜNYA KÜRESİ VE DOKU KAPLAMASI (YÜKSEK ÇÖZÜNÜRLÜK & SİNEMATİK)
        // =========================================================
        private void Build3DEarth()
        {
            try
            {
                // 80x80 yüksek poligon sayısı ile pürüzsüz küre
                MeshGeometry3D mesh = GenerateSphereMesh(1.35, 80, 80);
                EarthGeometryModel.Geometry = mesh;

                // Yüksek kaliteli dünya kaplaması
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Resources/earth.jpeg", UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImageBrush brush = new ImageBrush(bitmap)
                {
                    TileMode = TileMode.None,
                    Stretch = Stretch.Fill,
                    ViewportUnits = BrushMappingMode.Absolute
                };

                DiffuseMaterial diffuse = new DiffuseMaterial(brush);
                // Okyanus parlaması (Specular)
                SpecularMaterial specular = new SpecularMaterial(new SolidColorBrush(Color.FromArgb(90, 220, 240, 255)), 40);

                MaterialGroup group = new MaterialGroup();
                group.Children.Add(diffuse);
                group.Children.Add(specular);

                EarthGeometryModel.Material = group;
                EarthGeometryModel.BackMaterial = diffuse;
            }
            catch
            {
                EarthGeometryModel.Geometry = GenerateSphereMesh(1.35, 40, 40);
                EarthGeometryModel.Material = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(35, 75, 115)));
            }
        }

        private MeshGeometry3D GenerateSphereMesh(double radius, int latDivs, int longDivs)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int lat = 0; lat <= latDivs; lat++)
            {
                double theta = lat * Math.PI / latDivs;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int lon = 0; lon <= longDivs; lon++)
                {
                    double phi = lon * 2 * Math.PI / longDivs;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    // Gerçekçi koordinat eşleme
                    double x = radius * sinTheta * Math.Cos(phi);
                    double y = radius * cosTheta;
                    double z = -radius * sinTheta * Math.Sin(phi);

                    mesh.Positions.Add(new Point3D(x, y, z));
                    mesh.Normals.Add(new Vector3D(x / radius, y / radius, z / radius));

                    // Doku koordinatları (Kıta yönleri düzgün ve net)
                    double u = (double)lon / longDivs;
                    double v = (double)lat / latDivs;
                    mesh.TextureCoordinates.Add(new Point(u, v));
                }
            }

            for (int lat = 0; lat < latDivs; lat++)
            {
                for (int lon = 0; lon < longDivs; lon++)
                {
                    int current = lat * (longDivs + 1) + lon;
                    int next = current + longDivs + 1;

                    mesh.TriangleIndices.Add(current);
                    mesh.TriangleIndices.Add(current + 1);
                    mesh.TriangleIndices.Add(next);

                    mesh.TriangleIndices.Add(next);
                    mesh.TriangleIndices.Add(current + 1);
                    mesh.TriangleIndices.Add(next + 1);
                }
            }

            return mesh;
        }

        // =========================================================
        // FARE İLE DÜNYAYI ÇEVİRME VE YAKINLAŞTIRMA (ZOOM)
        // =========================================================
        private void WorldView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            lastMousePos = e.GetPosition(WorldViewport);
            WorldViewport.CaptureMouse();
        }

        private void WorldView_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = e.GetPosition(WorldViewport);
                double deltaX = currentPos.X - lastMousePos.X;
                double deltaY = currentPos.Y - lastMousePos.Y;

                rotationY.Angle += deltaX * 0.45;
                rotationX.Angle += deltaY * 0.3;

                lastMousePos = currentPos;

                if (isExploreMode)
                    UpdateGeospatialPins();
            }
        }

        private void WorldView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            WorldViewport.ReleaseMouseCapture();
        }

        private void WorldView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double currentZ = WorldCamera.Position.Z;
            // Tekerlek yukarı -> Yakınlaş (Z küçülür)
            // Tekerlek aşağı -> Uzaklaş (Z büyür)
            double delta = (e.Delta > 0) ? -0.3 : 0.3;
            double newZ = Math.Clamp(currentZ + delta, 1.8, 4.5);

            WorldCamera.Position = new Point3D(0, 0, newZ);
            WorldCamera.LookDirection = new Vector3D(0, 0, -newZ);

            if (isExploreMode)
                UpdateGeospatialPins();
        }

        private void RotateWorld(object? sender, EventArgs e)
        {
            if (!isDragging)
            {
                rotationY.Angle += rotationSpeed;
                if (rotationY.Angle >= 360)
                    rotationY.Angle -= 360;

                if (isExploreMode)
                    UpdateGeospatialPins();
            }
        }

        // =========================================================
        // DİNAMİK 3D COĞRAFİ PROJEKSİYON (DÜNYA DÖNDÜKÇE KONUMUNDA ÇIKAR)
        // =========================================================
        private void UpdateGeospatialPins()
        {
            if (!isExploreMode || WorldViewport.ActualWidth <= 0 || WorldViewport.ActualHeight <= 0) return;

            double radius = 1.35;
            double camZ = WorldCamera.Position.Z;
            bool showCities = camZ < 2.9;

            double radY = rotationY.Angle * Math.PI / 180.0;
            double radX = rotationX.Angle * Math.PI / 180.0;
            double radZ = -23.4 * Math.PI / 180.0;

            double fovRad = WorldCamera.FieldOfView * Math.PI / 180.0;
            double viewportH = WorldViewport.ActualHeight;
            double viewportW = WorldViewport.ActualWidth;
            double scale = (viewportH / 2.0) / Math.Tan(fovRad / 2.0);

            foreach (var pin in geoPins)
            {
                if (pin.Element == null) continue;

                // Zoom seviyesine göre Ülke / Şehir filtreleme
                if (showCities && !pin.IsCity)
                {
                    pin.Element.Visibility = Visibility.Collapsed;
                    continue;
                }
                if (!showCities && pin.IsCity)
                {
                    pin.Element.Visibility = Visibility.Collapsed;
                    continue;
                }

                // 1. Küre üzerindeki ham 3D nokta (Gerçek Lat/Lon)
                double theta = (90.0 - pin.Lat) * Math.PI / 180.0;
                double phi = pin.Lon * Math.PI / 180.0;

                double x0 = radius * Math.Sin(theta) * Math.Cos(phi);
                double y0 = radius * Math.Cos(theta);
                double z0 = -radius * Math.Sin(theta) * Math.Sin(phi);

                // 2. Y ekseninde dönüş (Dünyanın kendi etrafında dönüşü)
                double x1 = x0 * Math.Cos(radY) + z0 * Math.Sin(radY);
                double y1 = y0;
                double z1 = -x0 * Math.Sin(radY) + z0 * Math.Cos(radY);

                // 3. X ekseninde dönüş (Kullanıcının yukarı-aşağı eğmesi)
                double x2 = x1;
                double y2 = y1 * Math.Cos(radX) - z1 * Math.Sin(radX);
                double z2 = y1 * Math.Sin(radX) + z1 * Math.Cos(radX);

                // 4. Dünyanın 23.4° eksen eğikliği (Z ekseni)
                double x3 = x2 * Math.Cos(radZ) - y2 * Math.Sin(radZ);
                double y3 = x2 * Math.Sin(radZ) + y2 * Math.Cos(radZ);
                double z3 = z2;

                // 5. ÖN / ARKA KONTROLÜ: Sadece kameraya bakan (öndeki) yarımkürede olanlar görünür!
                if (z3 > 0.15)
                {
                    double dist = camZ - z3;
                    if (dist > 0.1)
                    {
                        double screenX = (viewportW / 2.0) + (x3 / dist) * scale;
                        double screenY = (viewportH / 2.0) - (y3 / dist) * scale;

                        double btnWidth = pin.Element.ActualWidth > 0 ? pin.Element.ActualWidth : 80;
                        double btnHeight = pin.Element.ActualHeight > 0 ? pin.Element.ActualHeight : 28;

                        Canvas.SetLeft(pin.Element, screenX - (btnWidth / 2.0));
                        Canvas.SetTop(pin.Element, screenY - (btnHeight / 2.0));
                        pin.Element.Visibility = Visibility.Visible;
                        pin.Element.Opacity = Math.Clamp((z3 - 0.15) / 0.5, 0.25, 1.0);
                    }
                }
                else
                {
                    // Dünyanın arkasında kalan taraf gizlenir
                    pin.Element.Visibility = Visibility.Collapsed;
                }
            }

            ZoomStatusTextBlock.Text = showCities
                ? "🔍 Şehirler Görünümü (Tıkla ve Şehri Seç!)"
                : "🌎 Dünyayı fareyle çevir — Avustralya'dan Jamaika'ya her ülke tam coğrafi konumunda görünür!";
        }

        // =========================================================
        // SEÇENEK 1: SPIN THE WORLD (ARAYÜZ GİZLENİR, DÜNYA DÖNER)
        // =========================================================
        private async void SpinTheWorld_Click(object sender, MouseButtonEventArgs e)
        {
            if (isSpinning) return;
            isSpinning = true;

            RoutePanel.Visibility = Visibility.Collapsed;
            WelcomeAnimationCanvas.Visibility = Visibility.Collapsed;
            SpinInfoOverlay.Visibility = Visibility.Visible;
            SpinStatusTextBlock.Text = "Ody dünyayı döndürüyor...";

            rotationSpeed = 22;

            string[] cities = { "Roma", "Tokyo", "Paris", "Kapadokya", "Kyoto", "Barselona", "Londra", "Reykjavik", "Positano", "İstanbul", "Zürih", "Prag", "New York", "Floransa", "Atina", "Sidney", "Kingston", "Rio de Janeiro" };

            for (int i = 0; i < 15; i++)
            {
                SpinStatusTextBlock.Text = cities[random.Next(cities.Length)] + "...";
                await Task.Delay(120);
            }

            rotationSpeed = 3.0;
            await Task.Delay(600);
            rotationSpeed = 0.4;

            string finalCity = cities[random.Next(cities.Length)];
            SpinStatusTextBlock.Text = $"✦ {finalCity} Seçildi! ✦";
            await Task.Delay(1000);

            SpinInfoOverlay.Visibility = Visibility.Collapsed;
            RoutePanel.Visibility = Visibility.Visible;
            WelcomeAnimationCanvas.Visibility = Visibility.Visible;
            SearchTextBox.Text = finalCity;

            isSpinning = false;
        }

        // =========================================================
        // SEÇENEK 2: EXPLORE THE GLOBE (KULLANICI ÇEVİRİR & ZOOM YAPAR)
        // =========================================================
        private void ExploreTheGlobe_Click(object sender, MouseButtonEventArgs e)
        {
            isExploreMode = true;
            RoutePanel.Visibility = Visibility.Collapsed;
            WelcomeAnimationCanvas.Visibility = Visibility.Collapsed;

            ExplorePinsCanvas.Visibility = Visibility.Visible;
            ExploreBottomBar.Visibility = Visibility.Visible;

            // Varsayılan kamera uzaklığı (Ülke görünümü)
            WorldCamera.Position = new Point3D(0, 0, 3.3);
            WorldCamera.LookDirection = new Vector3D(0, 0, -3.3);
            UpdateGeospatialPins();
        }

        private void ExitExploreMode_Click(object sender, RoutedEventArgs e)
        {
            isExploreMode = false;
            ExplorePinsCanvas.Visibility = Visibility.Collapsed;
            ExploreBottomBar.Visibility = Visibility.Collapsed;

            // Kamerayı normale al
            WorldCamera.Position = new Point3D(0, 0, 3.3);
            WorldCamera.LookDirection = new Vector3D(0, 0, -3.3);

            RoutePanel.Visibility = Visibility.Visible;
            WelcomeAnimationCanvas.Visibility = Visibility.Visible;
        }

        // =========================================================
        // EL YAZISI ANİMASYONU
        // =========================================================
        private async void StartWelcomeAnimation()
        {
            await Task.Delay(200);
            await DrawWelcomeText();
            await Task.Delay(400);

            var moveUp = new DoubleAnimation
            {
                From = 0,
                To = -35,
                Duration = TimeSpan.FromMilliseconds(600),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            WelcomeTitleTransform.BeginAnimation(TranslateTransform.YProperty, moveUp);

            var showSearch = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(550),
                BeginTime = TimeSpan.FromMilliseconds(150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            RoutePanel.BeginAnimation(UIElement.OpacityProperty, showSearch);
        }

        private async Task DrawWelcomeText()
        {
            double totalWidth = 0;
            foreach (char character in WelcomeMessage)
            {
                FormattedText formattedText = CreateFormattedText(character.ToString());
                totalWidth += formattedText.Width + LetterSpacing;
            }

            double currentX = (WelcomeAnimationCanvas.Width - totalWidth) / 2;

            var textColor = (Brush)Application.Current.Resources["Theme_Accent"] ?? new SolidColorBrush(Color.FromRgb(214, 164, 90));

            foreach (char character in WelcomeMessage)
            {
                FormattedText formattedText = CreateFormattedText(character.ToString());
                Geometry geometry = formattedText.BuildGeometry(new Point(0, 0));

                System.Windows.Shapes.Path path = new System.Windows.Shapes.Path
                {
                    Data = geometry,
                    Fill = textColor,
                    Stroke = textColor,
                    StrokeThickness = 1.0,
                    StrokeLineJoin = PenLineJoin.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeDashArray = new DoubleCollection { 1 },
                    StrokeDashOffset = 1,
                    RenderTransform = new TranslateTransform(currentX, 0),
                    Opacity = 1
                };

                WelcomeAnimationCanvas.Children.Add(path);
                AnimatePath(path);

                currentX += formattedText.Width + LetterSpacing;
                await Task.Delay(80);
            }
        }

        private void AnimatePath(System.Windows.Shapes.Path path)
        {
            double length = GetGeometryLength(path.Data);
            if (length <= 0) length = 100;

            path.StrokeDashArray = new DoubleCollection { length, length };
            path.StrokeDashOffset = length;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = length,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(650),
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            path.BeginAnimation(System.Windows.Shapes.Shape.StrokeDashOffsetProperty, animation);
        }

        private double GetGeometryLength(Geometry geometry)
        {
            PathGeometry pathGeometry = geometry.GetFlattenedPathGeometry(0.5, ToleranceType.Absolute);
            double length = 0;

            foreach (PathFigure figure in pathGeometry.Figures)
            {
                Point previous = figure.StartPoint;
                foreach (PathSegment segment in figure.Segments)
                {
                    if (segment is LineSegment line)
                    {
                        length += Distance(previous, line.Point);
                        previous = line.Point;
                    }
                    else if (segment is PolyLineSegment polyLine)
                    {
                        foreach (Point point in polyLine.Points)
                        {
                            length += Distance(previous, point);
                            previous = point;
                        }
                    }
                }
                if (figure.IsClosed)
                    length += Distance(previous, figure.StartPoint);
            }
            return length;
        }

        private double Distance(Point a, Point b) => Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));

        private FormattedText CreateFormattedText(string text)
        {
            Typeface typeface = new Typeface(
                new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Mea Culpa"),
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);

            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, LetterFontSize, new SolidColorBrush(Color.FromRgb(180, 22, 69)), pixelsPerDip);
        }

        // =========================================================
        // ARAMA VE DOĞRULAMA (DESTİNASYON ZORUNLULUĞU)
        // =========================================================
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text.StartsWith("Bir şehir veya rota ara"))
                SearchTextBox.Text = "";
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                SearchTextBox.Text = "Bir şehir veya rota ara (Örn: Roma, Tokyo)...";
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ValidateAndProceed();
        }

        private void StartTripButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateAndProceed();
        }

        private void ValidateAndProceed()
        {
            string city = SearchTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(city) ||
                city.StartsWith("Bir şehir") ||
                city.Length < 2 ||
                Regex.IsMatch(city, @"^\d+$"))
            {
                MessageBox.Show(
                    "Lütfen geçerli bir seyahat şehri veya rota girin!\n(Örn: Roma, Tokyo, Kapadokya, İzmir, Londra)",
                    "Ody: Şehir Seçmelisin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var tr = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            city = tr.TextInfo.ToTitleCase(city.ToLower(tr));

            CreateTripWindow window = new CreateTripWindow(city);
            window.WindowState = this.WindowState;
            if (this.WindowState != WindowState.Maximized)
            {
                window.Left = this.Left;
                window.Top = this.Top;
                window.Width = this.Width;
                window.Height = this.Height;
            }
            window.Show();
            Close();
        }

        // =========================================================
        // MENÜ NAVİGASYONLARI
        // =========================================================
        private void CreateButton_Click(object sender, RoutedEventArgs e) => ValidateAndProceed();

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void MyTripsButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripsWindow myTrips = new MyTripsWindow { Owner = this };
            myTrips.ShowDialog();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profile = new ProfileWindow { Owner = this };
            profile.ShowDialog();
        }
    }
}