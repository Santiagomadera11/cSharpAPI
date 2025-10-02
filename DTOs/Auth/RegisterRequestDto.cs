using System.Collections.Generic;

namespace USUARIOS.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string TipoDoc { get; set; } = string.Empty;
        public string NroDoc { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<int> Roles { get; set; } = new();
    }
}