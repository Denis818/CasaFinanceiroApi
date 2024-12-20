﻿using Domain.Interfaces.Repositories.Base;
using Domain.Models.Despesas;

namespace Domain.Interfaces.Repositories
{
    public interface IDespesaRepository : IRepositoryBase<Despesa>
    {
        Lazy<IList<Despesa>> GetListDespesasPorGrupo(Guid grupoCod);
    }
}
