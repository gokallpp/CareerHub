using System.ComponentModel.DataAnnotations;

namespace CareerHub.ViewModels
{
    public class CandidateProfileViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Şehir")]
        public string? City { get; set; }

        [StringLength(
            1000,
            ErrorMessage = "Hakkımda alanı en fazla 1000 karakter olabilir.")]
        [Display(Name = "Hakkımda")]
        public string? AboutMe { get; set; }

        public string? CvFileName { get; set; }
    }
}
