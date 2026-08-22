using System.ComponentModel.DataAnnotations;

namespace CareerHub.ViewModels
{
    public class CompanyCreateViewModel
    {
        [Required(ErrorMessage = "Şirket adı zorunludur.")]
        [Display(Name = "Şirket Adı")]
        public string Name { get; set; }

        [Display(Name = "Şirket Açıklaması")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Geçerli bir web sitesi adresi giriniz.")]
        [Display(Name = "Web Sitesi")]
        public string? Website { get; set; }
    }
}
