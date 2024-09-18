using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestShared.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Benutzername ist erforderlich")]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-Mail ist erforderlich")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Passwort ist erforderlich")]
        [MinLength(6, ErrorMessage = "Das Passwort muss mindestens 6 Zeichen lang sein")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Passwortbestätigung ist erforderlich")]
        [Compare(nameof(Password), ErrorMessage = "Passwörter stimmen nicht überein")]
        public string ConfirmPassword { get; set; }
    }
}
