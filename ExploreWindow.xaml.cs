using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace IMODY
{
    public partial class ExploreWindow : Window
    {
        private string currentFilter = "ALL";

        public ExploreWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(UserSession.CurrentTheme);
            LoadPosts();
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
                    Duration = TimeSpan.FromSeconds(22),
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToggleAddEventPanel_Click(object sender, RoutedEventArgs e)
        {
            AddEventBorder.Visibility = AddEventBorder.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void CancelAddEvent_Click(object sender, RoutedEventArgs e)
        {
            AddEventBorder.Visibility = Visibility.Collapsed;
        }

        private void SubmitEvent_Click(object sender, RoutedEventArgs e)
        {
            string title = NewTitleTextBox.Text.Trim();
            string author = NewAuthorTextBox.Text.Trim();
            string city = NewCityTextBox.Text.Trim();
            string content = NewContentTextBox.Text.Trim();
            string date = NewDateTextBox.Text.Trim();
            string price = NewPriceTextBox.Text.Trim();
            string category = (NewCategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "🏛️ Belediye / Festival";

            if (string.IsNullOrWhiteSpace(title) || title.StartsWith("Etkinlik Başlığı") ||
                string.IsNullOrWhiteSpace(content) || content.StartsWith("Etkinlik veya"))
            {
                MessageBox.Show("Lütfen etkinlik başlığını ve açıklamasını doldurun.", "Bilgi Eksik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var post = new ExplorePost
            {
                Title = title,
                AuthorName = string.IsNullOrWhiteSpace(author) || author.StartsWith("Kurum") ? "Yerel Organizatör" : author,
                City = string.IsNullOrWhiteSpace(city) || city.StartsWith("Şehir") ? "Genel" : city,
                Category = category,
                Content = content,
                EventDate = string.IsNullOrWhiteSpace(date) || date.StartsWith("Tarih") ? "Tarih Belirtilmedi" : date,
                PriceInfo = string.IsNullOrWhiteSpace(price) || price.StartsWith("Ücret") ? "Ücretsiz" : price,
                CreatedAt = DateTime.Now
            };

            PostService.AddPost(post);

            NewTitleTextBox.Text = "Etkinlik Başlığı";
            NewAuthorTextBox.Text = "Kurum / Organizatör";
            NewCityTextBox.Text = "Şehir";
            NewDateTextBox.Text = "Tarih & Saat";
            NewPriceTextBox.Text = "Ücret (Örn: Ücretsiz)";
            NewContentTextBox.Text = "Etkinlik veya keşif detaylarını buraya yazın...";

            AddEventBorder.Visibility = Visibility.Collapsed;
            LoadPosts();

            MessageBox.Show("Etkinliğiniz başarıyla yayına alındı! Keşfet akışında tüm gezginlerle paylaşıldı. 🚀", "Yayında", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FilterAll_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = "ALL";
            LoadPosts();
        }

        private void FilterMunicipality_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = "MUNICIPALITY";
            LoadPosts();
        }

        private void FilterConcert_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = "CONCERT";
            LoadPosts();
        }

        private void FilterTravelers_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = "TRAVELER";
            LoadPosts();
        }

        private void LoadPosts()
        {
            PostsPanel.Children.Clear();
            var allPosts = PostService.GetPosts();

            IEnumerable<ExplorePost> filtered = allPosts;
            if (currentFilter == "MUNICIPALITY")
                filtered = allPosts.Where(p => p.Category.Contains("Belediye") || p.Category.Contains("Festival"));
            else if (currentFilter == "CONCERT")
                filtered = allPosts.Where(p => p.Category.Contains("Konser") || p.Category.Contains("Gösteri"));
            else if (currentFilter == "TRAVELER")
                filtered = allPosts.Where(p => p.Category.Contains("Gezgin") || p.Category.Contains("Doğa"));

            foreach (var post in filtered)
            {
                PostsPanel.Children.Add(CreatePostCard(post));
            }
        }

        private Border CreatePostCard(ExplorePost post)
        {
            var card = new Border
            {
                Background = (Brush)Application.Current.Resources["Theme_BgCard"],
                BorderBrush = (Brush)Application.Current.Resources["Theme_Border"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(16),
                Padding = new Thickness(18),
                Margin = new Thickness(0, 0, 0, 14)
            };

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };

            var authorText = new TextBlock
            {
                Text = post.AuthorName,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Foreground = (Brush)Application.Current.Resources["Theme_TextPrimary"],
                VerticalAlignment = VerticalAlignment.Center
            };
            headerPanel.Children.Add(authorText);

            if (post.IsOfficial)
            {
                var verifiedBadge = new TextBlock
                {
                    Text = " ✓ Onaylı Kurum",
                    Foreground = (Brush)Application.Current.Resources["Theme_Accent"],
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(4, 0, 0, 0)
                };
                headerPanel.Children.Add(verifiedBadge);
            }

            var cityBadge = new Border
            {
                Background = (Brush)Application.Current.Resources["Theme_BgSub"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8, 2, 8, 2),
                Margin = new Thickness(12, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock
                {
                    Text = $"📍 {post.City}",
                    FontSize = 11,
                    Foreground = (Brush)Application.Current.Resources["Theme_TextSecondary"]
                }
            };
            headerPanel.Children.Add(cityBadge);

            var categoryBadge = new Border
            {
                Background = (Brush)Application.Current.Resources["Theme_BgSub"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8, 2, 8, 2),
                Margin = new Thickness(6, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock
                {
                    Text = post.Category,
                    FontSize = 11,
                    Foreground = (Brush)Application.Current.Resources["Theme_Accent"]
                }
            };
            headerPanel.Children.Add(categoryBadge);

            Grid.SetRow(headerPanel, 0);
            mainGrid.Children.Add(headerPanel);

            var contentPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };
            var titleText = new TextBlock
            {
                Text = post.Title,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)Application.Current.Resources["Theme_TextPrimary"],
                Margin = new Thickness(0, 0, 0, 4)
            };
            var bodyText = new TextBlock
            {
                Text = post.Content,
                FontSize = 12.5,
                Foreground = (Brush)Application.Current.Resources["Theme_TextSecondary"],
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 18
            };
            contentPanel.Children.Add(titleText);
            contentPanel.Children.Add(bodyText);

            Grid.SetRow(contentPanel, 1);
            mainGrid.Children.Add(contentPanel);

            var footerGrid = new Grid();
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var infoPanel = new StackPanel { Orientation = Orientation.Horizontal };
            if (!string.IsNullOrWhiteSpace(post.EventDate))
            {
                infoPanel.Children.Add(new TextBlock
                {
                    Text = $"🗓️ {post.EventDate}",
                    FontSize = 11.5,
                    Foreground = (Brush)Application.Current.Resources["Theme_TextSecondary"],
                    Margin = new Thickness(0, 0, 14, 0)
                });
            }
            if (!string.IsNullOrWhiteSpace(post.PriceInfo))
            {
                infoPanel.Children.Add(new TextBlock
                {
                    Text = $"🎟️ {post.PriceInfo}",
                    FontSize = 11.5,
                    Foreground = (Brush)Application.Current.Resources["Theme_Accent"],
                    FontWeight = FontWeights.SemiBold
                });
            }
            Grid.SetColumn(infoPanel, 0);
            footerGrid.Children.Add(infoPanel);

            var likeButton = new Button
            {
                Content = $"❤️ {post.LikesCount}",
                Background = Brushes.Transparent,
                Foreground = (Brush)Application.Current.Resources["Theme_TextSecondary"],
                BorderThickness = new Thickness(0),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = post.Id
            };
            likeButton.Click += (s, e) =>
            {
                post.LikesCount++;
                likeButton.Content = $"❤️ {post.LikesCount}";
                likeButton.Foreground = (Brush)Application.Current.Resources["Theme_Accent"];
            };
            Grid.SetColumn(likeButton, 1);
            footerGrid.Children.Add(likeButton);

            Grid.SetRow(footerGrid, 2);
            mainGrid.Children.Add(footerGrid);

            card.Child = mainGrid;
            return card;
        }
    }
}