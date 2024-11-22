using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories.Base;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Interfaces.Utilities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Commands.Services.Base
{
    public abstract class BaseCommandService<TEntity, TEntityDTO, TIRepository>(
        IServiceProvider service
    )
        where TIRepository : class, IRepositoryBase<TEntity>
        where TEntity : class, new()
        where TEntityDTO : class, new()
    {
        private readonly INotifier _notificador = service.GetRequiredService<INotifier>();

        protected readonly TIRepository _repository = service.GetRequiredService<TIRepository>();

        public CategoriaCodsDto CategoriaCods => _lazyCategoriaIds.Value;

        private readonly Lazy<CategoriaCodsDto> _lazyCategoriaIds = service
            .GetRequiredService<ICategoriaRepository>()
            .GetCategoriaCods();

        protected readonly HttpContext _httpContext = service
            .GetRequiredService<IHttpContextAccessor>()
            .HttpContext;

        public void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notificador.Notify(tipo, message);

        protected bool Validator<TEntityDto>(TEntityDto entityDto)
        {
            var validator = service.GetService<IValidator<TEntityDto>>();

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

        protected abstract TEntity MapToEntity(TEntityDTO entityDTO);
    }
}
