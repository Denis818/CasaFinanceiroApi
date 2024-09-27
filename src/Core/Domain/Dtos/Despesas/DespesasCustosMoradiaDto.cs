namespace Domain.Dtos.Despesas
{
    public class DespesasCustosMoradiaDto
    {
        public double ParcelaApartamento { get; set; }
        public double ParcelaCaixa { get; set; }
        public double ContaDeLuz { get; set; }
        public double Condominio { get; set; }

        public int MembrosForaJhonPeuCount { get; set; }
        public int MembrosForaJhonCount { get; set; }
    }
}
