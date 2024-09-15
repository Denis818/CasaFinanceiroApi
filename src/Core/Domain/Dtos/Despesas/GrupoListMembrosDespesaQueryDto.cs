namespace Domain.Dtos.Despesas
{
    public class GrupoListMembrosDespesaQueryDto
    {
        public IList<DespesaQueryDto> ListAluguel { get; set; } = [];
        public IList<MembroQueryDto> ListMembroForaJhon { get; set; } = [];
        public IList<MembroQueryDto> ListMembroForaJhonPeu { get; set; } = [];
    }
}
