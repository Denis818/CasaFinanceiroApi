using Application.Queries.Dtos;

namespace Application.Queries.Interfaces
{
    public interface IComparativoFaturasQueryServices
    {
        Task<List<ComparativoFaturasQueryDto>> GetComparativoFaturasAsync(Guid grupoFaturaCode1, Guid grupoFaturaCode2);
    }
}