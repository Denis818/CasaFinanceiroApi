﻿using Application.Commands.Dtos;
using Domain.Dtos;
using Domain.Models.GrupoFaturas;

namespace Application.Configurations.MappingsApp
{
    public static class GrupoFaturaMap
    {
        public static GrupoFaturaDto MapToDTO(this GrupoFatura grupoFatura)
        {
            return new GrupoFaturaDto
            {
                Nome = grupoFatura.Nome,
                Ano = grupoFatura.Ano,
                Code = grupoFatura.Code,
                DataCriacao = grupoFatura.DataCriacao,
                StatusFaturas = grupoFatura.StatusFaturas.Select(s => s.MapToDTO()).ToList()
            };
        }

        public static GrupoFatura MapToEntity(this GrupoFaturaCommandDto grupoFaturaDto)
        {
            return new GrupoFatura
            {
                Nome = grupoFaturaDto.Nome,
                Ano = grupoFaturaDto.Ano,
            };
        }

        public static void MapUpdateEntity(
            this GrupoFatura grupoFatura,
            GrupoFaturaCommandDto grupoFaturaDto
        )
        {
            grupoFatura.Nome = grupoFaturaDto.Nome;
            grupoFatura.Ano = grupoFaturaDto.Ano;
        }
    }
}
