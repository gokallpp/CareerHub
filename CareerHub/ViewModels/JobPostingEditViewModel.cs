using System.ComponentModel.DataAnnotations;

namespace CareerHub.ViewModels
{
    public class JobPostingEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İlan başlığı zorunludur.")]
        [Display(Name = "İlan Başlığı")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İş tipi zorunludur.")]
        [Display(Name = "İş Tipi")]
        public string JobType { get; set; }

        [Required(ErrorMessage = "Çalışma şekli zorunludur.")]
        [Display(Name = "Çalışma Şekli")]
        public string WorkType { get; set; }

        [Required(ErrorMessage = "İş açıklaması zorunludur.")]
        [Display(Name = "İş Açıklaması")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Konum zorunludur.")]
        [Display(Name = "Konum")]
        public string Location { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Geçerli bir maaş giriniz.")]
        [Display(Name = "Maaş")]
        public decimal Salary { get; set; }
    }
}
