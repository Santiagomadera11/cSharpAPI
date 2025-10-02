using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using USUARIOS.DTOs.Auth;
using USUARIOS.Models;
using USUARIOS.Services;
using USUARIOS.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace USUARIOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsuariosContext _context;
        private readonly JwtService _jwtService;

        public AuthController(UsuariosContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            if (!PasswordHelper.VerifyPassword(request.Password, usuario.PasswordHash, usuario.PasswordSalt))
                return Unauthorized("Contraseña incorrecta");

            var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();

            // === CORRECCIÓN AQUÍ ===
            // Cambiamos usuario.Id por usuario.UsuarioId
            var token = _jwtService.GenerateToken(usuario.Email, usuario.Nombre, usuario.UsuarioId);
            // =========================

            return new LoginResponseDto
            {
                Token = token,
                Nombre = usuario.Nombre,
                Roles = roles,
                UsuarioId = usuario.UsuarioId, // Y también aquí
                Email = usuario.Email
            };
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> Registrar(RegisterRequestDto request)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { msj = "El email ya está registrado" });

            PasswordHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var usuario = new Usuario
            {
                TipoDoc = request.TipoDoc,
                NroDoc = request.NroDoc,
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            foreach (var rolId in request.Roles)
            {
                usuario.UsuarioRoles.Add(new UsuarioRole
                {
                    RolId = rolId,
                    Usuario = usuario
                });
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { msj = "Usuario registrado correctamente" });
        }
    }
}