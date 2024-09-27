using Application.Commands.Dtos;
using Application.Queries.Dtos;
using Domain.Models.Users;

namespace Application.Configurations.MappingsApp
{
    public static class UserMap
    {
        public static UserQueryDto MapToDTO(this Usuario usuario)
        {
            return new UserQueryDto
            {
                Email = usuario.Email,
                Password = usuario.Password,
                Code = usuario.Code
            };
        }

        public static Usuario MapToEntity(this UserCommandDto usuarioDto)
        {
            return new Usuario
            {
                Email = usuarioDto.Email,
                Password = usuarioDto.Password,
            };
        }

        public static void MapUpdateEntity(this Usuario usuario, UserCommandDto usuarioDto)
        {
            usuario.Email = usuarioDto.Email;
            usuario.Password = usuarioDto.Password;
        }
    }
}
