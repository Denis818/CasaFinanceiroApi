using Domain.Converters.DatesTimes;
using Domain.Dtos.Base;
using Domain.Dtos.QueryResults;
using System.Text.Json.Serialization;

namespace Domain.Dtos
{
    public class DespesaQueryDto : QueryBaseDTO
    {

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCompra { get; set; }
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public double Total { get; set; }
        public GrupoFaturaQueryDto GrupoFatura { get; set; }
        public CategoriaQueryResult Categoria { get; set; }
    }
}
