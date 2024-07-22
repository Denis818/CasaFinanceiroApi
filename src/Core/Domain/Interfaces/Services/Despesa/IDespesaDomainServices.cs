using Domain.Dtos.Despesas;
using Domain.Dtos.Despesas.Consultas;

namespace Domain.Interfaces.Services.Despesa
{
    public interface IDespesaDomainServices
    {
        DespesasDistribuicaoCustosMoradiaDto CalcularDistribuicaoCustosMoradia(
            DespesasCustosMoradiaQueryDto custosDespesasMoradia
        );

        DespesasDistribuicaoCustosCasaQueryDto CalcularDistribuicaoCustosCasa(
            DespesasCustosDespesasCasaQueryDto custosDespesasCasa
        );
    }
}
