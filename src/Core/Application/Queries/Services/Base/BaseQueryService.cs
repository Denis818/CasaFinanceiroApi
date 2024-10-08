﻿using Domain.Dtos;
using Domain.Dtos.Base;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Base;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Interfaces.Utilities;
using Domain.Models.Base;
using Domain.Models.Despesas;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Queries.Services.Base
{
    public abstract class BaseQueryService<TEntity, TQueryDTO, TIRepository>
        where TIRepository : class, IRepositoryBase<TEntity>
        where TEntity : EntityBase
        where TQueryDTO : BaseDto
    {
        private readonly INotifier _notificador;
        private readonly IDespesaRepository _despesaRepository;

        protected readonly TIRepository _repository;
        protected readonly HttpContext _httpContext;
        protected readonly ICategoriaRepository _categoriaRepository;

        protected readonly Guid _grupoCode;
        protected readonly CategoriaCodsDto _categoriaIds;
        protected readonly IQueryable<Despesa> _queryDespesasPorGrupo;

        public BaseQueryService(IServiceProvider service)
        {
            _notificador = service.GetRequiredService<INotifier>();
            _despesaRepository = service.GetRequiredService<IDespesaRepository>();

            _repository = service.GetRequiredService<TIRepository>();
            _httpContext = service.GetRequiredService<IHttpContextAccessor>().HttpContext;
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();

            _categoriaIds = _categoriaRepository.GetCategoriaCodesAsync().Result;
            _grupoCode = (Guid)(
                _httpContext.Items["grupo-fatura-code"]
                ?? new Guid("00000000-0000-0000-0000-000000000000")
            );

            _queryDespesasPorGrupo = _despesaRepository
                .Get(d => d.GrupoFaturaCode == _grupoCode)
                .Include(c => c.Categoria)
                .Include(g => g.GrupoFatura);
        }

        public void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notificador.Notify(tipo, message);

        public virtual async Task<TQueryDTO> GetByCodigoAsync(Guid codigo)
        {
            var result = await _repository
                .Get(entity => entity.Code == codigo)
                .FirstOrDefaultAsync();
            return MapToDTO(result);
        }

        protected abstract TQueryDTO MapToDTO(TEntity entity);
    }
}
