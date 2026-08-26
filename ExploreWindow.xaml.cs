using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IMODY
{
    public partial class ExploreWindow : Window
    {
        private string currentFilter = "ALL";

        public ExploreWindow()
        {
            InitializeComponent();
            LoadPosts();
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
                EventDate = string.IsNullOrWhiteSpace(date) || date.StartsWith("Tarih") ? "Yakında" : date,
                PriceInfo = string.IsNullOrWhiteSpace(price) || price.StartsWith("Ücret") ? "Ücretsiz" : price,
                CreatedAt = DateTime.Now
            };

            PostService.Add(post);

            MessageBox.Show("Etkinlik / Keşif başarıyla yayına alındı!", "Tebrikler", MessageBoxButton.OK, MessageBoxImage.Information);

            AddEventBorder.Visibility = Visibility.Collapsed;
            NewTitleTextBox.Text = "Etkinlik Başlığı (Örn: Travis Scott Konseri)";
            NewContentTextBox.Text = "Etkinlik veya keşif detaylarını buraya yazın...";

            LoadPosts();
        }

        private void FilterAll_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = "ALL";
            LoadPosts();
        }

        private void FilterMunicipality_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = "Belediye";
            LoadPosts();
        }

        private void FilterConcert_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = "Konser";
            LoadPosts();
        }

        private void FilterTravelers_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = "Gezgin";
            LoadPosts();
        }

        private void LoadPosts()
        {
            PostsPanel.Children.Clear();
            List<ExplorePost> posts = PostService.GetAll();

            if (currentFilter != "ALL")
            {
                posts = posts.Where(p => p.Category.Contains(currentFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (posts.Count == 0)
            {
                PostsPanel.Children.Add(new TextBlock
                {
                    Text = "Bu kategoride henüz bir etkinlik veya keşif bulunmuyor.",
                    Foreground = new SolidColorBrush(Color.FromRgb(217, 166, 168)),
                    FontSize = 14,
                    Margin = new Thickness(0, 20, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                return;
            }

            foreach (ExplorePost post in posts)
            {
                Border card = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(59, 12, 30)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(125, 64, 83)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(14),
                    Padding = new Thickness(16),
                    Margin = new Thickness(0, 0, 0, 14)
                };

                StackPanel mainStack = new StackPanel();

                Grid topGrid = new Grid();
                topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel badgeStack = new StackPanel { Orientation = Orientation.Horizontal };

                Border categoryBadge = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(101, 22, 47)),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8, 3, 8, 3),
                    Margin = new Thickness(0, 0, 8, 0)
                };
                categoryBadge.Child = new TextBlock
                {
                    Text = post.Category,
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(214, 164, 90))
                };
                badgeStack.Children.Add(categoryBadge);

                if (!string.IsNullOrWhiteSpace(post.City))
                {
                    Border cityBadge = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(43, 7, 23)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(125, 64, 83)),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(6),
                        Padding = new Thickness(8, 3, 8, 3)
                    };
                    cityBadge.Child = new TextBlock
                    {
                        Text = $"📍 {post.City}",
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Color.FromRgb(245, 233, 221))
                    };
                    badgeStack.Children.Add(cityBadge);
                }

                topGrid.Children.Add(badgeStack);
                Grid.SetColumn(badgeStack, 0);

                StackPanel rightInfo = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
                if (!string.IsNullOrWhiteSpace(post.EventDate))
                {
                    rightInfo.Children.Add(new TextBlock
                    {
                        Text = $"📅 {post.EventDate}  •  ",
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Color.FromRgb(217, 166, 168))
                    });
                }
                rightInfo.Children.Add(new TextBlock
                {
                    Text = $"🏷️ {post.PriceInfo}",
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(214, 164, 90))
                });
                topGrid.Children.Add(rightInfo);
                Grid.SetColumn(rightInfo, 1);

                mainStack.Children.Add(topGrid);

                TextBlock titleText = new TextBlock
                {
                    Text = post.Title,
                    FontSize = 17,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(245, 233, 221)),
                    Margin = new Thickness(0, 8, 0, 2)
                };
                mainStack.Children.Add(titleText);

                TextBlock authorText = new TextBlock
                {
                    Text = $"Organizatör: {post.AuthorName}",
                    FontSize = 11,
                    FontStyle = FontStyles.Italic,
                    Foreground = new SolidColorBrush(Color.FromRgb(175, 160, 165)),
                    Margin = new Thickness(0, 0, 0, 8)
                };
                mainStack.Children.Add(authorText);

                TextBlock contentText = new TextBlock
                {
                    Text = post.Content,
                    FontSize = 13,
                    LineHeight = 18,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(Color.FromRgb(245, 233, 221))
                };
                mainStack.Children.Add(contentText);

                card.Child = mainStack;
                PostsPanel.Children.Add(card);
            }
        }
    }
}