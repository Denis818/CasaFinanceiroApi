using Domain.Enumeradores;
using Domain.Interfaces.Utilities;
using Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ModelState.Interface;

namespace Presentation.Api.Base
{
    public abstract class MainController(IServiceProvider service) : Controller
    {
        private readonly INotifier _notifier = service.GetRequiredService<INotifier>();

        private readonly IModelStateValidator _modelState =
            service.GetRequiredService<IModelStateValidator>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_modelState.ValidarModelState(context))
                return;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            if (context.Result is ObjectResult result)
            {
                context.Result = CustomResponse(result.Value);
            }
        }

        private IActionResult CustomResponse<TResponse>(TResponse content)
        {
            if (_notifier.HasNotifications(EnumTipoNotificacao.NotFount, out var notFount))
            {
                return NotFound(new ResponseDTO<TResponse>(content) { Mensagens = notFount });
            }

            if (_notifier.HasNotifications(EnumTipoNotificacao.ClientError, out var clientErrors))
            {
                return BadRequest(new ResponseDTO<TResponse>(content) { Mensagens = clientErrors });
            }

            if (_notifier.HasNotifications(EnumTipoNotificacao.ServerError, out var serverErrors))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseDTO<TResponse>(content) { Mensagens = serverErrors }
                );
            }

            _notifier.HasNotifications(EnumTipoNotificacao.Informacao, out var infoMessages);
            return Ok(new ResponseDTO<TResponse>(content) { Mensagens = infoMessages });
        }

        protected void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notifier.Notify(tipo, message);
    }

    public class ResponseDTO<T>
    {
        public T Dados { get; set; }
        public Notificacao[] Mensagens { get; set; }

        public ResponseDTO(T data, Notificacao[] messages = null)
        {
            Dados = data;
            Mensagens = messages ?? [];
        }

        public ResponseDTO() { }

        public void ContentTypeInvalido()
        {
            Mensagens = [new("Content-Type inválido.", EnumTipoNotificacao.ClientError)];
        }
    }
}
