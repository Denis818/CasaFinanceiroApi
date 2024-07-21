/*using Application.Services.Base;
using Domain.Dtos.Categorias.Consultas;
using Domain.Dtos.Membros;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services.Despesas.Base
{
    public abstract class BaseDespesaService : BaseAppService<Despesa, IDespesaRepository>
    {
        private readonly IGrupoFaturaRepository _grupoFaturaRepository;

        protected readonly ICategoriaRepository _categoriaRepository;
        protected readonly IMembroRepository _membroRepository;

        protected readonly CategoriaIdsDto _categoriaIds;
        protected readonly MembroIdDto _membroId;

        protected IQueryable<Despesa> _queryDespesasPorGrupo;

        public BaseDespesaService(IServiceProvider service)
            : base(service)
        {
            _membroRepository = service.GetRequiredService<IMembroRepository>();
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            _categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            _grupoFaturaRepository = service.GetRequiredService<IGrupoFaturaRepository>();

            _membroId = _membroRepository.GetMembersIds();
            _categoriaIds = _categoriaRepository.GetCategoriaIds();

            _queryDespesasPorGrupo = GetDespesasByGroup();
        }

        public IQueryable<Despesa> GetDespesasByGroup()
        {
            var grupoId = (int)(_httpContext.Items["GrupoFaturaId"] ?? 0);

            _grupoFatura = _grupoFaturaRepository.GetByIdAsync(grupoId).Result;

            var queryDespesas = _repository
                .Get(d => d.GrupoFaturaId == grupoId)
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura);

            return queryDespesas;
        }
    }
}
*/