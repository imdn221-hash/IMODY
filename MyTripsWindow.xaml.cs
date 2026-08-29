using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using IMODY.Models;
using IMODY.Services;

namespace IMODY
{
    public partial class MyTripsWindow : Window
    {
        private List<TravelMemory> memories = new List<TravelMemory>();
        private TravelMemory? selectedMemory = null;
        private int currentRating = 5;

        public MyTripsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyTheme(UserSession.CurrentTheme);
            LoadData();
        }

        private void LoadData()
        {
            memories = TravelMemoryService.LoadMemories();
            RefreshList();

            if (memories.Count > 0)
            {
                TripsListBox.SelectedIndex = 0;
            }
        }

        private void RefreshList()
        {
            TripsListBox.ItemsSource = null;
            TripsListBox.ItemsSource = memories;
            TripCountText.Text = $"Toplam {memories.Count} seyahat kayıtlı";
        }

        private void TripsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripsListBox.SelectedItem is TravelMemory memory)
            {
                selectedMemory = memory;
                DisplaySelectedMemory();
            }
        }

        private void DisplaySelectedMemory()
        {
            if (selectedMemory == null) return;

            TripTitleBox.Text = selectedMemory.Title;
            TripDestinationBox.Text = selectedMemory.Destination;
            TripDateRangeBox.Text = selectedMemory.DateRange;
            TripNotesBox.Text = selectedMemory.Notes;
            currentRating = selectedMemory.Rating;
            UpdateStarDisplay(currentRating);

            RefreshPhotoSlots();
        }

        private void RefreshPhotoSlots()
        {
            if (selectedMemory == null) return;

            var images = new[] { Img1, Img2, Img3, Img4, Img5 };
            var placeholders = new[] { Placeholder1, Placeholder2, Placeholder3, Placeholder4, Placeholder5 };
            var delBtns = new[] { DelBtn1, DelBtn2, DelBtn3, DelBtn4, DelBtn5 };

            if (selectedMemory.Photos == null)
                selectedMemory.Photos = new List<string>();

            int count = selectedMemory.Photos.Count;
            AlbumSlotCounter.Text = $"{count} / 5 Yüklendi";

            for (int i = 0; i < 5; i++)
            {
                if (i < count && File.Exists(selectedMemory.Photos[i]))
                {
                    try
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri(selectedMemory.Photos[i], UriKind.Absolute);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        images[i].Source = bmp;

                        images[i].Visibility = Visibility.Visible;
                        placeholders[i].Visibility = Visibility.Collapsed;
                        delBtns[i].Visibility = Visibility.Visible;
                    }
                    catch
                    {
                        images[i].Source = null;
                        images[i].Visibility = Visibility.Collapsed;
                        placeholders[i].Visibility = Visibility.Visible;
                        delBtns[i].Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    images[i].Source = null;
                    images[i].Visibility = Visibility.Collapsed;
                    placeholders[i].Visibility = Visibility.Visible;
                    delBtns[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UploadPhoto_Click(object sender, MouseButtonEventArgs e)
        {
            if (selectedMemory == null) return;

            if (selectedMemory.Photos == null)
                selectedMemory.Photos = new List<string>();

            if (selectedMemory.Photos.Count >= 5)
            {
                MessageBox.Show("Bu seyahat için maksimum 5 fotoğraf ekleyebilirsiniz.", "Albüm Dolu", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new OpenFileDialog
            {
                Title = "Seyahat Albümüne Fotoğraf Ekle",
                Filter = "Fotoğraf Dosyaları (*.jpg;*.jpeg;*.png;*.webp;*.bmp)|*.jpg;*.jpeg;*.png;*.webp;*.bmp|Tüm Dosyalar (*.*)|*.*",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                string savedPath = TravelMemoryService.CopyPhotoToAlbum(dialog.FileName);
                selectedMemory.Photos.Add(savedPath);
                TravelMemoryService.SaveMemories(memories);
                RefreshPhotoSlots();
                RefreshList();
                ShowSaveFeedback("Fotoğraf albüme eklendi!");
            }
        }

        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMemory == null || sender is not Button btn || btn.Tag == null) return;

            if (int.TryParse(btn.Tag.ToString(), out int slotIndex))
            {
                if (selectedMemory.Photos != null && slotIndex >= 0 && slotIndex < selectedMemory.Photos.Count)
                {
                    selectedMemory.Photos.RemoveAt(slotIndex);
                    TravelMemoryService.SaveMemories(memories);
                    RefreshPhotoSlots();
                    RefreshList();
                    ShowSaveFeedback("Fotoğraf kaldırıldı.");
                }
            }
        }

        private void ViewPhoto_Click(object sender, MouseButtonEventArgs e)
        {
            if (selectedMemory == null || sender is not Image img || img.Source == null) return;

            LightboxImage.Source = img.Source;
            LightboxModal.Visibility = Visibility.Visible;
        }

        private void CloseLightbox_Click(object sender, RoutedEventArgs e)
        {
            LightboxModal.Visibility = Visibility.Collapsed;
        }

        private void CloseLightbox_Click(object sender, MouseButtonEventArgs e)
        {
            LightboxModal.Visibility = Visibility.Collapsed;
        }

        private void Star_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.Tag != null && int.TryParse(tb.Tag.ToString(), out int starVal))
            {
                currentRating = starVal;
                if (selectedMemory != null)
                {
                    selectedMemory.Rating = currentRating;
                }
                UpdateStarDisplay(currentRating);
            }
        }

        private void UpdateStarDisplay(int rating)
        {
            var stars = new[] { Star1, Star2, Star3, Star4, Star5 };
            var accentBrush = (Brush)Application.Current.Resources["Theme_Accent"];
            var grayBrush = new SolidColorBrush(Color.FromArgb(120, 150, 150, 150));

            for (int i = 0; i < 5; i++)
            {
                if (i < rating)
                {
                    stars[i].Text = "★";
                    stars[i].Foreground = accentBrush;
                }
                else
                {
                    stars[i].Text = "☆";
                    stars[i].Foreground = grayBrush;
                }
            }
        }

        private void SaveTrip_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMemory == null) return;

            selectedMemory.Title = TripTitleBox.Text.Trim();
            selectedMemory.Destination = TripDestinationBox.Text.Trim();
            selectedMemory.DateRange = TripDateRangeBox.Text.Trim();
            selectedMemory.Notes = TripNotesBox.Text.Trim();
            selectedMemory.Rating = currentRating;

            TravelMemoryService.SaveMemories(memories);
            RefreshList();
            ShowSaveFeedback("✓ Seyahat notları & anılar kaydedildi!");
        }

        private void AddNewTrip_Click(object sender, RoutedEventArgs e)
        {
            var newTrip = new TravelMemory
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Yeni Seyahat Macerası",
                Destination = "Şehir, Ülke",
                DateRange = DateTime.Now.ToString("dd MMMM yyyy"),
                Rating = 5,
                Notes = "Bu seyahatle ilgili unutulmaz anılarını, lezzetleri ve notlarını buraya yaz...",
                Photos = new List<string>(),
                CreatedAt = DateTime.Now
            };

            memories.Insert(0, newTrip);
            TravelMemoryService.SaveMemories(memories);
            RefreshList();
            TripsListBox.SelectedIndex = 0;
            TripTitleBox.Focus();
            TripTitleBox.SelectAll();
        }

        private void DeleteTrip_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMemory == null) return;

            var result = MessageBox.Show($"'{selectedMemory.Title}' seyahatini ve tüm anılarını silmek istediğinize emin misiniz?", "Seyahati Sil", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                memories.Remove(selectedMemory);
                TravelMemoryService.SaveMemories(memories);
                RefreshList();
                if (memories.Count > 0)
                    TripsListBox.SelectedIndex = 0;
                else
                    selectedMemory = null;
            }
        }

        private void ShowSaveFeedback(string message)
        {
            SaveFeedbackText.Text = message;
            var anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(200));
            SaveFeedbackText.BeginAnimation(OpacityProperty, anim);

            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(2.5) };
            timer.Tick += (s, ev) =>
            {
                var fadeOut = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(300));
                SaveFeedbackText.BeginAnimation(OpacityProperty, fadeOut);
                timer.Stop();
            };
            timer.Start();
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
