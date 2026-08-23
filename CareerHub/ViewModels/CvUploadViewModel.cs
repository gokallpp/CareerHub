using System.ComponentModel.DataAnnotations;

namespace CareerHub.ViewModels
{
    public class CvUploadViewModel
    {
        [Required(ErrorMessage = "CV dosyası seçiniz.")]
        [Display(Name = "CV")]
        public IFormFile CvFile { get; set; }
    }
}
