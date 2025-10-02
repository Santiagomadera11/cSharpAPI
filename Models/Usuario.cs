using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace USUARIOS.Models
{
    public partial class Usuario
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        public string TipoDoc { get; set; } = null!;

        [Required(ErrorMessage = "El número de documento es obligatorio")]
        public string NroDoc { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El password hash es obligatorio")]
        public byte[] PasswordHash { get; set; } = null!;

        [Required(ErrorMessage = "El password salt es obligatorio")]
        public byte[] PasswordSalt { get; set; } = null!;

        // ✅ Inicializado para evitar NullReference
        public virtual ICollection<UsuarioRole> UsuarioRoles { get; set; } = new List<UsuarioRole>();
    }
}
