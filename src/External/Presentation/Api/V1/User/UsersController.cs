using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Asp.Versioning;
using Domain.Dtos.User;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V1.User
{
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [ApiVersionRoute("user")]
    public class UsersController(IAuthCommandService _authService, IServiceProvider service)
        : MainController(service)
    {
        [HttpPost("auth/login")]
        public async Task<UserTokenDto> Login(UserCommandDto userDto)
        {
            if (userDto.Email.IsNullOrEmpty() || userDto.Password.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.ClientError, "Email ou Senha incorretos.");
                return null;
            }

            return await _authService.AutenticarUsuario(userDto);
        }

        [HttpGet("info")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public UserInfoDto UserInfo() =>
            new(
                HttpContext.User.Identity.Name,
                _authService.VerificarPermissao(
                    EnumPermissoes.CATEGORIA_000001,
                    EnumPermissoes.CATEGORIA_000002,
                    EnumPermissoes.CATEGORIA_000003,
                    EnumPermissoes.MEMBRO_000001,
                    EnumPermissoes.MEMBRO_000002,
                    EnumPermissoes.MEMBRO_000003,
                    EnumPermissoes.DESPESA_000001,
                    EnumPermissoes.DESPESA_000002,
                    EnumPermissoes.DESPESA_000003,
                    EnumPermissoes.GRUPOFATURA_000001,
                    EnumPermissoes.GRUPOFATURA_000002,
                    EnumPermissoes.GRUPOFATURA_000003,
                    EnumPermissoes.STATUSFATURA_000001,
                    EnumPermissoes.STATUSFATURA_000002,
                    EnumPermissoes.STATUSFATURA_000003,
                    EnumPermissoes.LISTACOMPRA_000001,
                    EnumPermissoes.LISTACOMPRA_000002,
                    EnumPermissoes.LISTACOMPRA_000003
                )
            );
    }
}
