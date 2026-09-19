# CareerHub

CareerHub, yazılım sektörüne yönelik iş ilanlarının yayınlanabildiği ve adayların ilanlara başvuru yapabildiği bir **ASP.NET Core MVC iş ilanı platformudur**.

Proje; ASP.NET Core MVC, Entity Framework Core, PostgreSQL ve ASP.NET Core Identity kullanılarak geliştirilmiştir.

Aday, işveren ve yönetici olmak üzere üç farklı kullanıcı rolüne sahiptir.

---

## Özellikler

### Aday

- Kayıt olma ve giriş yapma
- Profil bilgilerini düzenleme
- Profil fotoğrafı yükleme
- CV yükleme ve değiştirme
- İş ilanlarını görüntüleme
- Pozisyon ve anahtar kelime ile arama
- Şehir, iş tipi ve çalışma şekline göre filtreleme
- İş ilanlarına başvuru yapma
- Yapılan başvuruları görüntüleme
- Başvuru durumlarını takip etme

### İşveren

- İşveren hesabı oluşturma
- Şirket profili oluşturma ve düzenleme
- İş ilanı oluşturma
- İlan düzenleme
- İlanı aktif / pasif hale getirme
- Her ilana yapılan başvuru sayısını görüntüleme
- İlan bazında başvuruları listeleme
- Başvuran adayların profil bilgilerini görüntüleme
- Aday CV'sini görüntüleme
- Başvuru durumunu güncelleme
- Tüm başvuruları tek ekranda görüntüleme

### Admin

- Admin dashboard
- Kullanıcıları görüntüleme
- Kullanıcı detaylarını görüntüleme
- Kullanıcı hesabını kilitleme / açma
- Şirketleri görüntüleme
- Şirket detaylarını görüntüleme
- İş ilanlarını yönetme
- İlanları aktif / pasif hale getirme
- Yapılan başvuruları görüntüleme

---

## Kullanılan Teknolojiler

- ASP.NET Core MVC
- C#
- Entity Framework Core
- PostgreSQL
- Npgsql
- ASP.NET Core Identity
- Razor Views
- LINQ
- Bootstrap
- HTML
- CSS
- JavaScript

---

## Kullanıcı Rolleri

CareerHub üç farklı kullanıcı rolüne sahiptir:

| Rol | Yetkiler |
|---|---|
| `Candidate` | İş ilanlarını görüntüleme ve başvuru yapma |
| `Employer` | Şirket ve iş ilanlarını yönetme |
| `Admin` | Platform yönetimi |

Public kayıt sırasında yalnızca `Candidate` ve `Employer` rolleri oluşturulabilir.

`Admin` rolü kullanıcı tarafından kayıt ekranından seçilemez.

---

## İş İlanları

İş ilanları ekranında kullanıcılar;

- Pozisyon veya anahtar kelime
- Şehir
- İş tipi
- Çalışma şekli

alanlarına göre filtreleme yapabilir.

İlanlar sayfalanarak gösterilir.

Masaüstü görünümünde ilanlar **split-view** yapısında gösterilir. Sol taraftan ilan seçildiğinde ilan detayları sağ tarafta görüntülenir.

Mobil cihazlarda ise ilan seçildiğinde ayrı ilan detay sayfası açılır.

---

## Başvuru Sistemi

Bir aday aynı ilana yalnızca bir kez başvurabilir.

Başvuru yapabilmek için adayın sisteme CV yüklemiş olması gerekir.

Başvurular aşağıdaki durumlara sahip olabilir:

- `Beklemede`
- `İncelendi`
- `Mülakat`
- `Kabul Edildi`
- `Reddedildi`



İşveren yalnızca kendi şirketine ait ilanların başvurularını görüntüleyebilir ve güncelleyebilir.

---

## Güvenlik

Projede ASP.NET Core Identity kullanılmaktadır.

Uygulanan bazı güvenlik kontrolleri:

- Role-based authorization
- Candidate / Employer / Admin rol ayrımı
- Form manipülasyonu ile Admin rolü oluşturmanın engellenmesi
- İşverenlerin yalnızca kendi şirket ve ilanlarını yönetebilmesi
- İşverenlerin yalnızca kendi ilanlarına başvuran adayları görüntüleyebilmesi
- Anti-forgery token kullanımı
- Güvenli `returnUrl` kontrolü
- CV dosyalarında dosya türü ve boyut kontrolü
- Profil fotoğraflarında dosya uzantısı ve dosya imzası kontrolü
- Yüklenen dosyalarda GUID tabanlı dosya isimlendirme
- Özel 403, 404 ve 500 hata sayfaları

---

## Veritabanı

Projede PostgreSQL kullanılmaktadır.

Entity Framework Core üzerinden veritabanı işlemleri gerçekleştirilmekte ve schema değişiklikleri Migration sistemiyle yönetilmektedir.

Temel tablolar:

- `AspNetUsers`
- `AspNetRoles`
- `companies`
- `job_postings`
- `job_applications`

Temel ilişkiler:

```text
ApplicationUser
      │
      │ Owner
      ▼
   Company
      │
      │ 1 - N
      ▼
 JobPosting
      │
      │ 1 - N
      ▼
JobApplication
      ▲
      │
      │ Candidate
ApplicationUser
