namespace Domain.Dtos.Despesas
{
    public class GrupoListMembrosDespesaDto
    {
        public IList<DespesaDto> ListAluguel { get; set; } = [];
        public IList<MembroDto> ListMembroForaJhon { get; set; } = [];
        public IList<MembroDto> ListMembroForaJhonPeu { get; set; } = [];
    }
}
