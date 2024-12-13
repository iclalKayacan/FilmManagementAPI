Film Management API

Kullanıcıların filmleri yönetmesine, favorilere/izleme listesine eklemesine, yorum yapmasına ve puanlama yapmasına olanak sağlayan bir RESTful API'dir.

Özellikler
Kullanıcı Yönetimi: Kayıt, giriş ve JWT ile kimlik doğrulama.
Film Yönetimi: Filmleri ekleme, düzenleme, silme (sadece Admin yetkisiyle).
Favoriler ve İzleme Listesi: Filmleri favorilere/izleme listesine ekleme ve çıkarma.
Yorum ve Puanlama: Filmlere yorum yapma ve puanlama, ortalama puan hesaplama.
Arama ve Filtreleme: Tür, yönetmen, yıl veya başlığa göre arama/filtreleme.
JWT Yetkilendirme: Admin ve standart kullanıcı rollerine özel erişim yetkileri.
Hata Yönetimi: Kullanıcı dostu ve açıklayıcı hata mesajları.
Swagger Entegrasyonu: API dokümantasyonu ve token ile yetkilendirme testleri.

Teknolojiler

ASP.NET Core 6.0
Entity Framework Core
JWT (JSON Web Token)
Swagger
SQL Server
