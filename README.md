🚚📦 CargoTrack —Yapay zeka destekli kurumsal lojistik platformu

Herkese Merhaba 👋

🏗️ MİMARİ VE ALTYAPI

✅ N-Katmanlı Mimari: Entity / DTO / DataAccess / Business / WebUI katmanları tamamen ayrıştırılmış durumda.

✅ Generic Repository Pattern + Domain Driven Service'ler.

✅ Entity Framework Core ile Code-First yaklaşım: Karmaşık ilişkiler, çoklu foreign key senaryoları ve cascade kısıtlamaları titizlikle kurgulandı.

✅ Mapster ile Entity ↔ DTO dönüşümleri.

✅ ASP.NET Core Identity ile 3 farklı rol (Admin / Şube Müdürü / Müşteri) ve her role özel veri erişim/izolasyon kapsamı.

✅ Global Exception Handling & Soft Delete mimarisi.

🧠 YAPAY ZEKA VE MAKİNE ÖĞRENMESİ (ML.NET)

Projeyi standart bir otomasyondan çıkarıp "akıllı" bir lojistik platformuna dönüştüren en heyecan verici geliştirmeler:

🤖 Gecikme Riski Tahmini (Binary Classification): Sistem, geçmiş teslimat verilerinden (ağırlık, desi, gönderi tipi, gün, mesafe) öğrenerek, yeni oluşturulan bir kargonun zamanında ulaşıp ulaşamayacağını (Yüksek Risk / Düşük Risk) tahmin ediyor.

📈 Dinamik Fiyatlandırma (Regression): Kargo ücretleri sadece statik kurallarla değil; Efsane Cuma gibi yoğun dönem katsayıları, mesafe ve kargo tipini analiz eden ML.NET regresyon modeliyle (FastTree/Sdca) dinamik olarak hesaplanıyor.

📍 Akıllı Rota Optimizasyonu (TSP): Kuryelerin gün içinde dağıtacağı kargoların adres koordinatları alınarak (Haversine formülü ile) en kısa mesafe ve en düşük süreye göre optimum teslimat sırası oluşturuluyor.

⚙️ GERÇEK İŞ KURALLARI

🔸 State Machine (Durum Makinesi): Kargo durumları rastgele değiştirilemez; sadece tanımlı yaşam döngüsü geçişleri (Kabul → Transfer → Dağıtım → Teslim) çalışır.

🔸 OTP Doğrulamalı Teslimat: Dağıtıma çıkan kargo için üretilen 6 haneli benzersiz kod girilmeden kargo teslim edilemez.

🔸 Otomatik İade Akışı: 3 başarısız teslimat denemesinde sistem otomatik olarak iade sürecini tetikler.

🔸 Merkezi Audit Log: Sistemdeki her durum değişikliği, işlemi kimin yaptığı ve eski/yeni değerleriyle birlikte mühürlenir.

🗺️ CANLI HARİTA ENTEGRASYONU

Kargo takip sayfası ve kurye rota planlayıcısında soyut çizgiler yok. Leaflet.js + OpenStreetMap entegrasyonu ile kargonun gerçek şehir koordinatları arasında ilerleyen canlı bir işaretçi ve kuryenin optimize edilmiş teslimat rotasının harita üzerinde birebir çizimi var.

🎨 TASARIM VE KULLANICI DENEYİMİ

İki farklı, amaca yönelik tasarım dili kurguladım:

🌑 Müşteri Ekranları: Lacivert-amber renk paleti, Space Grotesk tipografi, SVG animasyonları ve "glassmorphism" (camsı) tasarım detaylarıyla modern bir arayüz.

🟠 Admin/Kurye Paneli: MD3 tabanlı, veri okumayı kolaylaştıran kurumsal palet ve gerçek zamanlı (Chart.js) dashboard metrikleri. Sunucu taraflı sayfalama, filtreleme ve antetli/barkodlu resmi makbuz çıktıları.

🛠️ TEKNOLOJİLER
Backend: C#, ASP.NET Core 8 MVC, Entity Framework Core, ASP.NET Core Identity, ML.NET, Mapster, SQL Server, FluentValidation
Frontend: Razor Views, Tailwind CSS, Chart.js, Leaflet.js, Vanilla JS (SVG/Animasyonlar)

<img width="1029" height="845" alt="Ekran görüntüsü 2026-09-29 210144" src="https://github.com/user-attachments/assets/ba68c14d-557f-403e-ac94-9ddc44ac533c" />
<img width="1357" height="530" alt="Ekran görüntüsü 2026-09-29 210122" src="https://github.com/user-attachments/assets/48451e5b-4794-46fe-aa02-1be29f42ded5" />
<img width="1221" height="897" alt="Ekran görüntüsü 2026-09-29 210110" src="https://github.com/user-attachments/assets/f4ea7cda-c87c-48e3-ad47-1c228c9ca784" />
<img width="985" height="817" alt="Ekran görüntüsü 2026-09-29 210052" src="https://github.com/user-attachments/assets/3b62d2bc-27f5-4194-836e-d16c92ad57e6" />
<img width="1446" height="892" alt="Ekran görüntüsü 2026-09-29 210045" src="https://github.com/user-attachments/assets/0b3dddbe-6451-42bb-b097-f1364758e85c" />
<img width="1264" height="906" alt="Ekran görüntüsü 2026-09-29 210028" src="https://github.com/user-attachments/assets/52caf566-529f-4f96-8b2f-bbe022657cd6" />
<img width="1891" height="841" alt="Ekran görüntüsü 2026-09-29 210012" src="https://github.com/user-attachments/assets/7eef44b4-16c8-4635-8ba3-664b1bca537e" />
<img width="1895" height="905" alt="Ekran görüntüsü 2026-09-29 205951" src="https://github.com/user-attachments/assets/20f57273-9d53-44a7-a9d5-ebc3ad2a1b93" />
<img width="1890" height="900" alt="Ekran görüntüsü 2026-09-29 205938" src="https://github.com/user-attachments/assets/f06cdab2-c66c-46a9-87d2-d79928687c93" />
<img width="1669" height="846" alt="Ekran görüntüsü 2026-09-29 213443" src="https://github.com/user-attachments/assets/52467df3-b5ba-4318-b826-ce0fc7dbd52f" />
<img width="1690" height="894" alt="Ekran görüntüsü 2026-09-29 220319" src="https://github.com/user-attachments/assets/c25610bb-6dbd-4b49-9368-865922e66f5b" />
<img width="1601" height="855" alt="Ekran görüntüsü 2026-09-29 214639" src="https://github.com/user-attachments/assets/cf1750f0-5437-4ed1-9d89-9efd60500e27" />
<img width="1653" height="843" alt="Ekran görüntüsü 2026-09-29 213417" src="https://github.com/user-attachments/assets/594c99fa-b816-4646-a79c-dbaa66848bec" />
<img width="1079" height="815" alt="Ekran görüntüsü 2026-09-29 210505" src="https://github.com/user-attachments/assets/9f27b4f1-2a2f-4582-9684-bcf9390a0009" />
<img width="1144" height="837" alt="Ekran görüntüsü 2026-09-29 210442" src="https://github.com/user-attachments/assets/e21ae56a-e712-432a-b738-d1340eb618fa" />
<img width="1885" height="898" alt="Ekran görüntüsü 2026-09-29 210353" src="https://github.com/user-attachments/assets/95713294-c7ce-4c26-ad4c-b9f1d4cae475" />
<img width="1899" height="533" alt="Ekran görüntüsü 2026-09-29 210342" src="https://github.com/user-attachments/assets/d6143425-0dbc-48c9-b1e8-cb7fcbb06c00" />
<img width="1903" height="658" alt="Ekran görüntüsü 2026-09-29 210327" src="https://github.com/user-attachments/assets/4ea660f8-6e11-423b-ace0-1cbf524f1a68" />
<img width="1883" height="875" alt="Ekran görüntüsü 2026-09-29 210322" src="https://github.com/user-attachments/assets/e11c23f5-6b5d-40a1-bed4-1a5bad2b9871" />
<img width="1255" height="656" alt="Ekran görüntüsü 2026-09-29 210312" src="https://github.com/user-attachments/assets/958865a2-910a-4d86-8944-a67ec4468aab" />
<img width="1900" height="825" alt="Ekran görüntüsü 2026-09-29 210259" src="https://github.com/user-attachments/assets/428e88cc-00f5-4637-97a6-9c330502a1e6" />
<img width="1908" height="826" alt="Ekran görüntüsü 2026-09-29 210248" src="https://github.com/user-attachments/assets/fa0942b8-fddd-40c9-819a-a39e977b941f" />
<img width="1908" height="859" alt="Ekran görüntüsü 2026-09-29 210242" src="https://github.com/user-attachments/assets/628e894a-97b8-48a6-a195-f7a2b63662cd" />
<img width="1901" height="827" alt="Ekran görüntüsü 2026-09-29 210234" src="https://github.com/user-attachments/assets/bdd8f1d3-256b-486c-8339-516af64562dc" />
<img width="1908" height="891" alt="Ekran görüntüsü 2026-09-29 210225" src="https://github.com/user-attachments/assets/a439d688-229b-472d-b06e-76a9854535d7" />
