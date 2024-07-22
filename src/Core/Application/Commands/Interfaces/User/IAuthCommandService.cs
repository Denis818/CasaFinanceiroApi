using Application.Commands.Dtos;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;

namespace Application.Commands.Interfaces
{
    public interface IAuthCommandService
    {
        Task<UserTokenDto> AutenticarUsuario(UserCommandDto userDto);
        bool VerificarPermissao(params EnumPermissoes[] permissoesParaValidar);
        Task AddPermissaoAsync(UserPermissionCommandDto userPermissao);
    }
}
