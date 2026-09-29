using System.ComponentModel.DataAnnotations;

namespace CargoTrack.WebUI.Models
{
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pozisyon zorunludur.")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Şube seçilmelidir.")]
        public Guid BranchId { get; set; }
    }
}