using Microsoft.AspNetCore.Identity;

namespace CareerHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? City { get; set; }

        public string? AboutMe { get; set; }

        // Kullanıcının yüklediği CV'nin orijinal dosya adı.
        public string? CvFileName { get; set; }

        // CV'nin sunucuda saklandığı benzersiz dosya adı.
        public string? CvStoredFileName { get; set; }
    }
}
