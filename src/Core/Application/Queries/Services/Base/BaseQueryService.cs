using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Base;
using Domain.Interfaces.Utilities;
using Domain.Models.Despesas;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Queries.Services.Base
{
    public abstract class BaseQueryService<TEntity, TIRepository>
        where TEntity : class, new()
        where TIRepository : class, IRepositoryBase<TEntity>
    {
        private readonly INotifier _notificador;
        private readonly IDespesaRepository _despesaRepository;

        protected readonly TIRepository _repository;
        protected readonly HttpContext _httpContext;
        protected readonly ICategoriaRepository _categoriaRepository;

        protected readonly int _grupoId;
        protected readonly CategoriaIdsDto _categoriaIds;
        protected readonly IQueryable<Despesa> _queryDespesasPorGrupo;

        public BaseQueryService(IServiceProvider service)
        {
            _notificador = service.GetRequiredService<INotifier>();
            _despesaRepository = service.GetRequiredService<IDespesaRepository>();

            _repository = service.GetRequiredService<TIRepository>();
            _httpContext = service.GetRequiredService<IHttpContextAccessor>().HttpContext;
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();

            _categoriaIds = _categoriaRepository.GetCategoriaIds();
            _grupoId = (int)(_httpContext.Items["GrupoFaturaId"] ?? 0);

            _queryDespesasPorGrupo = _despesaRepository
                .Get(d => d.GrupoFaturaId == _grupoId)
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura);
        }

        public void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notificador.Notify(tipo, message);
    }
}