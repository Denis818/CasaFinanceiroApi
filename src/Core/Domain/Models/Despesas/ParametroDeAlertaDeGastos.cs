namespace Domain.Models.Despesas
{
    public class ParametroDeAlertaDeGastos
    {
        public ParametroDeAlertaDeGastos()
        {
            Code = Guid.NewGuid();
        }

        public int Id { get; set; }
        public Guid Code { get; set; }

        public string TipoMetrica { get; set; }
        public decimal LimiteVermelho { get; set; }
        public decimal LimiteAmarelo { get; set; }
    }
}
