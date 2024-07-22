using Domain.Enumeradores;

namespace Application.Commands.Dtos
{
    public record UserPermissionCommandDto(int UsuarioId, EnumPermissoes[] Permissoes);
}
