﻿using Application.Extensions;
using Domain.Dtos;
using Domain.Dtos.Base;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Base;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Interfaces.Repositories.GrupoFaturas;
using Domain.Interfaces.Repositories.Membros;
using Domain.Interfaces.Utilities;
using Domain.Models.Base;
using Domain.Models.Despesas;
using Domain.Models.GrupoFaturas;
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

        protected readonly Guid _grupoCode;

        protected readonly IDespesaRepository _despesaRepository;
        protected readonly IMembroRepository _membroRepository;
        protected readonly TIRepository _repository;
        protected readonly HttpContext _httpContext;
        protected readonly ICategoriaRepository _categoriaRepository;
        protected readonly IGrupoFaturaRepository _grupoFaturaRepository;

        protected IList<Despesa> ListDespesasPorGrupo => _lazyListDespesasPorGrupo.Value;
        private readonly Lazy<IList<Despesa>> _lazyListDespesasPorGrupo;

        public MembroIdDto MembroCods => _lazyMembroIds.Value;
        private readonly Lazy<MembroIdDto> _lazyMembroIds;

        public CategoriaCodsDto CategoriaCods => _lazyCategoriaIds.Value;
        private readonly Lazy<CategoriaCodsDto> _lazyCategoriaIds;

        public GrupoFatura _grupoFatura => _lazyGrupoFatura.Value;
        private readonly Lazy<GrupoFatura> _lazyGrupoFatura;

        public BaseQueryService(IServiceProvider service)
        {
            _notificador = service.GetRequiredService<INotifier>();
            _despesaRepository = service.GetRequiredService<IDespesaRepository>();

            _repository = service.GetRequiredService<TIRepository>();
            _httpContext = service.GetRequiredService<IHttpContextAccessor>().HttpContext;
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            _grupoFaturaRepository = service.GetRequiredService<IGrupoFaturaRepository>();
            _membroRepository = service.GetRequiredService<IMembroRepository>();

            _grupoCode = _httpContext.GetCurrentGrupoFaturaCode();
            _lazyListDespesasPorGrupo = _despesaRepository.GetListDespesasPorGrupo(_grupoCode);
            _lazyMembroIds = _membroRepository.GetMembroCods();
            _lazyCategoriaIds = _categoriaRepository.GetCategoriaCods();
            _lazyGrupoFatura = _grupoFaturaRepository.GetGrupoFatura(_grupoCode);
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
