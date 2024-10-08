﻿using Domain.Models.Base;
using Domain.Models.Categorias;
using Domain.Models.GrupoFaturas;

namespace Domain.Models.Despesas
{
    public class Despesa : EntityBase
    {
        public int Id { get; set; }
        public DateTime DataCompra { get; set; }
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public double Total { get; set; }
        public GrupoFatura GrupoFatura { get; set; }
        public int GrupoFaturaId { get; set; }
        public Categoria Categoria { get; set; }
        public int CategoriaId { get; set; }
        public Guid GrupoFaturaCode { get; set; }
        public Guid CategoriaCode { get; set; }

    }
}
