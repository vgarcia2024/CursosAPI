using AutoMapper;
using Tp_Programacion.Models.Curso;
using Tp_Programacion.Models.Curso.Dto;
using Tp_Programacion.Models.Role;
using Tp_Programacion.Models.Role.Dto;
using Tp_Programacion.Models.User;
using Tp_Programacion.Models.User.Dto;

namespace Tp_Programacion.Config
{
    public class Mapping :Profile
    {
        public Mapping() { 
            // Curso
             CreateMap<Curso, CursoDTO>().ReverseMap();
             CreateMap<CursosDTO, Curso>().ReverseMap(); // si CursosDTO es para listas
             CreateMap<CreateCursoDTO, Curso>().ReverseMap();
             CreateMap<UpdateCursoDTO, Curso>()
            .ForAllMembers(cfg => cfg.Condition((_, _, value) => value != null));

            // User
            CreateMap<User, UserDTO>().ForMember(
                dest => dest.Roles,
                opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToList())
            );
            CreateMap<RegisterDTO, User>().ReverseMap();
            CreateMap<UpdateUserDTO, User>()
                .ForAllMembers(cfg => cfg.Condition((_, _, value) => value != null));

            // Role
            CreateMap<Role, RoleDTO>().ReverseMap();
        }

    }
}
