namespace USUARIOS.DTOs
{
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string TipoDoc { get; set; } = string.Empty;
        public string NroDoc { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<RolDto> Roles { get; set; } = new();
    }

    public class UsuarioCreateDto
    {
        public string TipoDoc { get; set; } = string.Empty;
        public string NroDoc { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<int> Roles { get; set; } = new();
    }

    public class RolDto
    {
        public int RolId { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
