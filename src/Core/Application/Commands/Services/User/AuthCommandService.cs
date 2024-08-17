using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Helpers;
using Application.Resources.Messages;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Commands.Services
{
    public class AuthCommandService(IServiceProvider service, IConfiguration _configuration)
        : BaseCommandService<Usuario, UserCommandDto, IUsuarioRepository>(service),
            IAuthCommandService
    {
        protected override Usuario MapToEntity(UserCommandDto entity) => entity.MapToEntity();

        public async Task<UserTokenDto> AutenticarUsuario(UserCommandDto userDto)
        {
            if (userDto == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.ModeloInvalido);
                return null;
            }

            var usuario = await _repository
                .Get()
                .Include(c => c.Permissoes)
                .SingleOrDefaultAsync(u => u.Email == userDto.Email);

            if (usuario == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, Message.EmailNaoEncontrado);
                return null;
            }

            bool senhaValida = VerificarSenhaHash(userDto.Password, usuario.Password, usuario.Salt);

            if (!senhaValida)
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.SenhaInvalida);
                return null;
            }

            return GerarToken(usuario);
        }

        public bool VerificarPermissao(params EnumPermissoes[] permissoesParaValidar)
        {
            var permissoes = _httpContext?.User?.Claims?.Select(claim => claim.Value.ToString());

            var possuiPermissao = permissoesParaValidar
                .Select(permissao => permissao.ToString())
                .All(permissao => permissoes.Any(x => x == permissao));

            return possuiPermissao;
        }

        public async Task AddPermissaoAsync(UserPermissionCommandDto userPermissao)
        {
            var usuario = await _repository
                .Get(user => user.Id == userPermissao.UsuarioId)
                .Include(p => p.Permissoes)
                .FirstOrDefaultAsync();

            foreach (var permissao in userPermissao.Permissoes)
            {
                var possuiPermissao = usuario
                    .Permissoes.Where(p => p.Descricao == permissao.ToString())
                    .FirstOrDefault();

                if (possuiPermissao is null)
                {
                    usuario.Permissoes.Add(new Permissao { Descricao = permissao.ToString() });
                }
            }

            _repository.Update(usuario);
            await _repository.SaveChangesAsync();
        }

        #region Supports Methods
        private bool VerificarSenhaHash(string senha, string senhaHash, string salt) =>
            new PasswordHasherHelper().CompareHash(senha, salt) == senhaHash;

        private UserTokenDto GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenExpirationTime = DateTime.UtcNow.AddYears(
                int.Parse(_configuration["TokenConfiguration:ExpireDays"])
            );

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, usuario.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (usuario.Permissoes.Count > 0)
            {
                foreach (var permissao in usuario.Permissoes)
                {
                    claims.Add(new Claim("Permission", permissao.Descricao));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpirationTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Audience = _configuration["TokenConfiguration:Audience"],
                Issuer = _configuration["TokenConfiguration:Issuer"]
            };

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return new UserTokenDto()
            {
                Authenticated = true,
                Token = tokenHandler.WriteToken(token),
                Expiration = tokenExpirationTime,
            };
        }
        #endregion
    }
}
