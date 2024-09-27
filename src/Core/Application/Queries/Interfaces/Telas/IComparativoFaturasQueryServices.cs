using Application.Queries.Dtos;

namespace Application.Queries.Interfaces.Telas
{
    public interface IComparativoFaturasQueryServices
    {
        Task<List<ComparativoFaturasQueryDto>> GetComparativoFaturasAsync(Guid grupoFaturaCode1, Guid grupoFaturaCode2);
    }
}