using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using USUARIOS.DTOs;
using USUARIOS.DTOs.Auth;
using USUARIOS.Models;

namespace USUARIOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosContext _context;
        private readonly IMapper _mapper;

        public UsuariosController(UsuariosContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ========== ENDPOINTS DE AUTENTICACIÓN ==========

        /// <summary>
        /// Login de usuario
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { msj = "Email y contraseña son requeridos" });

            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return Unauthorized(new { msj = "Usuario no encontrado" });

            using var hmac = new HMACSHA512(usuario.PasswordSalt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            if (!hash.SequenceEqual(usuario.PasswordHash))
                return Unauthorized(new { msj = "Contraseña incorrecta" });

            var response = new LoginResponseDto
            {
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Roles = usuario.UsuarioRoles.Select(r => r.Rol.Nombre).ToList()
            };

            return Ok(response);
        }

        // ========== ENDPOINTS CRUD ==========

        /// <summary>
        /// Crear un nuevo usuario
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> CrearUsuario([FromBody] UsuarioCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { msj = "El objeto UsuarioCreateDto es requerido" });

            if (await _context.Usuarios.AnyAsync(u => u.NroDoc == dto.NroDoc))
                return Conflict(new { msj = $"Ya existe un usuario con el documento {dto.NroDoc}" });

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { msj = "La contraseña es requerida" });

            var rolesValidos = await _context.Roles
                .Where(r => dto.Roles.Contains(r.RolId))
                .ToListAsync();

            if (rolesValidos.Count != dto.Roles.Count)
                return BadRequest(new { msj = "Uno o más roles enviados no existen." });

            using var hmac = new HMACSHA512();
            var usuario = _mapper.Map<Usuario>(dto);
            usuario.PasswordSalt = hmac.Key;
            usuario.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            foreach (var rolId in dto.Roles.Distinct())
            {
                _context.UsuarioRoles.Add(new UsuarioRole
                {
                    UsuarioId = usuario.UsuarioId,
                    RolId = rolId
                });
            }

            await _context.SaveChangesAsync();

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            return CreatedAtAction(nameof(BuscarUsuario), new { id = usuario.UsuarioId }, usuarioDto);
        }

        /// <summary>
        /// Listar todos los usuarios
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> ListarUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .ToListAsync();

            return Ok(_mapper.Map<List<UsuarioDto>>(usuarios));
        }

        /// <summary>
        /// Buscar un usuario por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> BuscarUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
                return NotFound(new { msj = "Usuario no encontrado" });

            return Ok(_mapper.Map<UsuarioDto>(usuario));
        }

        /// <summary>
        /// Actualizar un usuario existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioDto>> ActualizarUsuario(int id, [FromBody] UsuarioCreateDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
                return NotFound(new { msj = "Usuario no encontrado" });

            usuario.TipoDoc = dto.TipoDoc;
            usuario.NroDoc = dto.NroDoc;
            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                using var hmac = new HMACSHA512();
                usuario.PasswordSalt = hmac.Key;
                usuario.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            }

            usuario.UsuarioRoles.Clear();
            foreach (var rolId in dto.Roles.Distinct())
            {
                usuario.UsuarioRoles.Add(new UsuarioRole
                {
                    UsuarioId = usuario.UsuarioId,
                    RolId = rolId
                });
            }

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<UsuarioDto>(usuario));
        }

        /// <summary>
        /// Eliminar un usuario
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { msj = "Usuario no encontrado" });

            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return Ok(new { msj = "Usuario eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}