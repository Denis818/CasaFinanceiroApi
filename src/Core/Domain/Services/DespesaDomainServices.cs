using Domain.Dtos.Despesas;
using Domain.Extensions.Help;
using Domain.Interfaces.Services.Despesa;

namespace Domain.Services
{
    public class DespesaDomainServices : IDespesaDomainServices
    {
        public DespesasDistribuicaoCustosMoradiaDto CalcularDistribuicaoCustosMoradia(
            DespesasCustosMoradiaDto custosDespesasMoradia
        )
        {
            double totalAptoMaisCaixa =
                custosDespesasMoradia.ParcelaApartamento + custosDespesasMoradia.ParcelaCaixa;

            double totalLuzMaisCondominio =
                custosDespesasMoradia.ContaDeLuz + custosDespesasMoradia.Condominio;

            double totalAptoMaisCaixaAbate300Peu100Estacionamento = (
                totalAptoMaisCaixa - 300 - 100
            ).RountToZeroIfNegative(); //300 aluguel cobrado do peu. 100 reais do estacionamento alugado.

            double valorAptoMaisCaixaParaCadaMembro = (
                totalAptoMaisCaixaAbate300Peu100Estacionamento
                / custosDespesasMoradia.MembrosForaJhonPeuCount
            ).RountToZeroIfNegative();

            double valorLuzMaisCondominioParaCadaMembro = (
                totalLuzMaisCondominio / custosDespesasMoradia.MembrosForaJhonCount
            ).RountToZeroIfNegative();

            double valorParaMembrosForaPeu =
                valorAptoMaisCaixaParaCadaMembro + valorLuzMaisCondominioParaCadaMembro;

            double valorParaDoPeu = 300 + valorLuzMaisCondominioParaCadaMembro;

            return new DespesasDistribuicaoCustosMoradiaDto()
            {
                ValorParaDoPeu = valorParaDoPeu,
                TotalAptoMaisCaixa = totalAptoMaisCaixa,
                TotalLuzMaisCondominio = totalLuzMaisCondominio,

                ValorParaMembrosForaPeu = valorParaMembrosForaPeu.RountToZeroIfNegative(),

                ValorAptoMaisCaixaParaCadaMembro =
                    valorAptoMaisCaixaParaCadaMembro.RountToZeroIfNegative(),

                TotalAptoMaisCaixaAbate300Peu100Estacionamento =
                    totalAptoMaisCaixaAbate300Peu100Estacionamento.RountToZeroIfNegative(),

                ValorLuzMaisCondominioParaCadaMembro =
                    valorLuzMaisCondominioParaCadaMembro.RountToZeroIfNegative(),
            };
        }

        public DespesasDistribuicaoCustosCasaDto CalcularDistribuicaoCustosCasa(
            DespesasCustosDespesasCasaDto custosDespesasCasa
        )
        {
            //  Almoço divido com Jhon
            double totalAlmocoParteDoJhon =
                custosDespesasCasa.ValorTotalAlmoco / custosDespesasCasa.TodosMembros.Count;

            // Total do almoço dividido com Jhon
            double totalDoAlmocoComAbateParteDoJhon =
                custosDespesasCasa.ValorTotalAlmoco - totalAlmocoParteDoJhon;

            //Calcular todas as depesas da casa.
            double despesaGeraisMaisAlmoco =
                custosDespesasCasa.TotalDespesaGeraisForaAlmoco
                + custosDespesasCasa.ValorTotalAlmoco;

            //Somando almoço/janta dividio com Jhon mais total das despesas gerais
            double TotalDespesasGeraisMaisAlmocoDividido =
                custosDespesasCasa.TotalDespesaGeraisForaAlmoco + totalDoAlmocoComAbateParteDoJhon;

            //Dividindo as despesas somadas
            double despesaGeraisMaisAlmocoDividioPorMembro =
                TotalDespesasGeraisMaisAlmocoDividido / custosDespesasCasa.MembrosForaJhonCount;

            return new DespesasDistribuicaoCustosCasaDto()
            {
                Membros = custosDespesasCasa.TodosMembros,
                TotalSomenteAlmoco = custosDespesasCasa.ValorTotalAlmoco,
                TotalDespesaGeraisForaAlmoco = custosDespesasCasa.TotalDespesaGeraisForaAlmoco,
                TotalAlmocoParteDoJhon = totalAlmocoParteDoJhon.RountToZeroIfNegative(),
                TotalDespesasGeraisMaisAlmocoDividido = TotalDespesasGeraisMaisAlmocoDividido,
                DespesaGeraisMaisAlmoco = despesaGeraisMaisAlmoco,

                DespesaGeraisMaisAlmocoDividioPorMembro =
                    despesaGeraisMaisAlmocoDividioPorMembro.RountToZeroIfNegative(),

                TotalAlmocoDividioComJhon =
                    totalDoAlmocoComAbateParteDoJhon.RountToZeroIfNegative(),
            };
        }
    }
}
