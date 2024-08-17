using Application.Commands.Services.Base;
using Domain.Converters.DatesTimes;
using Domain.Dtos;
using System.Text.Json.Serialization;

namespace Application.Commands.Dtos
{
    public class DespesaCommandDto : CommandBaseDTO
    {

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCompra { get; set; }
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public GrupoFaturaQueryDto GrupoFatura { get; set; }
        public CategoriaQueryDto Categoria { get; set; }
    }
}
