namespace Domain.Dtos.Despesas.Consultas
{
    public class DespesasCustosMoradiaQueryDto
    {
        public double ParcelaApartamento { get; set; }
        public double ParcelaCaixa { get; set; }
        public double ContaDeLuz { get; set; }
        public double Condominio { get; set; }

        public int MembrosForaJhonPeuCount { get; set; }
        public int MembrosForaJhonCount { get; set; }
    }

    public class GrupoListMembrosDespesaDto
    {
        public IList<DespesaQueryDto> ListAluguel { get; set; } = [];
        public IList<MembroQueryDto> ListMembroForaJhon { get; set; } = [];
        public IList<MembroQueryDto> ListMembroForaJhonPeu { get; set; } = [];
    }

    public class DetalhamentoDespesasMoradiaDto
    {
        public DespesasCustosMoradiaQueryDto CustosDespesasMoradia { get; set; } = new();
        public GrupoListMembrosDespesaDto GrupoListMembrosDespesa { get; set; } = new();
        public DespesasDistribuicaoCustosMoradiaDto DistribuicaoCustos { get; set; } = new();
    }
}
