using Application.Interfaces.Utilities;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories.Base;
using Domain.Models.Despesas;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Base
{
    public abstract class BaseAppService<TEntity, TIRepository>
        where TEntity : class, new()
        where TIRepository : class, IRepositoryBase<TEntity>
    {
        private readonly IServiceProvider _service;
        private readonly INotifier _notificador;

        protected readonly TIRepository _repository;

        protected readonly HttpContext _httpContext;

        public BaseAppService(IServiceProvider service)
        {
            _service = service;

            _notificador = service.GetRequiredService<INotifier>();
            _repository = service.GetRequiredService<TIRepository>();

            _httpContext = service.GetRequiredService<IHttpContextAccessor>().HttpContext;
        }

        public void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notificador.Notify(tipo, message);

        protected bool Validator<TEntityDto>(TEntityDto entityDto)
        {
            var validator = _service.GetService<IValidator<TEntityDto>>();

            ValidationResult results = validator.Validate(entityDto);

            if (!results.IsValid)
            {
                var groupedFailures = results
                    .Errors.GroupBy(failure => failure.PropertyName)
                    .Select(group => new
                    {
                        PropertyName = group.Key,
                        Errors = string.Join(" ", group.Select(err => err.ErrorMessage))
                    });

                foreach (var failure in groupedFailures)
                {
                    Notificar(EnumTipoNotificacao.Informacao, $"{failure.Errors}");
                }

                return true;
            }

            return false;
        }
    }
}