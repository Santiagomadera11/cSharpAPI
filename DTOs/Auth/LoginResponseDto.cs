namespace USUARIOS.DTOs.Auth
{
    /// <summary>
    /// DTO para respuesta de login
    /// </summary>
    public class LoginResponseDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty; // ¡Esta es la línea que faltaba!
    }
}