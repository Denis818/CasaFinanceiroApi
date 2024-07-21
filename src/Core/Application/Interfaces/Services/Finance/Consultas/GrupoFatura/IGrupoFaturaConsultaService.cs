using Domain.Dtos.Despesas.Criacao;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Finance.Consultas
{
    public interface IGrupoFaturaConsultaService
    {
        Task<IEnumerable<GrupoFatura>> GetAllAsync(string ano);
        Task<string> GetNameFatura(int id);
        Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status);
    }
}