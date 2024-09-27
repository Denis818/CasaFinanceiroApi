using Domain.Converters.DatesTimes;
using Domain.Dtos.Base;
using Domain.Dtos.Categoria;
using System.Text.Json.Serialization;

namespace Domain.Dtos
{
    public class DespesaDto : BaseDto
    {

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCompra { get; set; }
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public double Total { get; set; }
        public GrupoFaturaDto GrupoFatura { get; set; }
        public CategoriaDto Categoria { get; set; }
    }
}
