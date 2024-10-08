﻿using Domain.Models.Base;

namespace Domain.Models.GrupoFaturas
{
    public class StatusFatura : EntityBase
    {
        public int Id { get; set; }

        public string FaturaNome { get; set; }
        public string Estado { get; set; }
        public Guid GrupoFaturaCode { get; set; }
        public GrupoFatura GrupoFatura { get; set; }
        public int GrupoFaturaId { get; set; }
    }
}
