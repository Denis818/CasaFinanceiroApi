﻿namespace Domain.Dtos.QueryResults.Despesas
{
    public class RelatorioGastosDoGrupoQueryResult
    {
        public string GrupoFaturaNome { get; set; }
        public double TotalGastosMoradia { get; set; }
        public double TotalGastosCasa { get; set; }
        public double TotalGeral { get; set; }
    }
}
