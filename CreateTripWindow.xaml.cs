using IMODY.Models;
using IMODY.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace IMODY
{
    public partial class CreateTripWindow : Window
    {
        private readonly string destination = "Roma";
        private int currentStep = 1;
        private const int DateStep = 1;
        private const int PeopleStep = 2;
        private const int AccommodationStep = 3;
        private const int InterestsStep = 4;
        private const int PlanStep = 5;

        private int selectedPeopleCount = 2;
        private string selectedAccommodation = "Yerim Var";
        private readonly List<string> conversationHistory = new();
        private bool isGenerating = false;

        private List<DaySchedule> parsedDays = new();
        private string allPhotoTips = "";
        private string allFoodDiscovery = "";
        private string allGameMode = "";
        private int activeDayIndex = 0;
        private string currentVideoFile = "";

        public CreateTripWindow(string destination)
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(destination))
            {
                var tr = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
                this.destination = tr.TextInfo.ToTitleCase(destination.Trim().ToLower(tr));
            }
            else
            {
                this.destination = "Roma";
            }

            HeaderDestinationText.Text = $"{this.destination.ToUpper(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"))} SEYAHATİ";

            DepartureDatePicker.SelectedDate = DateTime.Today.AddDays(7);
            ReturnDatePicker.SelectedDate = DateTime.Today.AddDays(10);

            ShowStep(DateStep);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SwitchBackgroundVideo("bg_ody_creating.mp4", 0.12);
        }

        private void SwitchBackgroundVideo(string videoFileName, double speedRatio = 0.12)
        {
            try
            {
                currentVideoFile = videoFileName;

                string videoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", videoFileName);
                if (!System.IO.File.Exists(videoPath))
                {
                    string alt = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, videoFileName);
                    if (System.IO.File.Exists(alt)) videoPath = alt;
                    else videoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "bg_ody_creating.mp4");
                }

                if (System.IO.File.Exists(videoPath))
                {
                    WaterVideoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
                    WaterVideoPlayer.IsMuted = true;
                    WaterVideoPlayer.Volume = 0;
                    WaterVideoPlayer.SpeedRatio = speedRatio;
                    WaterVideoPlayer.Play();
                }
            }
            catch { }
        }

        private void WaterVideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                WaterVideoPlayer.Position = TimeSpan.Zero;
                WaterVideoPlayer.Play();
            }
            catch { }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (isGenerating) return;

            if (currentStep == DateStep)
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
                return;
            }

            ShowStep(currentStep - 1);
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (isGenerating) return;

            if (currentStep == DateStep)
            {
                if (DepartureDatePicker.SelectedDate == null || ReturnDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Lütfen gidiş ve dönüş tarihlerinizi seçin.", "Eksik Bilgi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ShowStep(currentStep + 1);
            }
            else if (currentStep == InterestsStep)
            {
                ShowStep(PlanStep);
                await GeneratePlanAsync();
            }
            else
            {
                ShowStep(currentStep + 1);
            }
        }

        private bool ValidateCurrentStep()
        {
            if (currentStep == DateStep)
            {
                if (DepartureDatePicker.SelectedDate is null || ReturnDatePicker.SelectedDate is null)
                {
                    MessageBox.Show("Lütfen gidiş ve dönüş tarihlerini seçin.", "Ody: Tarih Seçimi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (ReturnDatePicker.SelectedDate.Value.Date < DepartureDatePicker.SelectedDate.Value.Date)
                {
                    MessageBox.Show("Dönüş tarihi gidiş tarihinden önce olamaz.", "Ody: Tarih Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void ShowStep(int step)
        {
            currentStep = step;
            HideAllPanels();
            BackButton.Visibility = Visibility.Visible;
            NextButton.Visibility = Visibility.Visible;
            NextButton.IsEnabled = true;

            StepBadge.Text = $"ADIM {step} / 5";
            UIElement? activePanel = null;

            // Çarkların döndüğü sinematik arka plan (Ultra Slow-Mo 0.12x)
            SwitchBackgroundVideo("bg_ody_creating.mp4", 0.12);

            switch (step)
            {
                case DateStep:
                    QuestionText.Text = $"{destination} için ne zaman yola çıkıyoruz?";
                    DatePanel.Visibility = Visibility.Visible;
                    activePanel = DatePanel;
                    NextButton.Content = "Devam Et →";
                    break;

                case PeopleStep:
                    QuestionText.Text = "Kaç kişi seyahat edeceksiniz?";
                    PeoplePanel.Visibility = Visibility.Visible;
                    activePanel = PeoplePanel;
                    NextButton.Content = "Devam Et →";
                    break;

                case AccommodationStep:
                    QuestionText.Text = "Konaklama durumun nasıl?";
                    AccommodationPanel.Visibility = Visibility.Visible;
                    activePanel = AccommodationPanel;
                    NextButton.Content = "Devam Et →";
                    break;

                case InterestsStep:
                    QuestionText.Text = "Ody ile Rota Sohbeti";
                    InterestChatPanel.Visibility = Visibility.Visible;
                    activePanel = InterestChatPanel;
                    NextButton.Content = "✨ Ody Planı Hazırlasın →";

                    if (ChatHistoryPanel.Children.Count == 0)
                    {
                        AddOdyChatBubble($"Selam! {destination} için harika bir macera tasarlıyoruz. 🎒 1. günde antik efsaneleri ve tarihi meydanları mı keşfetmek istersin, yoksa renkli sokaklarda taze kahve & kruvasan eşliğinde kaybolmak mı? Bana tarzını söyle, rotayı hemen şekillendireyim!");
                    }
                    break;

                case PlanStep:
                    QuestionText.Text = $"{destination} Seyahat Planın";
                    PlanDisplayPanel.Visibility = Visibility.Visible;
                    activePanel = PlanDisplayPanel;
                    NextButton.Visibility = Visibility.Collapsed;
                    GameModeBtnText.Text = $"🎲 {selectedPeopleCount} Kişilik Oyun";
                    break;
            }

            if (activePanel != null)
            {
                AnimatePanel(activePanel);
            }
        }

        private void OpenFarewell_Click(object sender, RoutedEventArgs e)
        {
            var farewell = new TripFarewellWindow(destination) { Owner = this };
            farewell.ShowDialog();
        }

        private void AnimatePanel(UIElement panel)
        {
            try
            {
                panel.Opacity = 0;
                var tt = new TranslateTransform(0, 20);
                panel.RenderTransform = tt;

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(380))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                var slideUp = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(380))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                panel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                tt.BeginAnimation(TranslateTransform.YProperty, slideUp);
            }
            catch { }
        }

        private void HideAllPanels()
        {
            DatePanel.Visibility = Visibility.Collapsed;
            PeoplePanel.Visibility = Visibility.Collapsed;
            AccommodationPanel.Visibility = Visibility.Collapsed;
            InterestChatPanel.Visibility = Visibility.Collapsed;
            PlanDisplayPanel.Visibility = Visibility.Collapsed;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartureDatePicker.SelectedDate != null && ReturnDatePicker.SelectedDate != null)
            {
                var days = (ReturnDatePicker.SelectedDate.Value.Date - DepartureDatePicker.SelectedDate.Value.Date).Days + 1;
                if (days > 0)
                {
                    DateAnimationStatusText.Text = $"✨ Toplam {days} günlük unutulmaz bir {destination} seyahati planlanıyor.";
                }
            }
        }

        // =========================================================
        // KİŞİ SAYISI SEÇİMLERİ
        // =========================================================
        private void People1_Click(object sender, MouseButtonEventArgs e) => SelectPeople(1, PeopleCard1);
        private void People2_Click(object sender, MouseButtonEventArgs e) => SelectPeople(2, PeopleCard2);
        private void People3_Click(object sender, MouseButtonEventArgs e) => SelectPeople(4, PeopleCard3);
        private void People4_Click(object sender, MouseButtonEventArgs e) => SelectPeople(6, PeopleCard4);

        private void SelectPeople(int count, Border selectedCard)
        {
            selectedPeopleCount = count;
            HighlightCard(PeoplePanel, selectedCard);
        }

        // =========================================================
        // KONAKLAMA SEÇİMLERİ & LİNKLER
        // =========================================================
        private bool isCampModeSelected = false;

        private void AccHavePlace_Click(object sender, MouseButtonEventArgs e)
        {
            selectedAccommodation = "Yerim Var";
            isCampModeSelected = false;
            SelectCampModeButton.Content = "🏕️ Kampı Seç";
            SelectCampModeButton.Background = new SolidColorBrush(Color.FromRgb(101, 22, 47));
            SelectCampModeButton.Foreground = new SolidColorBrush(Color.FromRgb(245, 233, 221));

            AccHavePlaceDot.Visibility = Visibility.Visible;
            AccNeedPlaceDot.Visibility = Visibility.Collapsed;

            AccHavePlaceCard.BorderBrush = (Brush)Application.Current.Resources["Theme_Accent"];
            AccHavePlaceCard.BorderThickness = new Thickness(2);
            AccNeedPlaceCard.BorderThickness = new Thickness(1);
            AccNeedPlaceCard.BorderBrush = (Brush)Application.Current.Resources["Theme_Border"];

            AccHavePlaceSubPanel.Visibility = Visibility.Visible;
            AccDetailsSubPanel.Visibility = Visibility.Collapsed;
        }

        private void AccNeedPlace_Click(object sender, MouseButtonEventArgs e)
        {
            selectedAccommodation = isCampModeSelected ? "Doğa ve Çadır Kampı (Survival Modu)" : "Yerim Yok";
            AccNeedPlaceDot.Visibility = Visibility.Visible;
            AccHavePlaceDot.Visibility = Visibility.Collapsed;

            AccNeedPlaceCard.BorderBrush = (Brush)Application.Current.Resources["Theme_Accent"];
            AccNeedPlaceCard.BorderThickness = new Thickness(2);
            AccHavePlaceCard.BorderThickness = new Thickness(1);
            AccHavePlaceCard.BorderBrush = (Brush)Application.Current.Resources["Theme_Border"];

            AccHavePlaceSubPanel.Visibility = Visibility.Collapsed;
            AccDetailsSubPanel.Visibility = Visibility.Visible;
        }

        private void SelectCampMode_Click(object sender, RoutedEventArgs e)
        {
            if (!isCampModeSelected)
            {
                isCampModeSelected = true;
                selectedAccommodation = "Doğa ve Çadır Kampı (Survival Modu)";
                SelectCampModeButton.Content = "✓ Kamp Seçildi!";
                SelectCampModeButton.Background = new SolidColorBrush(Color.FromRgb(40, 140, 70));
                SelectCampModeButton.Foreground = Brushes.White;
            }
            else
            {
                isCampModeSelected = false;
                selectedAccommodation = "Yerim Yok";
                SelectCampModeButton.Content = "🏕️ Kampı Seç";
                SelectCampModeButton.Background = new SolidColorBrush(Color.FromRgb(101, 22, 47));
                SelectCampModeButton.Foreground = new SolidColorBrush(Color.FromRgb(245, 233, 221));
            }
        }

        private void OpenTrivago_Click(object sender, RoutedEventArgs e) => OpenBrowserUrl($"https://www.trivago.com.tr/tr/srl?search={Uri.EscapeDataString(destination)}");
        private void OpenEts_Click(object sender, RoutedEventArgs e) => OpenBrowserUrl($"https://www.etstur.com/Arama?search={Uri.EscapeDataString(destination)}");
        private void OpenBooking_Click(object sender, RoutedEventArgs e) => OpenBrowserUrl($"https://www.booking.com/searchresults.tr.html?ss={Uri.EscapeDataString(destination)}");
        private void OpenAirbnb_Click(object sender, RoutedEventArgs e) => OpenBrowserUrl($"https://www.airbnb.com.tr/s/{Uri.EscapeDataString(destination)}/homes");

        private void OpenBrowserUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tarayıcı açılamadı: {ex.Message}", "Ody");
            }
        }

        private void HotelNameInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (HotelNamePlaceholder != null)
            {
                HotelNamePlaceholder.Visibility = string.IsNullOrWhiteSpace(HotelNameInputTextBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void HighlightCard(Panel parentPanel, Border selectedCard)
        {
            foreach (var child in parentPanel.Children)
            {
                if (child is Border b)
                {
                    b.BorderThickness = new Thickness(1);
                    b.BorderBrush = (Brush)Application.Current.Resources["Theme_Border"];
                }
            }
            selectedCard.BorderThickness = new Thickness(2);
            selectedCard.BorderBrush = (Brush)Application.Current.Resources["Theme_Accent"];
        }

        // =========================================================
        // CANLI SOHBET AKIŞI (CHAT WITH ODY)
        // =========================================================
        private void ChatInput_GotFocus(object sender, RoutedEventArgs e) { }
        private void ChatInput_LostFocus(object sender, RoutedEventArgs e) { }

        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendChatMessage();
        }

        private void SendChatMessage_Click(object sender, RoutedEventArgs e) => SendChatMessage();

        private async void SendChatMessage()
        {
            string msg = UserChatInputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(msg) || isGenerating) return;

            UserChatInputTextBox.Text = "";
            conversationHistory.Add($"Kullanıcı: {msg}");
            AddUserChatBubble(msg);
            ChatScrollViewer.ScrollToEnd();

            // Ody'nin sohbetteki samimi yanıtı
            AddOdyChatBubble("Ody rotayı inceliyor... 💭");
            Border? loadingBubble = ChatHistoryPanel.Children[^1] as Border;

            try
            {
                string chatPrompt = $@"
Sen Ody'sin; enerjik, şehri avucunun içi gibi bilen, samimi ve ilham verici bir seyahat koçusun.
Şehir: {destination}, {selectedPeopleCount} kişi gidecek.
Kullanıcı az önce şunu söyledi: '{msg}'.

KURALLAR:
- Asla resmi veya kuru mülakat gibi tek tek soru sorma.
- Kullanıcının tercihine göre {destination}'dan çok cazip ve sürükleyici 1-2 mekan/sokak örneği ver.
- Ulaşım veya yürüyüş kolaylığı hakkında kısa bir tüyo ekle (Örn: 'Trastevere'den 10 dk yürüyerek nehir kıyısına geçebilirsin!').
- 2-3 cümleyi geçmeyen canlı, tatlı ve samimi bir yanıt ver.
";
                var gemini = new GeminiService();
                string odyReply = await gemini.GenerateAsync(chatPrompt);

                if (loadingBubble != null && loadingBubble.Child is TextBlock tb)
                {
                    tb.Text = odyReply.Trim();
                    conversationHistory.Add($"Ody: {odyReply.Trim()}");
                }
            }
            catch
            {
                if (loadingBubble != null && loadingBubble.Child is TextBlock tb)
                {
                    tb.Text = "Harika bir tercih! Ulaşımı çok pratik ve manzarası efsane olan noktaları planına ekliyorum. ✨";
                }
            }

            ChatScrollViewer.ScrollToEnd();
        }

        private void AddUserChatBubble(string text)
        {
            Border bubble = new Border
            {
                Background = (Brush)Application.Current.Resources["Theme_Accent"],
                CornerRadius = new CornerRadius(14, 14, 2, 14),
                Padding = new Thickness(14, 9, 14, 9),
                Margin = new Thickness(60, 0, 0, 8),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            bubble.Child = new TextBlock
            {
                Text = text,
                FontSize = 12.5,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(43, 7, 23)),
                TextWrapping = TextWrapping.Wrap
            };
            ChatHistoryPanel.Children.Add(bubble);
        }

        private void AddOdyChatBubble(string text)
        {
            Border bubble = new Border
            {
                Background = (Brush)Application.Current.Resources["Theme_BgSub"],
                BorderBrush = (Brush)Application.Current.Resources["Theme_Border"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(14, 14, 14, 2),
                Padding = new Thickness(14, 9, 14, 9),
                Margin = new Thickness(0, 0, 60, 8),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            bubble.Child = new TextBlock
            {
                Text = text,
                FontSize = 12.5,
                Foreground = (Brush)Application.Current.Resources["Theme_TextPrimary"],
                TextWrapping = TextWrapping.Wrap
            };
            ChatHistoryPanel.Children.Add(bubble);
        }

        // =========================================================
        // SEYAHAT PLANI OLUŞTURMA & GÜN KUTULARI (DAY BOXES)
        // =========================================================
        private async Task GeneratePlanAsync()
        {
            isGenerating = true;
            SwitchBackgroundVideo("bg_ody_creating.mp4", 0.35);
            MorningText.Text = "✨ Ody gün gün rotaları, Pinterest kahvaltılarını ve ulaşım tüyolarını hazırlıyor...";
            AfternoonText.Text = "Lütfen birkaç saniye bekle...";
            EveningText.Text = "Mitolojik hikayeler, yöresel sokak lezzetleri ve oyun modu hazırlanıyor...";

            try
            {
                int totalDays = Math.Max(1, (ReturnDatePicker.SelectedDate!.Value.Date - DepartureDatePicker.SelectedDate!.Value.Date).Days + 1);

                string finalAcc = selectedAccommodation;
                if (selectedAccommodation == "Yerim Var" && !string.IsNullOrWhiteSpace(HotelNameInputTextBox.Text))
                {
                    finalAcc = $"Yerim Var (Otel: {HotelNameInputTextBox.Text.Trim()})";
                }

                string prompt = $@"
Sen Ody'sin, kişisel seyahat koçusun.
Şehir: {destination}
Tarih: {DepartureDatePicker.SelectedDate:dd.MM.yyyy} - {ReturnDatePicker.SelectedDate:dd.MM.yyyy} ({totalDays} gün)
Kişi Sayısı: {selectedPeopleCount} kişi
Konaklama: {finalAcc}
Kullanıcı ile Sohbet Geçmişi / İstekleri:
{string.Join("\n", conversationHistory)}

Lütfen aşağıdaki JSON formatında geçerli bir yanıt dön. Sadece ve sadece JSON döndür, markdown blokları veya fazladan açıklama yazma:
{{
  ""Days"": [
    {{
      ""DayNumber"": 1,
      ""DayTitle"": ""1. Gün: Tarihin Kalbine Yolculuk"",
      ""MorningBreakfast"": ""Tarihi bir kafede kahvaltı, taze kahve ve kruvasan lezzeti..."",
      ""MorningTransit"": ""🚶‍♂️ 8 dk yürüyüş mesafesinde"",
      ""AfternoonActivity"": ""Öğleden sonra keşfedilecek antik sokaklar, müze ve mitolojik hikaye..."",
      ""AfternoonTransit"": ""🚌 Otobüs / Metro ile 10 dk (2 durak)"",
      ""EveningActivity"": ""Akşam gün batımı seyir noktası ve lezzetli akşam yemeği mekan önerisi..."",
      ""EveningTransit"": ""🚶‍♂️ 12 dk sahil / sokak yürüyüşü""
    }}
  ],
  ""PhotoTips"": ""Bu rota için en iyi altın ışık saatleri (Golden Hour), gizli fotoğraf açıları ve çekim tüyoları..."",
  ""FoodDiscovery"": ""{destination}'da gezerken ayaküstü tadılması gereken 4-5 meşhur yöresel sokak lezzeti, tatlısı ve en popüler fırın/bistro önerileri..."",
  ""GameMode"": ""{selectedPeopleCount} kişilik grup için günün sonunda veya kafede oynanabilecek eğlenceli seyahat/dedektiflik oyunu...""
}}
Tam {totalDays} günlük plan hazırla.
";

                var gemini = new GeminiService();
                string rawJson = await gemini.GenerateAsync(prompt);
                ParseAndDisplayPlan(rawJson, totalDays);
            }
            catch (Exception ex)
            {
                MorningText.Text = "Plan oluşturulurken bağlantıda bir sorun oldu, ancak Ody seninle yola çıkmaya hazır!";
                AfternoonText.Text = ex.Message;
                EveningText.Text = "Lütfen tekrar deneyin.";
            }
            finally
            {
                isGenerating = false;
            }
        }

        private void ParseAndDisplayPlan(string rawText, int totalDays)
        {
            try
            {
                int start = rawText.IndexOf('{');
                int end = rawText.LastIndexOf('}');
                if (start >= 0 && end > start)
                {
                    string json = rawText.Substring(start, end - start + 1);
                    using JsonDocument doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    parsedDays.Clear();
                    if (root.TryGetProperty("Days", out var daysElem))
                    {
                        foreach (var d in daysElem.EnumerateArray())
                        {
                            parsedDays.Add(new DaySchedule
                            {
                                DayNumber = d.GetProperty("DayNumber").GetInt32(),
                                DayTitle = d.GetProperty("DayTitle").GetString() ?? "",
                                MorningBreakfast = d.GetProperty("MorningBreakfast").GetString() ?? "",
                                MorningTransit = d.TryGetProperty("MorningTransit", out var mt) ? mt.GetString() ?? "🚶‍♂️ Yürüme mesafesinde" : "🚶‍♂️ Yürüme mesafesinde",
                                AfternoonActivity = d.GetProperty("AfternoonActivity").GetString() ?? "",
                                AfternoonTransit = d.TryGetProperty("AfternoonTransit", out var at) ? at.GetString() ?? "🚌 Toplu taşıma ile 12 dk" : "🚌 Toplu taşıma ile 12 dk",
                                EveningActivity = d.GetProperty("EveningActivity").GetString() ?? "",
                                EveningTransit = d.TryGetProperty("EveningTransit", out var et) ? et.GetString() ?? "🚶‍♂️ 10 dk yürüyüş mesafesinde" : "🚶‍♂️ 10 dk yürüyüş mesafesinde"
                            });
                        }
                    }

                    if (root.TryGetProperty("PhotoTips", out var photoElem))
                        allPhotoTips = photoElem.GetString() ?? "";

                    if (root.TryGetProperty("FoodDiscovery", out var foodElem))
                        allFoodDiscovery = foodElem.GetString() ?? "";

                    if (root.TryGetProperty("GameMode", out var gameElem))
                        allGameMode = gameElem.GetString() ?? "";
                }
            }
            catch
            {
                // Fallback
                parsedDays.Clear();
                for (int i = 1; i <= totalDays; i++)
                {
                    parsedDays.Add(new DaySchedule
                    {
                        DayNumber = i,
                        DayTitle = $"{i}. Gün: {destination} Keşfi",
                        MorningBreakfast = $"{destination} merkezindeki tarihi bir kafede taze espresso ve kruvasanla güne başla.",
                        MorningTransit = "🚶‍♂️ 6 dk yürüyüş mesafesinde",
                        AfternoonActivity = $"{destination}'nın en popüler tarihi meydanlarını, müzelerini ve sokak lezzetlerini keşfet.",
                        AfternoonTransit = "🚌 64 no'lu otobüs ile 10 dk",
                        EveningActivity = "Panoramik gün batımı noktasında dinlen ve geleneksel akşam yemeğinin tadını çıkar.",
                        EveningTransit = "🚶‍♂️ 12 dk yürüyüş mesafesinde"
                    });
                }
                allPhotoTips = $"{destination} için en iyi ışık saatleri 18:00 - 19:30 arasıdır. Tarihi kemerlerin altından geniş açı pozlar çekebilirsiniz.";
                allFoodDiscovery = $"{destination} Sokak Lezzetleri: Sıcak fırın pizzası, el yapımı taze dondurma (gelato), maritozzo ve çıtır sokak atıştırmalıkları!";
                allGameMode = $"{selectedPeopleCount} kişi için oyun: 'Şehir Dedektifi' - Gün boyu en ilginç sokak tabelasını ve tarihi kapı tokmağını ilk bulan kazanır!";
            }

            SwitchBackgroundVideo("bg_ody_creating.mp4", 0.12);
            BuildDayBoxes();
        }

        private void BuildDayBoxes()
        {
            DayBoxesContainer.Children.Clear();
            if (parsedDays.Count == 0) return;

            for (int i = 0; i < parsedDays.Count; i++)
            {
                int index = i;
                Button dayBtn = new Button
                {
                    Content = $"📅 {parsedDays[i].DayNumber}. Gün",
                    Height = 36,
                    MinWidth = 100,
                    Margin = new Thickness(0, 0, 10, 0),
                    Padding = new Thickness(14, 6, 14, 6),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Cursor = Cursors.Hand
                };

                dayBtn.Resources.Add(typeof(Border), new Style(typeof(Border))
                {
                    Setters = { new Setter(Border.CornerRadiusProperty, new CornerRadius(10)) }
                });

                dayBtn.Click += (s, e) => SelectDay(index);
                DayBoxesContainer.Children.Add(dayBtn);
            }

            SelectDay(0);
        }

        private void SelectDay(int index)
        {
            if (index < 0 || index >= parsedDays.Count) return;
            activeDayIndex = index;

            // Gün butonlarını vurgula
            for (int i = 0; i < DayBoxesContainer.Children.Count; i++)
            {
                if (DayBoxesContainer.Children[i] is Button btn)
                {
                    if (i == index)
                    {
                        btn.Background = (Brush)Application.Current.Resources["Theme_Accent"];
                        btn.Foreground = new SolidColorBrush(Color.FromRgb(43, 7, 23));
                    }
                    else
                    {
                        btn.Background = (Brush)Application.Current.Resources["Theme_BgSub"];
                        btn.Foreground = (Brush)Application.Current.Resources["Theme_TextPrimary"];
                    }
                }
            }

            var day = parsedDays[index];
            MorningText.Text = day.MorningBreakfast;
            MorningTransitText.Text = day.MorningTransit;

            AfternoonText.Text = day.AfternoonActivity;
            AfternoonTransitText.Text = day.AfternoonTransit;

            EveningText.Text = day.EveningActivity;
            EveningTransitText.Text = day.EveningTransit;

            // Her gün için farklı estetik görselleri yükle
            int breakfastNum = (index % 5) + 1;
            int afternoonNum = (index % 5) + 1;
            int dinnerNum = (index % 8) + 1;

            LoadImageSafe(MorningBreakfastImage, $"breakfast_{breakfastNum}.jpg");
            LoadImageSafe(AfternoonImage, $"afternoon_{afternoonNum}.jpg");

            // Kullanıcının gönderdiği 8 adet Pinterest Akşam Yemeği Fotoğrafı (Her gün için sırayla)
            LoadImageSafe(EveningImage, $"dinner_asia_{dinnerNum}.jpg");

            AnimatePanel(DayDetailCardsPanel);
        }

        private bool IsAsianDestination()
        {
            try
            {
                string d = (destination ?? "").ToLower(new System.Globalization.CultureInfo("tr-TR"));
                string[] asianKeywords = new[]
                {
                    "tokyo", "japonya", "kyoto", "osaka", "seul", "kore", "güney kore", "guney kore",
                    "pekin", "şanghay", "sanghay", "çin", "cin", "hong kong", "taipei", "tayvan",
                    "bangkok", "phuket", "tayland", "bali", "endonezya", "singapur", "singapore",
                    "malezya", "kuala lumpur", "vietnam", "hanoi", "ho chi minh", "filipinler", "manila",
                    "hindistan", "mumbai", "delhi", "nepal", "maldivler", "asya"
                };

                foreach (var k in asianKeywords)
                {
                    if (d.Contains(k)) return true;
                }
            }
            catch { }
            return false;
        }

        private void LoadImageSafe(Image imgElement, string fileName)
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
                if (!System.IO.File.Exists(path))
                {
                    string altPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                    if (System.IO.File.Exists(altPath)) path = altPath;
                }

                if (System.IO.File.Exists(path))
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

        // =========================================================
        // FOTOĞRAF TÜYOLARI, LEZZET KEŞFİ & OYUN MODU MODALLARI
        // =========================================================
        private void OpenPhotoTips_Click(object sender, RoutedEventArgs e)
        {
            PhotoTipsModalText.Text = string.IsNullOrWhiteSpace(allPhotoTips)
                ? $"📸 {destination} için altın saatler 18:30 - 19:30 arasıdır. Arka plan ışığını kullanarak büyüleyici kadrajlar yakalayabilirsin."
                : allPhotoTips;
            PhotoTipsModal.Visibility = Visibility.Visible;
        }

        private void ClosePhotoTips_Click(object sender, RoutedEventArgs e)
        {
            PhotoTipsModal.Visibility = Visibility.Collapsed;
        }

        private void OpenFoodDiscovery_Click(object sender, RoutedEventArgs e)
        {
            FoodModalHeader.Text = $"🍴 {destination} Yöresel Lezzet & Sokak Tatları";
            FoodModalText.Text = string.IsNullOrWhiteSpace(allFoodDiscovery)
                ? $"🍴 {destination}'da gezerken ayaküstü mutlaka tatman gereken sokak lezzetleri, yerel fırın tatlıları ve meşhur atıştırmalıklar!"
                : allFoodDiscovery;
            FoodDiscoveryModal.Visibility = Visibility.Visible;
        }

        private void CloseFoodDiscovery_Click(object sender, RoutedEventArgs e)
        {
            FoodDiscoveryModal.Visibility = Visibility.Collapsed;
        }

        private void OpenGameMode_Click(object sender, RoutedEventArgs e)
        {
            GameModeModalHeader.Text = $"🎲 {selectedPeopleCount} Kişilik Oyun Modu & Görevler";
            GameModeModalText.Text = string.IsNullOrWhiteSpace(allGameMode)
                ? $"🎲 {selectedPeopleCount} kişi için eğlenceli seyahat oyunu: 'Gizli Sokak Avcısı' - Şehirde en ilginç yerel kahve kupasını veya tarihi tabelayı ilk fotoğraflayan ekip üyesi kahveleri ısmarlar!"
                : allGameMode;
            GameModeModal.Visibility = Visibility.Visible;
        }

        private void CloseGameMode_Click(object sender, RoutedEventArgs e)
        {
            GameModeModal.Visibility = Visibility.Collapsed;
        }

        // =========================================================
        // ODY İLE PLANI REVİZE ETME
        // =========================================================
        private void ReviseInput_GotFocus(object sender, RoutedEventArgs e) { }
        private void ReviseInput_LostFocus(object sender, RoutedEventArgs e) { }

        private void ReviseInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                RevisePlanWithOdy();
        }

        private void RevisePlanWithOdy_Click(object sender, RoutedEventArgs e) => RevisePlanWithOdy();

        private async void RevisePlanWithOdy()
        {
            string userChange = PlanReviseTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userChange) || isGenerating) return;

            isGenerating = true;
            MorningText.Text = $"🔄 Ody isteğini plana ekliyor: \"{userChange}\"...";
            PlanReviseTextBox.Text = "";

            try
            {
                int totalDays = parsedDays.Count > 0 ? parsedDays.Count : 3;
                string prompt = $@"
Kullanıcı mevcut seyahat planına şu değişikliği istedi: '{userChange}'.
Şehir: {destination}, {totalDays} günlük plan.
Lütfen güncellenmiş planı yine aynı JSON formatında dön:
{{
  ""Days"": [
    {{
      ""DayNumber"": 1,
      ""DayTitle"": ""1. Gün: ..."",
      ""MorningBreakfast"": ""..."",
      ""MorningTransit"": ""🚶‍♂️ ... dk"",
      ""AfternoonActivity"": ""..."",
      ""AfternoonTransit"": ""🚌 ..."",
      ""EveningActivity"": ""..."",
      ""EveningTransit"": ""🚶‍♂️ ... dk""
    }}
  ],
  ""PhotoTips"": ""..."",
  ""FoodDiscovery"": ""..."",
  ""GameMode"": ""...""
}}
Sadece JSON döndür.
";
                var gemini = new GeminiService();
                string rawJson = await gemini.GenerateAsync(prompt);
                ParseAndDisplayPlan(rawJson, totalDays);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Plan güncellenemedi: {ex.Message}", "Ody");
            }
            finally
            {
                isGenerating = false;
            }
        }

        private class DaySchedule
        {
            public int DayNumber { get; set; }
            public string DayTitle { get; set; } = "";
            public string MorningBreakfast { get; set; } = "";
            public string MorningTransit { get; set; } = "🚶‍♂️ Yürüme mesafesinde";
            public string AfternoonActivity { get; set; } = "";
            public string AfternoonTransit { get; set; } = "🚌 Toplu taşıma ile 10 dk";
            public string EveningActivity { get; set; } = "";
            public string EveningTransit { get; set; } = "🚶‍♂️ 10 dk yürüyüş mesafesinde";
        }
    }
}
