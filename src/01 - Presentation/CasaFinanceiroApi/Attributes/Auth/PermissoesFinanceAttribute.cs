﻿using CasaFinanceiroApi.Base;
using Domain.Enumeradores;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasaFinanceiroApi.Attributes.Auth
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PermissoesFinanceAttribute(params EnumPermissoes[] enumPermissoes)
        : Attribute,
            IAuthorizationFilter
    {
        private IEnumerable<string> EnumPermissoes { get; } =
            enumPermissoes.Select(x => x.ToString());

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var possuiTodasPermissoes = EnumPermissoes.All(permissao =>
                context.HttpContext.User.Claims.Any(claim => claim.Value == permissao)
            );

            if (!possuiTodasPermissoes)
            {
                var response = new ResponseDTO<string>()
                {
                    Mensagens = [new Notificacao("Oops você não tem permissão.")]
                };

                context.Result = new ObjectResult(response) { StatusCode = 401 };
                return;
            }
        }
    }
}
