# 🌍 IMODY — AI Travel Companion

> **Microsoft Summer Internship 2026** kapsamında geliştirilmiş yapay zeka destekli akıllı seyahat koçu ve canlı macera rehberi.

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![WPF 3D](https://img.shields.io/badge/WPF-3D%20Graphics-blue.svg)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Generative AI](https://img.shields.io/badge/AI-Gemini%20API-orange.svg)](https://ai.google.dev/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## 🎥 Tanıtım Videosu
Projenin 4 dakikalık detaylı tanıtım ve sunum videosuna repodan erişebilirsiniz:
* 🎬 **[IMODY Tanıtım Videosu](IMODY_Tanitim_Videosu.mp4)**

---

## ✨ Proje Hakkında
**IMODY**, seyahat planlamasındaki sekme karmaşasını ve bilgi dağınıklığını ortadan kaldıran; rota seçiminden ulaşım sürelerine, mekan bazlı fotoğraf tüyolarından sokak lezzetlerine kadar her detayı tek ekranda sunan masaüstü seyahat asistanıdır.

Adını Homeros'un mitolojik **Odyssey (Odysseia)** destanından ve kurucusunun isminden (*İklim'in Ody'si / AI-Ody*) alan IMODY; kullanıcılarına seyahat öncesinde, sırasında ve sonrasında rehberlik eder.

---

## 🚀 Öne Çıkan Özellikler

1. **🌍 İnteraktif 3D Dünya Küresi (WPF 3D):**
   * Küresel koordinat matematiği (\(\theta, \phi \to x,y,z\)) ile modellenmiş, 23.4° eksen eğikliğine sahip interaktif küre.
   * Fare ile 360° serbest dönüş ve zoom desteği.
   * Kıtaların parlamasını sağlayan `EmissiveMaterial` ve okyanus yansıması için `SpecularMaterial` fotometrik ışık katmanları.

2. **✨ Kaligrafik Çizim Animasyonu:**
   * *"Sıradaki Rotamız Neresi?"* başlığının vektörel eğrilerle (`PathGeometry`, `StrokeDashOffset`) harf harf çizilme animasyonu.

3. **🧭 Akıllı Rota Seçimi:**
   * **Doğrudan Arama:** Şehir adı yazarak plan sihirbazını başlatma.
   * **🎰 Spin the Globe (Dünyayı Döndür):** Dünyanın otomatik dönerek rastgele bir ülke/şehir belirlemesi.
   * **🗺️ Explore the Globe (Keşfet):** 3D dünya üzerinde dönen coğrafi ülke pinlerine tıklayarak keşif.

4. **🤖 Yapay Zeka Seyahat Koçu (Ody & Generative AI):**
   * C# `async/await` asenkron mimarisi ile donmayan arayüz.
   * **📋 Gün Gün Rota & Ulaşım Özeti:** Her gün gidilecek yerler ve hemen altında adım adım ulaşım süreleri (yürüyüş, metro, otobüs).
   * **📸 Mekan Bazlı Fotoğraf & Poz Rehberi:** Gidilecek her mekan için sabah yumuşak ışığı ve akşam altın saatler (Golden Hour) için nerede durulup nasıl poz verileceğini anlatan rehber.
   * **🍴 Gizli Sokak Lezzetleri:** Yerel otantik tatlar ve fırın/bistro önerileri.
   * **🎲 Kişi Sayısına Özel Grup Oyun Modu:** Seyahati eğlenceli hale getiren mini oyunlar ve dedektiflik görevleri.

5. **📖 Seyahat Günlüğü & 🏅 Gezgin Rütbeleri:**
   * Seyahat notları kaydetme ve rota başına **5'er adet anı fotoğrafı** yükleme.
   * Çaylak Seyyah'tan Galaksi Kaptanına kadar uzanan oyunlaştırılmış seviye ve XP sistemi.

6. **📦 Kurulum & Dağıtım:**
   * **Inno Setup** ile derlenmiş bağımsız `IMODY_Kurulum_Setup.exe` kurulum sihirbazı.
   * .NET runtime gerektirmeyen self-contained taşınabilir paket.

---

## 🛠️ Kullanılan Teknolojiler & Kütüphaneler
* **Programlama Dili:** C# 12
* **Framework & UI:** .NET 8.0, WPF (Windows Presentation Foundation), XAML
* **3D Grafik Motoru:** Viewport3D, MeshGeometry3D, Directional/Ambient Lighting
* **Yapay Zeka:** Google Generative AI (Gemini API), Asynchronous Task Architecture
* **Medya & Görüntü İşleme:** MediaElement, OpenCV (Python video döngü optimizasyonu), Pillow
* **Kurulum:** Inno Setup 6

---

## 👩‍💻 Geliştirici & Teşekkür
* **Geliştirici:** İklim Düzen (Zonguldak Bülent Ecevit Üniversitesi Bilgisayar Bölümü)
* **Özel Teşekkür:** Bu projede vizyonu, mentorluğu ve desteğiyle yol gösteren değerli hocamız **Barbaros Günay**'a ve bu imkanı sunan tüm **Microsoft** ailesine sonsuz teşekkürler! 🌟🎓
