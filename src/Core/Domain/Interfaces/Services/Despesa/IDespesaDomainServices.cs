using Domain.Dtos.Despesas;

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
