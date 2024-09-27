using Domain.Dtos.Despesas;

namespace Domain.Interfaces.Services.Despesa
{
    public interface IDespesaDomainServices
    {
        DespesasDistribuicaoCustosMoradiaDto CalcularDistribuicaoCustosMoradia(
            DespesasCustosMoradiaDto custosDespesasMoradia
        );

        DespesasDistribuicaoCustosCasaDto CalcularDistribuicaoCustosCasa(
            DespesasCustosDespesasCasaDto custosDespesasCasa
        );
    }
}
