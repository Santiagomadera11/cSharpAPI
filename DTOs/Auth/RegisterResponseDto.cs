namespace USUARIOS.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}