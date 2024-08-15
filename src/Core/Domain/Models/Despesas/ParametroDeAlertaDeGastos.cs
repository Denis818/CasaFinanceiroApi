namespace Domain.Models.Despesas
{
    public class ParametroDeAlertaDeGastos
    {
        public int Id { get; set; }
        public string TipoMetrica { get; set; }
        public decimal LimiteVermelho { get; set; }
        public decimal LimiteAmarelo { get; set; }
    }
}
