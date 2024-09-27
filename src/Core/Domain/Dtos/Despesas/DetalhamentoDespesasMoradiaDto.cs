namespace Domain.Dtos.Despesas
{
    public class DetalhamentoDespesasMoradiaDto
    {
        public DespesasCustosMoradiaDto CustosDespesasMoradia { get; set; } = new();
        public GrupoListMembrosDespesaDto GrupoListMembrosDespesa { get; set; } = new();
        public DespesasDistribuicaoCustosMoradiaDto DistribuicaoCustos { get; set; } = new();
    }
}
