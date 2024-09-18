using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestShared.DTO
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Benutzername ist erforderlich")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Passwort ist erforderlich")]
        public string Password { get; set; }
    }
}
