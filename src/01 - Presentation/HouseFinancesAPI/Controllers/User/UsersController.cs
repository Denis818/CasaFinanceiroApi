﻿using Application.Interfaces.Services.User;
using Domain.Dtos.User;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;
using HouseFinancesAPI.Attributes;
using HouseFinancesAPI.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HouseFinancesAPI.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IAuthAppService _authService, IServiceProvider service)
        : BaseApiController(service)
    {
        [HttpPost("login")]
        public async Task<UserTokenDto> Login(UserDto userDto)
        {
            if(userDto.Email.IsNullOrEmpty() || userDto.Password.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.ClientError, "Email ou Senha incorretos.");
                return null;
            }

            if(userDto.Email.Replace(" ", "").Equals("master", StringComparison.CurrentCultureIgnoreCase))
                userDto.Email = _configuration["UserMaster:Email"];

            return await _authService.AutenticarUsuario(userDto);
        }

        [HttpGet("info")]
        [AutorizationFinance]
        [ApiExplorerSettings(IgnoreApi = true)]
        public UserInfoDto UserInfo() =>
            new(
                HttpContext.User.Identity.Name,
                _authService.VerificarPermissao(EnumPermissoes.USU_000001)
            );
    }
}