namespace Domain.Dtos.Despesas
{
    public class DetalhamentoDespesasMoradiaQueryDto
    {
        public DespesasCustosMoradiaQueryDto CustosDespesasMoradia { get; set; } = new();
        public GrupoListMembrosDespesaQueryDto GrupoListMembrosDespesa { get; set; } = new();
        public DespesasDistribuicaoCustosMoradiaDto DistribuicaoCustos { get; set; } = new();
    }
}
