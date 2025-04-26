using System.ComponentModel.DataAnnotations;

namespace BiblioApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un usuario válido")]

        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]

        public string Clave { get; set; }
    }
}
