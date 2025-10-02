using AutoMapper;
using USUARIOS.Models;
using USUARIOS.DTOs;

namespace USUARIOS.Mappings // 👈 ESTE namespace DEBE COINCIDIR
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Usuario -> UsuarioDto
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UsuarioRoles.Select(ur => new RolDto
                    {
                        RolId = ur.Rol.RolId,
                        Nombre = ur.Rol.Nombre
                    }).ToList()
                ));

            // UsuarioCreateDto -> Usuario
            CreateMap<UsuarioCreateDto, Usuario>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioRoles, opt => opt.Ignore());
        }
    }
}
